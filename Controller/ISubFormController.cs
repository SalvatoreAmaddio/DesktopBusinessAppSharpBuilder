using FrontEnd.Events;
using FrontEnd.Model;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This interface defines a set of methods and properties to work as a bridge between an instances of <see cref="IAbstractFormController"/>, <see cref="IAbstractFormListController"/>  and <see cref="SubForm"/> objects.
    /// </summary>
    public interface ISubFormController
    {
        /// <summary>
        /// Holds a reference to the SubForm's ParentRecord property. This property is set by the <see cref="SetParentRecord(AbstractModel?)"/>  called within the SubForm object.
        /// </summary>
        public AbstractModel? ParentRecord { get; }

        /// <summary>
        /// This method is called by the <see cref="SubForm"/> class to notify that its ParentController has moved to another Record.
        /// </summary>
        /// <param name="ParentRecord"></param>
        public void SetParentRecord(AbstractModel? ParentRecord);

        /// <summary>
        /// Occurs when the SubForm is going to add a new Record.
        /// <para/>
        /// For Example:
        /// <code>
        /// public YourSubFormController() => NewRecordEvent += OnNewRecordEvent;
        /// ...
        /// private void OnNewRecordEvent(object? sender, EventArgs e) 
        /// {
        ///      Employee? employee = (Employee?)ParentRecord;
        ///      if (employee!=null) 
        ///      {
        ///           CurrentRecord.Employee = new(employee.EmployeeID);
        ///           CurrentRecord.IsDirty = false;
        ///      }
        /// }
        /// </code>
        /// </summary>
        public event NewRecordEventHandler? NewRecordEvent;

        /// <summary>
        /// Override this method to implement a custom logic to filter a SubForm object.
        /// <para/>
        /// For Example:
        /// <code>
        /// ...
        /// string sql = $"SELECT * FROM YourTable WHERE YourForeignKey = @foreignKey;";
        /// List&lt;QueryParameter> queryParameters = [];
        /// queryParameters.Add(new ("employeeID", ParentRecord?.GetTablePK()?.GetValue()));
        /// var results = await CreateFromAsyncList(sql, queryParameters);
        /// Source.ReplaceRange(results);
        /// GoFirst();
        /// ...
        /// </code>
        /// </summary>
        public void OnSubFormFilter();
    }

}
