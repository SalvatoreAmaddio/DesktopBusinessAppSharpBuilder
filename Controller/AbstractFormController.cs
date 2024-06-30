using Backend.Controller;
using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using Backend.Source;
using FrontEnd.Dialogs;
using FrontEnd.Events;
using FrontEnd.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using FrontEnd.Source;
using System.Windows.Controls;
using Backend.Enums;
using Backend.ExtensionMethods;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Extends <see cref="AbstractSQLModelController{M}"/> and implements <see cref="IAbstractFormController{M}"/> to manage form-specific functionality.
    /// </summary>
    /// <typeparam name="M">An <see cref="IAbstractModel"/> object.</typeparam>
    public abstract class AbstractFormController<M> : AbstractSQLModelController<M>, IParentController, ISubFormController, IDisposable, IAbstractFormController<M> where M : IAbstractModel, new()
    {
        #region Backing Fields
        private bool _readOnly = false;
        private M? _currentRecord;
        private string _search = string.Empty;
        private bool _isLoading = false;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        private UIElement? _uiElement;
        private readonly List<ISubFormController> _subControllers = new();
        #endregion

        #region Properties
        public override bool AllowNewRecord
        {
            get => base.AllowNewRecord;
            set
            {
                UpdateProperty(ref value, ref _allowNewRecord);
                Navigator.AllowNewRecord = value;
            }
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set => UpdateProperty(ref value, ref _readOnly);
        }

        public AbstractClause SearchQry { get; private set; }

        public UIElement? UI
        {
            get => _uiElement;
            set
            {
                if (value is not Window && value is not Page)
                    throw new Exception("UI Element is meant to be either a Window or a Page");

                if (value is Page _page)
                    _page.Loaded += OnPageLoaded;

                if (value is Window _win)
                {
                    _win.Closed += OnWinClosed;
                    _win.Closing += OnWinClosing;
                    _win.Loaded += OnWinLoaded;
                }

                _uiElement = value;
            }
        }

        /// <summary>
        /// Wrap-up method for creating a <see cref="RecordSource{M}"/> from an async list.
        /// </summary>
        /// <param name="qry">The query to be used, can be null.</param>
        /// <param name="parameters">A list of parameters to be used, can be null.</param>
        /// <returns>A <see cref="RecordSource{M}"/>.</returns>
        public Task<RecordSource<M>> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) =>
            RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters).Cast<M>());

        public bool AllowAutoSave { get; set; } = false;

        public IEnumerable<M>? MasterSource => DatabaseManager.Find<M>()?.MasterSource.Cast<M>();

        public IAbstractFormController? ParentController { get; set; }

        public IAbstractModel? ParentRecord { get; protected set; }

        public override M? CurrentRecord
        {
            get => _currentRecord;
            set => UpdateProperty(ref value, ref _currentRecord);
        }

        public RecordSource<M> RecordSource => (RecordSource<M>)DataSource;

        public override string Records
        {
            get => _records;
            protected set => UpdateProperty(ref value, ref _records);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => UpdateProperty(ref value, ref _isLoading);
        }

        public string Search
        {
            get => _search;
            set => UpdateProperty(ref value, ref _search);
        }
        #endregion

        #region Commands
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public ICommand RequeryCMD { get; set; }
        #endregion

        #region Events
        public event WindowClosingEventHandler? WindowClosing;
        public event WindowClosedEventHandler? WindowClosed;
        public event WindowLoadedEventHandler? WindowLoaded;
        public event AfterSubFormFilterEventHandler? AfterSubFormFilter;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event NotifyParentControllerEventHandler? NotifyParentController;
        #endregion

        public AbstractFormController() : base()
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
            RequeryCMD = new CMDAsync(RequeryAsync);
            SearchQry = InstantiateSearchQry();
        }

        protected override IDataSource<M> InitSource() => new RecordSource<M>(Db, this);

        /// <summary>
        /// Reloads the search query.
        /// </summary>
        public void ReloadSearchQry()
        {
            SearchQry.Dispose();
            SearchQry = InstantiateSearchQry();
        }

        /// <summary>
        /// Instantiates a search query.
        /// </summary>
        /// <returns>An <see cref="AbstractClause"/> object.</returns>
        public virtual AbstractClause InstantiateSearchQry() => new M().Select().All().From();

        /// <summary>
        /// Requeries the database table. This method is called by the <see cref="RequeryCMD"/> command.
        /// It retrieves the records and replaces the existing ones in the <see cref="Db"/> and <see cref="Source"/> properties.
        /// </summary>
        public virtual async Task RequeryAsync()
        {
            IsLoading = true; // Notify the GUI that a process is ongoing.
            IEnumerable<M>? results = null;
            await Task.Run(async () => // Retrieve the records without freezing the GUI.
            {
                results = await RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync().Cast<M>());
            });

            if (results == null) throw new Exception("Source is null"); // Handle error case.
            Db.ReplaceRecords(results.Cast<ISQLModel>().ToList()); // Replace master record source records.
            RecordSource.ReplaceRecords(results); // Update child source for this controller.
            IsLoading = false; // Notify the GUI that the process has completed.
        }

        #region SubForm Methods
        public void SetParentRecord(IAbstractModel? parentRecord)
        {
            ParentRecord = parentRecord;
            OnSubFormFilter();
            if (ParentRecord != null && ParentRecord.IsNewRecord())
                Records = "NO RECORDS";
            else
                GoFirst();
        }

        public virtual void OnSubFormFilter()
        {
            throw new NotImplementedException("You have not overridden the OnSubFormFilter() method in the Controller class that handles the SubForm.");
        }

        public ISubFormController GetSubController(int index) => _subControllers[index];

        public C? GetSubController<C>(int index) where C : IAbstractFormController => (C?)_subControllers[index];

        public void AddSubControllers(ISubFormController controller)
        {
            controller.ParentController = this;
            _subControllers.Add(controller);
        }

        public void RemoveSubControllers(ISubFormController controller)
        {
            controller.ParentController = null;
            _subControllers.Remove(controller);
        }
        #endregion

        #region Alter Record
        /// <summary>
        /// Performs an update or insert CRUD operation. This method is called by the <see cref="UpdateCMD"/> command.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        public virtual bool PerformUpdate() => Update(CurrentRecord);

        public virtual Task<bool> PerformUpdateAsync()
        {
            bool result = Update(CurrentRecord);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Performs an update or insert CRUD operation.
        /// </summary>
        /// <param name="model">The record to update or insert.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        protected virtual bool Update(M? model)
        {
            if (ReadOnly)
            {
                Failure.Allert("This view is read only", "Action Denied");
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                return false;
            }

            if (model == null) return false;
            _currentRecord = model;
            bool result = AlterRecord();
            if (result)
                NotifyParentController?.Invoke(this, EventArgs.Empty);
            return result;
        }

        /// <summary>
        /// Performs a delete CRUD operation. This method is called by the <see cref="DeleteCMD"/> command.
        /// </summary>
        /// <param name="model">The record to delete.</param>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        protected virtual bool Delete(M? model)
        {
            if (ReadOnly)
            {
                Failure.Allert("This view is read only", "Action Denied");
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                return false;
            }

            if (!AllowDelete(model)) return false;
            DialogResult result = UnsavedDialog.Ask("Are you sure you want to delete this record?");
            if (result == DialogResult.No) return false;

            CurrentRecord = model;
            DeleteRecord();
            NotifyParentController?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public virtual bool AllowDelete(M? model) => true;

        protected override void OnUIApplication(EntityTree tree, ISQLModel record)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                tree.NotifyMasterSourceChildren(record);
            });
        }

        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; // No changes to update.
            if (!CurrentRecord.AllowUpdate()) return false; // Record did not meet update criteria.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Model = CurrentRecord;
            Db.Crud(crud, sql, parameters);
            CurrentRecord.IsDirty = false;
            Db.MasterSource?.NotifyChildren(crud, Db.Model); // Notify children sources of changes.
            if (crud == CRUD.INSERT) GoLast(); // Move to the last record if a new record was inserted.
            return true;
        }
        #endregion

        #region GoTo
        protected override bool CanMove()
        {
            if (CurrentRecord != null)
            {
                if (CurrentRecord.IsNewRecord() && !CurrentRecord.IsDirty) return true; // New record with no changes.
                if (!CurrentRecord.AllowUpdate()) return false; // Record did not meet update criteria.
            }
            return true;
        }
        #endregion

        #region Event Invokers
        public void InvokeAfterSubFormFilterEvent() => AfterSubFormFilter?.Invoke(this, EventArgs.Empty);
        #endregion

        #region Notifier
        public void RaisePropertyChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "")
        {
            BeforeUpdateArgs args = new(value, _backProp, propName);
            BeforeUpdate?.Invoke(this, args);
            if (args.Cancel) return;
            _backProp = value;
            RaisePropertyChanged(propName);
            AfterUpdate?.Invoke(this, args);
        }
        #endregion

        #region Event Subscriptions
        private void OnWinClosed(object? sender, EventArgs e)
        {
            if (UI is Window win)
                win.Closed -= OnWinClosed;
            WindowClosed?.Invoke(sender, e);
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (UI is Page _page)
                _page.Loaded -= OnPageLoaded;

            UI = Window.GetWindow(UI);
            if (UI is Window win && win.IsLoaded)
                OnWinLoaded(win, new());
        }

        private void OnWinLoaded(object sender, RoutedEventArgs e) => WindowLoaded?.Invoke(sender, e);

        public async void OnWinClosing(object? sender, CancelEventArgs e)
        {
            if (ReadOnly)
            {
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                Dispose();
                return; // If the Controller is read-only, close the window without further checks.
            }

            WindowClosing?.Invoke(sender, e);

            if (e.Cancel) return;

            bool dirty = RecordSource.Any(s => s.IsDirty);

            if (CurrentRecord != null)
                dirty = CurrentRecord.IsDirty;

            if (!dirty)
            {
                Dispose();
                return; // If the record is not dirty, close the window.
            }

            e.Cancel = dirty;

            if (AllowAutoSave)
            {
                e.Cancel = !PerformUpdate();
                return;
            }

            // Check record integrity before closing.
            DialogResult result = UnsavedDialog.Ask("Do you want to save your changes before closing?");
            if (result == DialogResult.No)
            {
                CurrentRecord?.Undo();
                e.Cancel = false; // Allow closing the window.
            }
            else
            {
                bool updateResult = PerformUpdate();
                e.Cancel = !updateResult; // Prevent closing if the update fails.
            }

            if (!e.Cancel)
                await Task.Run(Dispose);
        }
        #endregion

        public void SetLoading(bool val) => IsLoading = val;

        #region Disposers
        protected override void DisposeEvents()
        {
            base.DisposeEvents();
            WindowClosing = null;
            WindowLoaded = null;
            AfterUpdate = null;
            BeforeUpdate = null;
            AfterSubFormFilter = null;
        }

        public override void Dispose()
        {
            DisposeEvents();

            if (_uiElement is Window _win)
            {
                try
                {
                    _win.Closing -= OnWinClosing;
                    _win.Loaded -= OnWinLoaded;
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _win.Closing -= OnWinClosing;
                        _win.Loaded -= OnWinLoaded;
                    });
                }
            }

            RecordSource.Dispose(false);
            foreach (ISubFormController subController in _subControllers)
                subController.Dispose();

            _subControllers.Clear();
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}