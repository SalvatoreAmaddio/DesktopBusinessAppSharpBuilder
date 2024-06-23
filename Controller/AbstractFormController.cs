﻿using Backend.Controller;
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
        public bool ReadOnly { get; set; } = false;
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
                    _win.Closed += OnWinClosed;
                    _win.Closing += OnWinClosing;
                    _win.Loaded += OnWinLoaded;
                }
                if (_uiElement is Page _page) 
                {
                    _page.Loaded += OnPageLoaded;
                }
                    
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
        public event BeforeWindowClosingEventHandler? BeforeWindowClosing;
        public event WindowClosingEventHandler? WindowClosing;
        public event WindowClosedEventHandler? WindowClosed;
        public event WindowLoadedEventHandler? WindowLoaded;
        public event AfterSubFormFilterEventHandler? AfterSubFormFilter;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event RecordMovingEventHandler? RecordMovingEvent;
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

        public RecordSource<M> AsRecordSource() => (RecordSource<M>)Source;
        protected override IRecordSource InitSource() => new RecordSource<M>(Db, this);

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

        #region SubForm Methods
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
            if (ReadOnly)
            {
                Failure.Allert("This view is read only","Action Denied");
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                return false;
            }

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
            if (ReadOnly)
            {
                Failure.Allert("This view is read only", "Action Denied");
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                return false;
            }
            DialogResult result = ConfirmDialog.Ask("Are you sure you want to delete this record?");
            if (result == DialogResult.No) return false;
            CurrentRecord = model;
            DeleteRecord();
            NotifyParentControllerEvent?.Invoke(this, EventArgs.Empty);
            return true;
        }
        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; //if the record has not been changed there is nothing to update.
            if (!CurrentRecord.AllowUpdate()) return false; //the record did not meet the criteria to be updated.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            Db.Model = CurrentRecord;
            Db.Crud(crud, sql, parameters);
            CurrentRecord.IsDirty = false;
            Db.MasterSource?.NotifyChildren(crud, Db.Model); //tell children sources to reflect the changes occured in the master source's collection.
            if (crud == CRUD.INSERT) GoLast(); //if the we have inserted a new record instruct the Navigator to move to the last record.
            return true;
        }
        #endregion

        #region GoTo
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
        public override bool GoNew()
        {
            if (!AllowNewRecord) return false;
            if (InvokeOnRecordMovedEvent(RecordMovement.GoNew)) // if the event is cancelled.
                return false;
            if (!CanMove()) return false;
            bool moved = Navigator.MoveNew();
            if (!moved) return false;
            CurrentModel = new M();
            Records = Source.RecordPositionDisplayer();
            return moved;
        }
        public override bool GoFirst()
        {
            bool result = base.GoFirst();
            if (InvokeOnRecordMovedEvent(RecordMovement.GoFirst)) // if the event is cancelled.
                return false;
            return result;
        }
        public override bool GoLast()
        {
            bool result = base.GoLast();
            if (InvokeOnRecordMovedEvent(RecordMovement.GoLast)) // if the event is cancelled.
                return false;
            return result;
        }
        public override bool GoPrevious()
        {
            bool result = base.GoPrevious();
            if (InvokeOnRecordMovedEvent(RecordMovement.GoPrevious)) // if the event is cancelled.
                return false;
            return result;
        }
        public override bool GoNext()
        {
            bool result = base.GoNext();
            if (InvokeOnRecordMovedEvent(RecordMovement.GoNext)) // if the event is cancelled.
                return false;
            return result;
        }
        public override bool GoAt(int index)
        {
            bool result = base.GoAt(index);
            if (InvokeOnRecordMovedEvent(RecordMovement.GoAt)) // if the event is cancelled.
                return false;
            return result;
        }
        #endregion

        #region Event Invokers
        public void InvokeAfterSubFormFilterEvent() => AfterSubFormFilter?.Invoke(this, EventArgs.Empty);
        protected bool InvokeOnRecordMovedEvent(RecordMovement recordMovement)
        {
            AllowRecordMovementArgs args = new(recordMovement);
            RecordMovingEvent?.Invoke(this, args);
            return args.Cancel;
        }
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
        private void OnWinClosed(object? sender, EventArgs e) => WindowClosed?.Invoke(sender, e);
        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            Window? win = Window.GetWindow(UI);
            if (win != null)
            {
                win.Closed += OnWinClosed;
                win.Closing += OnWinClosing;
                win.Loaded += OnWinLoaded;
            }
        }
        private void OnWinLoaded(object sender, RoutedEventArgs e) => WindowLoaded?.Invoke(sender, e);
        public async void OnWinClosing(object? sender, CancelEventArgs e)
        {
            if (ReadOnly)
            {
                CurrentRecord?.Undo();
                CurrentRecord?.Clean();
                Dispose();
                return; // if the Controller is on ReadOnly, there is nothing to check, close the window.
            }

            BeforeWindowClosing?.Invoke(sender, e);

            if (e.Cancel) return;

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

            WindowClosing?.Invoke(sender, e);
        }
        #endregion

        #region Disposers
        public void UnsubscribeWindowClosedEvent()
        {
            if (_uiElement is Window _win)
            {
                _win.Closed -= OnWinClosed;
            }

            if (_uiElement is Page _page)
                _page.Loaded -= OnPageLoaded;
        }
        public override void Dispose()
        {
            BeforeWindowClosing = null;
            WindowClosing = null;
            WindowLoaded = null;
            AfterUpdate = null;
            BeforeUpdate = null;
            RecordMovingEvent = null;
            AfterSubFormFilter = null;

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

            if (_uiElement is Page _page)
                _page.Loaded -= OnPageLoaded;

            AsRecordSource().Dispose(false);
            foreach (ISubFormController subController in _subControllers)
                subController.Dispose();

            _subControllers.Clear();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}