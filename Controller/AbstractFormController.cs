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

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class extends <see cref="AbstractSQLModelController"/> and implementats <see cref="IAbstractFormController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormController<M> : AbstractSQLModelController, ISubFormController, IDisposable, IAbstractFormController<M> where M : AbstractModel, new()
    {
        string _search = string.Empty;
        bool _isloading = false;
        protected ISQLModel? _currentModel;
        private string _records = string.Empty;
        private Window? _window;
        public Window? Window
        {
            get => _window;
            set
            {
                _window = value;
                if (_window != null)
                    _window.Closing += OnWindowClosing;
            }
        }
        public AbstractModel? ParentRecord { get; private set; }
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
        public override bool AllowNewRecord
        {
            get => _allowNewRecord;
            set
            {
                UpdateProperty(ref value, ref _allowNewRecord);
                base.AllowNewRecord = value;
            }
        }
        public bool IsLoading { get => _isloading; set => UpdateProperty(ref value, ref _isloading); }
        public string Search { get => _search; set => UpdateProperty(ref value, ref _search); }
        public ICommand UpdateCMD { get; set; }
        public ICommand DeleteCMD { get; set; }
        public ICommand RequeryCMD { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event AfterUpdateEventHandler? AfterUpdate;
        public event BeforeUpdateEventHandler? BeforeUpdate;
        public event NewRecordEventHandler? NewRecordEvent;

        public AbstractFormController() : base()
        {
            UpdateCMD = new CMD<M>(Update);
            DeleteCMD = new CMD<M>(Delete);
            RequeryCMD = new CMDAsync(Requery);
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
        protected virtual async Task Requery()
        {
            IsLoading = true; //notify the GUI that there is a process going on.
            IEnumerable<M>? results = null;
            await Task.Run(async () => //retrieve the records. Do not freeze the GUI.
            {
                results = await RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync().Cast<M>());
            });

            if (results == null) throw new Exception("Source is null"); //Something has gone wrong.
            Db.ReplaceRecords(results.ToList<ISQLModel>()); //Replace the Master RecordSource's records with the newly fetched ones.
            AsRecordSource().ReplaceRange(results); //Update also its child source for this controller.
            IsLoading = false; //Notify the GUI the process has terminated
        }

        public bool PerformUpdate() => Update(CurrentRecord);

        /// <summary>
        /// This method is called by <see cref="UpdateCMD"/> command to perform an Update or Insert CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be inserted or updated</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual bool Update(M? model)
        {
            if (model == null) return false;
            CurrentRecord = model;
            return AlterRecord();
        }

        /// <summary>
        /// This method is called by <see cref="DeleteCMD"/> command to perform a Delete CRUD operation.
        /// </summary>
        /// <param name="model">The record that must be deleted</param>
        /// <returns>true if the operation was successful</returns>
        protected virtual void Delete(M? model)
        {
            DialogResult result = ConfirmDialog.Ask("Are you sure you want to delete this record?");
            if (result == DialogResult.No) return;
            CurrentRecord = model;
            DeleteRecord();
        }

        public override bool GoNew()
        {
            if (!CanMove()) return false;
            bool moved = Navigator.MoveNew();
            if (!moved) return false;
            CurrentModel = new M();
            InvokeOnNewRecordEvent();
            Records = Source.RecordPositionDisplayer();
            return moved;
        }

        protected void InvokeOnNewRecordEvent() => NewRecordEvent?.Invoke(this, EventArgs.Empty);

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
            Db.Records?.NotifyChildren(crud, Db.Model); //tell children sources to reflect the changes occured in the master source's collection.
            if (crud == CRUD.INSERT) GoLast(); //if the we have inserted a new record instruct the Navigator to move to the last record.
            return true;
        }

        public void SetParentRecord(AbstractModel? parentRecord)
        {
            ParentRecord = parentRecord;
            OnSubFormFilter();
        }

        public virtual void OnSubFormFilter()
        {
            throw new NotImplementedException("You have not override the OnSubFormFilter() method in the Controller class that handles the SubForm.");
        }

        public void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            bool dirty = AsRecordSource().Any(s => ((M)s).IsDirty);
            e.Cancel = dirty; // if the record is not dirty, there is nothing to check, close the window.

            if (dirty) //the record has been changed, check its integrity before closing.
            {
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
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_window != null)
                    _window.Closing -= OnWindowClosing;
                AfterUpdate = null;
                BeforeUpdate = null;
                NewRecordEvent = null;
            }

            _disposed = true;
        }

        ~AbstractFormController() => Dispose(false);
    }

}
