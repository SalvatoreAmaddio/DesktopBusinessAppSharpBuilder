﻿using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Windows.Input;
using FrontEnd.Events;
using Backend.Enums;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class extends <see cref="AbstractFormListController{M}"/> and implements <see cref="IAbstractFormListController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        bool _openWindowOnNew = true;

        #region Commands
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        #endregion

        #region Properties
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
            AsRecordSource().RunFilter += OnSourceRunFilter;
        }
        public override async Task RequeryAsync()
        {
            IsLoading = true; //notify the GUI that there is a process going on.
            await Task.Delay(10);
            IEnumerable<M>? results = null;
            await Task.Run(async () => //retrieve the records. Do not freeze the GUI.
            {
                results = await RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync().Cast<M>());
            });

            if (results == null) throw new Exception("Source is null"); //Something has gone wrong.
            Db.ReplaceRecords(results.ToList<ISQLModel>()); //Replace the Master RecordSource's records with the newly fetched ones.

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

            AsRecordSource().ReplaceRecords(results); //Update also its child source for this controller.
            IsLoading = false; //Notify the GUI the process has terminated
        }
        protected async Task OnSearchPropertyRequeryAsync(object? sender)
        {
            IEnumerable<M> results = await Task.Run(SearchRecordAsync);
            AsRecordSource().ReplaceRecords(results);

            if (sender is not FilterEventArgs filterEvtArgs)
                GoFirst();
        }
        public abstract Task<IEnumerable<M>> SearchRecordAsync();
        public abstract void OnOptionFilterClicked(FilterEventArgs e);

        #region Open Windows
        /// <summary>
        /// Override this method to open a new window to view the selected record. <para/>
        /// For Example:
        /// <code>
        ///  EmployeeForm win = new (model);
        ///  win.Show();
        /// </code>
        /// </summary>
        /// <param name="model">An <see cref="AbstractModel"/> object which is the record to visualise in the new Window</param>
        protected abstract void Open(M model);

        /// <summary>
        /// Calls the <see cref="Open(M)"/> by passing a new instance of <see cref="AbstractModel"/>.
        /// </summary>
        protected void OpenNew() => Open(new());
        #endregion

        #region Event Subscriptions
        private void OnSourceRunFilter(object? sender, FilterEventArgs e) => OnOptionFilterClicked(e);
        #endregion

        #region Go At
        public void CleanSource()
        {
            if (OpenWindowOnNew) return;
            List<M> toRemove = AsRecordSource().Where(s => s.IsNewRecord()).ToList(); //get only the records which are new in the collection.

            foreach (var item in toRemove)
                AsRecordSource().Remove(item); //get rid of them.
        }
        public override bool GoNew()
        {
            if (!AllowNewRecord) return false;
            if (OpenWindowOnNew) 
            {
                base.GoNew(); //tell the Navigator to add a new record.
                OpenNew(); //open a new window displaying the new record.
                return true;
            }
            if (!CanMove()) return false; //Cannot move to a new record because the current record break integrity rules.
            if (AsRecordSource().Any(s => s.IsNewRecord())) return false; //If there is already a new record exit the method.
            AsRecordSource().Add(new M()); //add a new record to the collection.
            Navigator.MoveLast(); //Therefore, you can now move to the last record which is indeed a new record.
            CurrentModel = Navigator.Current; //set the CurrentModel property.
            InvokeOnRecordMovedEvent(RecordMovement.GoNew); //if you are using SubForms, Invoke the the OnNewRecordEvent().
            Records = "New Record"; //update RecordTracker's record displayer.
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
                CurrentModel = null;
                Records = Source.RecordPositionDisplayer();
                return false;
            }
            else if (record.IsNewRecord() && OpenWindowOnNew) return GoNew();
            else if (record.IsNewRecord() && !OpenWindowOnNew) return Navigator.MoveNew();
            else
            {
                CleanSource();
                Navigator.MoveAt(record);
                CurrentModel = Navigator.Current;
                Records = Source.RecordPositionDisplayer();
                return true;    
            }
        }
        #endregion

        #region AlterRecord
        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; //if the record has not been changed there is nothing to update.
            if (!CurrentRecord.AllowUpdate()) return false; //the record did not meet the criteria to be updated.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            AbstractModel temp = CurrentRecord;

            if (crud == CRUD.INSERT) 
            {   //INSERT must follow a slighlty different logic if it the user is allowed to insert new rows himself. This is to avoid unexpected behaviour between the RecordSource and the Lista object.
                if (AsRecordSource().Count > 0 && !OpenWindowOnNew) 
                {
                    temp = AsRecordSource()[Source.Count - 1];
                    AsRecordSource().RemoveAt(Source.Count - 1);
                }
                ExecuteCRUD(ref temp, crud, sql, parameters);
            } 
            else ExecuteCRUD(ref temp, crud, sql, parameters);
            Db.MasterSource?.NotifyChildren(crud, temp);
            return true;
        }

        /// <summary>
        /// Execute a CRUD Operation. This method is only used in <see cref="AlterRecord(string?, List{QueryParameter}?)"/> to avoid code repition.
        /// </summary>
        private void ExecuteCRUD(ref AbstractModel temp, CRUD crud, string? sql = null, List<QueryParameter>? parameters = null) 
        {
            Db.Model = temp;
            Db.Crud(crud, sql, parameters);
            temp.IsDirty = false;
        }
        #endregion
        public override void Dispose()
        {
            SearchQry.Dispose();
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}