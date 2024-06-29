using Backend.Model;
using Backend.Controller;
using FrontEnd.Events;
using FrontEnd.Model;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Extends <see cref="IAbstractFormController"/> and adds a set of methods and properties to bridge between <see cref="IAbstractModel"/> objects and <see cref="Forms.FormList"/> objects.
    /// </summary>
    public interface IAbstractFormListController : IAbstractFormController
    {
        /// <summary>
        /// Called by the <see cref="Forms.HeaderFilter"/> object when an option is selected or unselected.
        /// Instructs the controller to filter its RecordSource.
        /// <para/>
        /// For example:
        /// <code>
        /// public override async void OnOptionFilter()
        /// {
        ///     ReloadSearchQry();
        ///     GenderOptions.Conditions&lt;WhereClause>(SearchQry);
        ///     TitleOptions.Conditions&lt;WhereClause>(SearchQry);
        ///     // Other conditions if needed
        ///     await SearchRecordAsync(); //OR// OnAfterUpdate(e, new(null, null, nameof(Search)));
        /// }
        /// </code>
        /// </summary>
        /// <param name="e">The filter event arguments.</param>
        void OnOptionFilterClicked(FilterEventArgs e);

        /// <summary>
        /// Gets or sets the <see cref="AbstractClause"/> to be used. This property is set in the constructor through the <see cref="IAbstractFormController.InstantiateSearchQry"/>.
        /// </summary>
        AbstractClause SearchQry { get; }

        /// <summary>
        /// Indicates whether the controller should open a window or add a new row to the <see cref="Lista"/> to add a new record.
        /// <para/>
        /// <c>KEEP IN MIND THAT:</c>
        /// <list type="bullet">
        /// <item>If set to true, the <see cref="VoidParentUpdate"/> property gets set to false.</item>
        /// <item>If set to false, the <see cref="VoidParentUpdate"/> property gets set to true.</item>
        /// </list>
        /// Default value is true.
        /// </summary>
        bool OpenWindowOnNew { get; set; }

        /// <summary>
        /// Removes empty new records from the <see cref="IAbstractSQLModelController.Source"/>.
        /// </summary>
        void CleanSource();
    }

    /// <summary>
    /// Extends <see cref="IAbstractFormController{M}"/> and <see cref="IAbstractFormListController"/> and adds a set of properties necessary to deal with <see cref="FormList"/> objects.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object.</typeparam>
    public interface IAbstractFormListController<M> : IAbstractFormController<M>, IAbstractFormListController where M : IAbstractModel, new()
    {
        /// <summary>
        /// Gets or sets the command to execute to open a record.
        /// </summary>
        ICommand OpenCMD { get; set; }

        /// <summary>
        /// Gets or sets the command to execute to open a new record.
        /// </summary>
        ICommand OpenNewCMD { get; set; }

        /// <summary>
        /// Gets or sets the string parameter used in a search textbox to filter the RecordSource.
        /// </summary>
        string Search { get; set; }

        /// <summary>
        /// Override this method to implement your filter logic.
        /// For example:
        /// <code>
        /// // Override SearchQry property.
        /// public override string SearchQry { get; set; } = $"SELECT * FROM {nameof(Employee)} WHERE (LOWER(FirstName) LIKE @name OR LOWER(LastName) LIKE @name)";
        /// ...
        /// public override async Task SearchRecordAsync() 
        /// {
        ///     SearchQry.AddParameter("name", Search.ToLower() + "%");
        ///     return await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        /// }
        /// </code>
        /// </summary>
        /// <returns>A task representing the asynchronous operation, containing the search results.</returns>
        Task<IEnumerable<M>> SearchRecordAsync();
    }
}
