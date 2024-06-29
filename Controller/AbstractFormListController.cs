using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Windows.Input;
using FrontEnd.Events;
using Backend.Enums;
using Backend.Events;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Extends <see cref="AbstractFormController{M}"/> and implements <see cref="IAbstractFormListController{M}"/> 
    /// to provide functionality for managing form lists and opening windows for records.
    /// </summary>
    /// <typeparam name="M">An <see cref="IAbstractModel"/> object.</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IAbstractFormListController<M> where M : IAbstractModel, new()
    {
        bool _openWindowOnNew = true;

        #region Commands
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether a new window should be opened when a new record is created.
        /// </summary>
        public bool OpenWindowOnNew
        {
            get => _openWindowOnNew;
            set => _openWindowOnNew = value;
        }
        #endregion

        public AbstractFormListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD(OpenNew);
            RecordSource.RunFilter += OnSourceRunFilter;
        }

        /// <summary>
        /// Requeries the database table asynchronously. This method is called by the <see cref="RequeryCMD"/> command.
        /// It retrieves the records and replaces the existing ones in the <see cref="Db"/> and <see cref="Source"/> properties.
        /// </summary>
        public override async Task RequeryAsync()
        {
            IsLoading = true; // Notify the GUI that a process is ongoing.
            await Task.Delay(10);
            IEnumerable<M>? results = null;
            await Task.Run(async () => // Retrieve the records without freezing the GUI.
            {
                results = await RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync().Cast<M>());
            });

            if (results == null) throw new Exception("Source is null"); // Handle error case.
            Db.ReplaceRecords(results.Cast<ISQLModel>().ToList()); // Replace master record source records.

            try
            {
                OnSubFormFilter();
                IsLoading = false;
                return;
            }
            catch
            {
                try
                {
                    OnOptionFilterClicked(new FilterEventArgs());
                    IsLoading = false;
                    return;
                }
                catch (NotImplementedException)
                {
                    try
                    {
                        results = await SearchRecordAsync();
                    }
                    catch (NotImplementedException) { }
                }
            }

            RecordSource.ReplaceRecords(results); // Update child source for this controller.
            IsLoading = false; // Notify the GUI that the process has completed.
        }

        /// <summary>
        /// Handles requerying the data source based on the search property asynchronously.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        protected async Task OnSearchPropertyRequeryAsync(object? sender)
        {
            IEnumerable<M> results = await Task.Run(SearchRecordAsync);
            RecordSource.ReplaceRecords(results);

            if (sender is not FilterEventArgs filterEvtArgs)
                GoFirst();
        }

        /// <summary>
        /// Searches for records asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the search results.</returns>
        public abstract Task<IEnumerable<M>> SearchRecordAsync();

        public abstract void OnOptionFilterClicked(FilterEventArgs e);

        public override abstract AbstractClause InstantiateSearchQry();

        #region Open Windows
        /// <summary>
        /// Opens a new window to view the selected record. Override this method to provide custom logic for opening windows.
        /// </summary>
        /// <param name="model">An <see cref="AbstractModel"/> object representing the record to visualize in the new window.</param>
        protected abstract void Open(M model);

        /// <summary>
        /// Calls the <see cref="Open(M)"/> method, passing a new instance of <see cref="AbstractModel"/>.
        /// </summary>
        protected void OpenNew() => Open(new());
        #endregion

        #region Event Subscriptions
        /// <summary>
        /// Handles the run filter event of the data source.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSourceRunFilter(object? sender, FilterEventArgs e) => OnOptionFilterClicked(e);
        #endregion

        #region Go To
        /// <summary>
        /// Cleans the data source by removing new records if <see cref="OpenWindowOnNew"/> is false.
        /// </summary>
        public void CleanSource()
        {
            if (OpenWindowOnNew) return;
            List<M> toRemove = RecordSource.Where(s => s.IsNewRecord()).ToList(); // Get only the new records in the collection.

            foreach (var item in toRemove)
                RecordSource.Remove(item); // Remove them.
        }

        public override bool GoNew()
        {
            if (!AllowNewRecord) return false;
            if (OpenWindowOnNew)
            {
                if (base.GoNew()) // Tell the Navigator to add a new record.
                {
                    OpenNew(); // Open a new window displaying the new record.
                    return true;
                }
                return false;
            }
            if (!CanMove()) return false; // Cannot move to a new record because the current record breaks integrity rules.
            if (InvokeBeforeRecordNavigationEvent(RecordMovement.GoNew)) return false; // Event was cancelled.

            if (RecordSource.Any(s => s.IsNewRecord())) return false; // If there is already a new record, exit the method.
            RecordSource.Add(new M()); // Add a new record to the collection.
            Navigator.GoLast(); // Move to the last record, which is a new record.
            CurrentRecord = Navigator.Current; // Set the CurrentModel property.
            if (InvokeAfterRecordNavigationEvent(RecordMovement.GoNew)) return false; // If using SubForms, invoke the OnNewRecordEvent().
            Records = "New Record"; // Update RecordTracker's record displayer.
            return true;
        }

        public override bool GoPrevious()
        {
            if (!CanMove()) return false;
            CleanSource();
            return base.GoPrevious();
        }

        public override bool GoLast()
        {
            if (!CanMove()) return false;
            CleanSource();
            return base.GoLast();
        }

        public override bool GoFirst()
        {
            if (!CanMove()) return false;
            CleanSource();
            return base.GoFirst();
        }

        public override bool GoAt(ISQLModel? record)
        {
            if (!CanMove()) return false;

            if (record == null)
            {
                CurrentRecord = default;
                Records = Source.RecordPositionDisplayer();
                return false;
            }

            if (record.IsNewRecord() && OpenWindowOnNew) return GoNew();
            if (record.IsNewRecord() && !OpenWindowOnNew)
            {
                if (InvokeBeforeRecordNavigationEvent(RecordMovement.GoAt)) return false; // Event was cancelled.
                bool result = Navigator.GoNew();
                if (InvokeAfterRecordNavigationEvent(RecordMovement.GoAt)) return false; // Event was cancelled.
                return result;
            }

            CleanSource();
            Navigator.GoAt(record);
            CurrentRecord = Navigator.Current;
            Records = Source.RecordPositionDisplayer();

            if (InvokeAfterRecordNavigationEvent(RecordMovement.GoAt)) return false; // Event was cancelled.

            return true;
        }
        #endregion

        #region Alter Record
        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; // No changes to update.
            if (!CurrentRecord.AllowUpdate()) return false; // Record did not meet update criteria.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            IAbstractModel temp = CurrentRecord;

            if (crud == CRUD.INSERT)
            {   // INSERT must follow a slightly different logic if the user is allowed to insert new rows themselves. This is to avoid unexpected behavior between the RecordSource and the list object.
                if (RecordSource.Count > 0 && !OpenWindowOnNew)
                {
                    temp = RecordSource[Source.Count - 1];
                    RecordSource.RemoveAt(Source.Count - 1);
                }
                ExecuteCRUD(ref temp, crud, sql, parameters);
            }
            else ExecuteCRUD(ref temp, crud, sql, parameters);
            Db.MasterSource?.NotifyChildren(crud, temp);
            return true;
        }

        /// <summary>
        /// Executes a CRUD operation. This method is used in <see cref="AlterRecord(string?, List{QueryParameter}?)"/> to avoid code repetition.
        /// </summary>
        /// <param name="temp">The model to alter.</param>
        /// <param name="crud">The CRUD operation to perform.</param>
        /// <param name="sql">The optional SQL query.</param>
        /// <param name="parameters">The optional query parameters.</param>
        private void ExecuteCRUD(ref IAbstractModel temp, CRUD crud, string? sql = null, List<QueryParameter>? parameters = null)
        {
            Db.Model = temp;
            Db.Crud(crud, sql, parameters);
            temp.IsDirty = false;
        }
        #endregion

        /// <summary>
        /// Disposes the resources used by the controller.
        /// </summary>
        public override void Dispose()
        {
            SearchQry.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}