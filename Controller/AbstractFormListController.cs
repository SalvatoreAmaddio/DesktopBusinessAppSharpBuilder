using Backend.Database;
using Backend.Exceptions;
using Backend.Model;
using FrontEnd.Model;
using FrontEnd.Source;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This class extends <see cref="AbstractFormListController{M}"/> and implements <see cref="IAbstractFormListController{M}"/>
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public abstract class AbstractFormListController<M> : AbstractFormController<M>, IAbstractFormListController<M> where M : AbstractModel, new()
    {
        bool _openWindowOnNew = true;
        protected FilterQueryBuilder QueryBuiler;
        public ICommand OpenCMD { get; set; }
        public ICommand OpenNewCMD { get; set; }
        public abstract string SearchQry { get; set; }
        public bool OpenWindowOnNew
        {
            get => _openWindowOnNew;
            set => _openWindowOnNew = value;
        }

        protected readonly List<QueryParameter> SearchParameters = [];
        public AbstractFormListController() : base()
        {
            OpenCMD = new CMD<M>(Open);
            OpenNewCMD = new CMD(OpenNew);
            QueryBuiler = new(SearchQry);
            AsRecordSource().RunFilter += OnSourceRunFilter;
        }
        public abstract Task<IEnumerable<M>> SearchRecordAsync();
        public abstract void OnOptionFilter();

        /// <summary>
        /// Override this method to open a new window to view the selected record. <para/>
        /// For Example:
        /// <code>
        ///  EmployeeForm win = new (model);
        ///  win.Show();
        /// </code>
        /// </summary>
        /// <param name="model">An <see cref="AbstractModel"/> object which is the record to visualise in the new Window</param>
        protected abstract void Open(M? model);

        /// <summary>
        /// Calls the <see cref="Open(M?)"/> by passing a new instance of <see cref="AbstractModel"/>.
        /// </summary>
        protected void OpenNew() => Open(new());
        private void OnSourceRunFilter(object? sender, EventArgs e) => OnOptionFilter();
        public void CleanSource()
        {
            if (OpenWindowOnNew) return;
            List<M> toRemove = AsRecordSource().Where(s => s.IsNewRecord()).ToList(); //get only the records which are new in the collection.

            foreach (var item in toRemove)
                AsRecordSource().Remove(item); //get rid of them.
        }

        public override bool GoNew()
        {
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
            InvokeOnNewRecordEvent(); //if you are using SubForms, Invoke the the OnNewRecordEvent().
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

        /// <summary>
        /// Wrap up method for the <see cref="RecordSource{M}.CreateFromAsyncList(IAsyncEnumerable{ISQLModel})"/>
        /// </summary>
        /// <param name="qry">The query to be used, can be null</param>
        /// <param name="parameters">A list of parameters to be used, can be null</param>
        /// <returns>A RecordSource</returns>
        public Task<RecordSource<M>> CreateFromAsyncList(string? qry = null, List<QueryParameter>? parameters = null) 
        {
            return RecordSource<M>.CreateFromAsyncList(Db.RetrieveAsync(qry, parameters).Cast<M>());
        } 

        public override bool AlterRecord(string? sql = null, List<QueryParameter>? parameters = null)
        {
            if (CurrentRecord == null) throw new NoModelException();
            if (!CurrentRecord.IsDirty) return false; //if the record has not been changed there is nothing to update.
            if (!CurrentRecord.AllowUpdate()) return false; //the record did not meet the criteria to be updated.
            CRUD crud = (!CurrentRecord.IsNewRecord()) ? CRUD.UPDATE : CRUD.INSERT;
            AbstractModel temp = CurrentRecord;

            if (crud == CRUD.INSERT) 
            {   //INSERT must follow a slighlty different logic to avoid unexpected behaviour between the RecordSource and the Lista object.
                temp = AsRecordSource()[Source.Count - 1];
                AsRecordSource().RemoveAt(Source.Count - 1);
                ExecuteCRUD(ref temp, crud, sql, parameters);
            } 
            else ExecuteCRUD(ref temp, crud, sql, parameters);
            Db.Records?.NotifyChildren(crud, temp);
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                SearchParameters.Clear();
            }
        }

        ~AbstractFormListController() => Dispose(false);

    }
}