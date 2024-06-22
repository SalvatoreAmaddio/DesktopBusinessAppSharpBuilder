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
    /// This class extends <see cref="AbstractSQLModelController"/> and implementats <see cref="IAbstractFormController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormController<M> : AbstractSQLModelController, IParentController, ISubFormController, IDisposable, IAbstractFormController<M> where M : AbstractModel, new()
    {
        #region backing fields
        private string _search = string.Empty;
        private bool _isloading = false;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        private UIElement? _uiElement;
        private readonly List<ISubFormController> _subControllers = [];
        #endregion

        #region Properties
        public AbstractClause SearchQry { get; private set; }

        public UIElement? UI
        {
            get => _uiElement;
            set
            {
                if (value is not Window && value is not Page)
                    throw new Exception("UI Element is meant to be either a Window or a Page");
                _uiElement = value;
                if (_uiElement is Window _win) 
                {
                    _win.Closed += OnWindowClosed;
                    _win.Closing += OnWindowClosing;
                    _win.Loaded += OnWindowLoaded;
                }
                if (_uiElement is Page _page) 
                {
                    _page.Loaded += OnPageLoaded;
                }
                    
            }
        }

        protected virtual void OnWindowClosed(object? sender, EventArgs e)
        {
        }

        protected void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Window? win = Window.GetWindow(UI);
            if (win != null) 
            {
                win.Closed += OnWindowClosed;
                win.Closing += OnWindowClosing;
                win.Loaded += OnWindowLoaded;
            }
        }
        /// <summary>
        /// Wrap up method for the <see cref="RecordSource{M}.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource<M>> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) =>
        RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters).Cast<M>());
        protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e) { }
        public bool AllowAutoSave { get; set; } = false;
        public IEnumerable<M>? MasterSource => DatabaseManager.Find<M>()?.MasterSource.Cast<M>();
        public IAbstractFormController? ParentController { get; set; }
        public AbstractModel? ParentRecord { get; protected set; }
        public override ISQLModel? CurrentModel
        {
            get => _currentModel;
            set
            {
                UpdateProperty(ref value, ref _currentModel);
                RaisePropertyChanged(nameof(CurrentRecord));
            }
        }
        public M? CurrentRecord
        {
            get => (M?)CurrentModel;
            set => CurrentModel = value;
        }
        public override string Records { get => _records; protected set => UpdateProperty(ref value, ref _records); }
        
        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        #endregion

        #region Commands
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public ICommand RequeryCMD { get; set; }
        #endregion

        #region Events
        public event AfterSubFormFilterEventHandler? AfterSubFormFilter;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event NewRecordEventHandler? NewRecordEvent;
        public event NotifyParentControllerEventHandler? NotifyParentControllerEvent;
        #endregion

        public AbstractFormController() : base()
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
            RequeryCMD = new CMDAsync(RequeryAsync);
            SearchQry = InstantiateSearchQry();
        }

        public void ReloadSearchQry()
        {
            SearchQry.Dispose();
            SearchQry = InstantiateSearchQry();
        }

        public virtual AbstractClause InstantiateSearchQry() => new M().From();

        public void RunAfterSubFormFilterEvent() => AfterSubFormFilter?.Invoke(this, EventArgs.Empty);
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
        public RecordSource<M> AsRecordSource()=>(RecordSource<M>)Source;
        protected override IRecordSource InitSource() => new RecordSource<M>(Db,this);

        /// <summary>
        /// It checks if the <see cref="CurrentRecord"/>'s property meets the conditions to be updated. This method is called whenever the <see cref="Navigator"/> moves.
        /// </summary>
        /// <returns>true if the Navigator can move.</returns>
        protected override bool CanMove()
        {
            if (CurrentRecord != null)
            {
                if (CurrentRecord.IsNewRecord() && !CurrentRecord.IsDirty) return true; //the record is a new record and no changes have been made yet.
                if (!CurrentRecord.AllowUpdate()) return false; //the record has changed but it did not met the conditions to be updated.
            }
            return true;
        }

        /// <summary>
        /// This method is called by <see cref="RequeryCMD"/> command to Requery the database table.
        /// It awaits the <see cref="RecordSource.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/> whose result is then used to replace the records kept in the <see cref="IAbstractSQLModelController.Db"/> property and in the <see cref="IAbstractSQLModelController.Source"/> property.
        /// </summary>
        public virtual async Task RequeryAsync()
        {
            IsLoading = true; //notify the GUI that there is a process going on.
            IEnumerable<M>? results = null;
            await Task.Run(async () => //retrieve the records. Do not freeze the GUI.
            {
                results = await RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync().Cast<M>());
            });

            if (results == null) throw new Exception("Source is null"); //Something has gone wrong.
            Db.ReplaceRecords(results.ToList<ISQLModel>()); //Replace the Master RecordSource's records with the newly fetched ones.
            AsRecordSource().ReplaceRecords(results); //Update also its child source for this controller.
            IsLoading = false; //Notify the GUI the process has terminated
        }
        public bool PerformUpdate() => Update(CurrentRecord);

        public Task<bool> PerformUpdateAsync() 
        {
            bool result = Update(CurrentRecord);
            return Task.FromResult(result);
        } 

        /// <summary>
        /// This method is called by <see cref="UpdateCMD"/> command to perform an Update or Insert CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be inserted or updated</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual bool Update(M? model)
        {
            if (model == null) return false;
            CurrentRecord = model;
            bool result = AlterRecord();
            if (result) 
                NotifyParentControllerEvent?.Invoke(this, EventArgs.Empty);
            return result;
        }

        /// <summary>
        /// This method is called by <see cref="DeleteCMD"/> command to perform a Delete CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be deleted</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual bool Delete(M? model)
        {
            DialogResult result = ConfirmDialog.Ask("Are you sure you want to delete this record?");
            if (result == DialogResult.No) return false;
            CurrentRecord = model;
            DeleteRecord();
            NotifyParentControllerEvent?.Invoke(this, EventArgs.Empty);
            return true;
        }
        public override bool GoNew()
        {
            if (!CanMove()) return false;
            bool moved = Navigator.MoveNew();
            if (!moved) return false;
            CurrentModel = new M();
            if (InvokeOnNewRecordEvent()) // if the event is cancelled.
            {
                return false;
            }
            Records = Source.RecordPositionDisplayer();
            return moved;
        }
        protected bool InvokeOnNewRecordEvent() 
        {
            AllowRecordMovementArgs args = new(RecordMovement.GoNew);
            NewRecordEvent?.Invoke(this, args);
            return args.Cancel;
        } 
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
        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentModel == null) throw new NoModelException();
            if (!((AbstractModel)CurrentModel).IsDirty) return false; //if the record has not been changed there is nothing to update.
            CRUD crud = (!CurrentModel.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            if (!CurrentModel.AllowUpdate()) return false; //the record did not meet the criteria to be updated.
            Db.Model = CurrentModel;
            Db.Crud(crud, sql, parameters);
            ((AbstractModel)CurrentModel).IsDirty = false;
            Db.MasterSource?.NotifyChildren(crud, Db.Model); //tell children sources to reflect the changes occured in the master source's collection.
            if (crud == CRUD.INSERT) GoLast(); //if the we have inserted a new record instruct the Navigator to move to the last record.
            return true;
        }
        public void SetParentRecord(AbstractModel? parentRecord)
        {
            ParentRecord = parentRecord;
            OnSubFormFilter();
            if (ParentRecord != null) 
            {
                if (ParentRecord.IsNewRecord())
                    Records = "NO RECORDS";
            }
            else Records = "NO RECORDS";
        }
        public virtual void OnSubFormFilter()
        {
            throw new NotImplementedException("You have not override the OnSubFormFilter() method in the Controller class that handles the SubForm.");
        }
        public virtual async void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            bool dirty = AsRecordSource().Any(s => s.IsDirty);
            if (CurrentRecord!=null) 
                dirty = CurrentRecord.IsDirty;

            if (!dirty) 
            {
                Dispose();
                return; // if the record is not dirty, there is nothing to check, close the window.
            }
            e.Cancel = dirty; 
            
            if (AllowAutoSave) 
            {
                e.Cancel = !PerformUpdate();
                return;
            }

            //the record has been changed, check its integrity before closing.
            DialogResult result = UnsavedDialog.Ask("Do you want to save your changes before closing?");
            if (result == DialogResult.No) //the user has decided to undo its changes to the record. 
            {
                CurrentRecord?.Undo(); //set the record's property to how they were before changing.
                e.Cancel = false; //can close the window.
            }
            else //the user has decided to apply its changes to the record. 
            {
                bool updateResult = PerformUpdate(); //perform an update against the Database.
                e.Cancel = !updateResult; //if the update fails, force the User to stay on the Windwow. If the update was successful, close the window.
            }

            if (!e.Cancel) 
                await Task.Run(Dispose);
        }

        public void DisposeWindow() 
        {
            if (_uiElement is Window _win)
            {
                _win.Closing -= OnWindowClosed;
            }

            if (_uiElement is Page _page)
                _page.Loaded -= OnPageLoaded;
        }

        public override void Dispose()
        {
            if (_uiElement is Window _win) 
            {
                try 
                {
                    _win.Closing -= OnWindowClosing;
                    _win.Loaded -= OnWindowLoaded;
                }
                catch 
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {                    
                        _win.Closing -= OnWindowClosing;
                        _win.Loaded -= OnWindowLoaded;
                    });
                }
            }

            if (_uiElement is Page _page)
                _page.Loaded -= OnPageLoaded;

            AfterUpdate = null;
            BeforeUpdate = null;
            NewRecordEvent = null;
            AfterSubFormFilter = null;
            AsRecordSource().Dispose(false);
            foreach (ISubFormController subController in _subControllers)
                subController.Dispose();
            _subControllers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}