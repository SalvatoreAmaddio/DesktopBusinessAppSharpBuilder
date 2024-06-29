using Backend.Events;
using FrontEnd.Events;
using FrontEnd.Model;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Defines a set of methods and properties to act as a bridge between instances of <see cref="IAbstractFormController"/>, 
    /// <see cref="IAbstractFormListController"/>, and <see cref="SubForm"/> objects.
    /// </summary>
    public interface ISubFormController
    {
        /// <summary>
        /// Event triggered after the subform filter is applied.
        /// </summary>
        public event AfterSubFormFilterEventHandler? AfterSubFormFilter;

        /// <summary>
        /// Invokes the <see cref="AfterSubFormFilter"/> event.
        /// </summary>
        public void InvokeAfterSubFormFilterEvent();

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IAbstractFormController? ParentController { get; set; }

        /// <summary>
        /// Holds a reference to the SubForm's ParentRecord property. 
        /// This property is set by the <see cref="SetParentRecord(IAbstractModel?)"/> method called within the SubForm object.
        /// </summary>
        public IAbstractModel? ParentRecord { get; }

        /// <summary>
        /// Notifies the subform that its ParentController has moved to another record.
        /// </summary>
        /// <param name="ParentRecord">The new parent record.</param>
        public void SetParentRecord(IAbstractModel? ParentRecord);

        /// <summary>
        /// Event triggered when the SubForm is going to add a new record.
        /// <para/>
        /// Example usage:
        /// <code>
        /// public YourSubFormController() => AfterRecordNavigation += OnNewRecordEvent;
        /// ...
        /// private void OnNewRecordEvent(object? sender, EventArgs e) 
        /// {
        ///      Employee? employee = (Employee?)ParentRecord;
        ///      if (employee != null) 
        ///      {
        ///           CurrentRecord.Employee = new(employee.EmployeeID);
        ///           CurrentRecord.IsDirty = false;
        ///      }
        /// }
        /// </code>
        /// </summary>
        public event AfterRecordNavigationEventHandler? AfterRecordNavigation;

        /// <summary>
        /// Event triggered to notify the parent controller.
        /// </summary>
        public event NotifyParentControllerEventHandler? NotifyParentController;

        /// <summary>
        /// Override this method to implement custom logic to filter a SubForm object.
        /// <para/>
        /// Example usage:
        /// <code>
        /// ...
        ///ReloadSearchQry();
        ///SearchQry.AddParameter("patientID", ParentRecord?.GetPrimaryKey()?.GetValue());
        ///RecordSource<Treatment> results = await CreateFromAsyncList(SearchQry.Statement(), SearchQry.Params());
        ///RecordSource.ReplaceRange(results);
        ///GoFirst();
        /// ...
        /// </code>
        /// </summary>
        public void OnSubFormFilter();

        /// <summary>
        /// Disposes the controller, releasing any resources it holds.
        /// </summary>
        public void Dispose();
    }
}