using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This interface extends <see cref="IAbstractFormController"/> and adds a set of methods and properties to work as a bridge between <see cref="AbstractModel"/> objects and <see cref="FormList"/> objects.
    /// <para/>
    /// see also <seealso cref="RecordTracker"/>, <seealso cref="HeaderFilter"/>
    /// </summary>
    public interface IAbstractFormListController : IAbstractFormController
    {

        /// <summary>
        /// This method is called by the <see cref="Forms.HeaderFilter"/> object when an option is selected or unselected.
        /// It instructs the Controller to filter its RecordSource.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override async void OnOptionFilter()
        /// {
        ///     QueryBuiler.Clear();
        ///     QueryBuiler.AddCondition(GenderOptions.Conditions(QueryBuiler));
        ///     ... // Other conditions if needed
        ///     await SearchRecordAsync();
        /// }
        /// </code>
        /// </summary>
        public void OnOptionFilter();

        /// <summary>
        /// Gets and Sets the Search Query to be used. This property works in conjunction with a <see cref="FilterQueryBuilder"/> object.
        /// <para/>
        /// Your statement must have a WHERE clause.
        /// <para/>
        /// For Example:
        /// <code>
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Payslip WHERE EmployeeID = @ID;";
        /// //OR
        /// public override string SearchQry { get; set; } = $"SELECT * FROM Employee WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// </code>
        /// </summary>
        public string SearchQry { get; set; }

        /// <summary>
        /// Tells if the Controller shall open a Window or add a new row to the <see cref="Lista"/> to add a New Record.<para/>
        /// <c>KEEP IN MIND THAT:</c>
        /// <list type="bullet">
        /// <item>If set to true, the <see cref="VoidParentUpdate"/> property gets set to false.</item>
        /// <item>If set to false, the <see cref="VoidParentUpdate"/> property gets set to true.</item>
        /// </list>
        /// Default value is true.
        /// </summary>
        public bool OpenWindowOnNew { get; set; }

        /// <summary>
        /// Removes empty new records from the Source.
        /// </summary>
        public void CleanSource();
    }

    /// <summary>
    /// This Interface extends <see cref="IAbstractFormController{M}"/> and <see cref="IAbstractFormListController"/> and adds a set of properties necessary to deal with <see cref="FormList"/> objects.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public interface IAbstractFormListController<M> : IAbstractFormController<M>, IAbstractFormListController where M : AbstractModel, new()
    {
        /// <summary>
        /// Gets and Sets the command to execute to open a Record.
        /// </summary>
        public ICommand OpenCMD { get; set; }

        /// <summary>
        /// Gets and Sets the command to execute to open a New Record.
        /// </summary>
        public ICommand OpenNewCMD { get; set; }

        /// <summary>
        /// Gets and Sets the string parameter used in a search textbox to filter the RecordSource.
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Override this method to implement your filter logic. 
        /// For Example:
        /// <code>
        /// //overide SearchQry Property.
        /// public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// ...
        /// public override async Task SearchRecordAsync() 
        /// {
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     QueryBuiler.AddParameter("name", Search.ToLower() + "%");
        ///     var results = await CreateFromAsyncList(QueryBuiler.Query, QueryBuiler.Params);
        ///     Source.ReplaceRange(results);
        ///     GoFirst();
        /// }
        /// </code>
        /// </summary>
        /// <returns>A Taks</returns>
        public Task<IEnumerable<M>> SearchRecordAsync();
    }

}
