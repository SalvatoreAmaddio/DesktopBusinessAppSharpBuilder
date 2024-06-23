using Backend.Controller;
using FrontEnd.Model;
using FrontEnd.Notifier;
using FrontEnd.Source;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend.Database;
using Backend.Model;

namespace FrontEnd.Controller
{
    public interface IParentController 
    {
        public ISubFormController GetSubController(int index);

        public void AddSubControllers(ISubFormController controller);

        public void RemoveSubControllers(ISubFormController controller);
    }

    /// <summary>
    /// This interface extends <see cref="IAbstractSQLModelController"/> and adds properties and methods to work as a bridge between <see cref="AbstractModel"/> objects and <see cref="Form"/> objects.
    /// </summary>
    public interface IAbstractFormController : IAbstractSQLModelController, IParentController, INotifier
    {
        /// <summary>
        /// Gets and sets a reference to a <see cref="Window"/> or <see cref="Page"/> object that the Controller is associated to. <para/>
        /// If the element is a Window, its Closing event gets subscribed to the  <see cref="OnWinClosing(object?, CancelEventArgs)"/>.<para/>
        /// Whereas, a Page gets its Unloaded event subscribed. Both subscriptions aim at calling <see cref="IDisposable.Dispose"/> 
        /// </summary>
        public UIElement? UI { get; set; }

        /// <summary>
        /// Notify the GUI that a process involving an instance of <see cref="AbstractForm"/> is running.
        /// </summary>
        public bool IsLoading { get; set; }
        public AbstractClause InstantiateSearchQry();
        public void ReloadSearchQry();

        /// <summary>
        /// Perform an Insert/Update CRUD operation on the <see cref="IAbstractSQLModelController.CurrentModel"/> property.
        /// </summary>
        /// <returns>true if the operation was successful.</returns>
        public bool PerformUpdate();
        public Task<bool> PerformUpdateAsync();

        /// <summary>
        /// Handles record's integrity checks before the Window closes. <para/>
        /// For Example; Subscribe the Closing event of your Window and implement it as follow:
        /// <code>
        ///private void Window_Closing(object sender, CancelEventArgs e)
        ///{
        ///    ((EmployeeController) DataContext).OnWindowClosing(e);
        ///}
        /// </code>
        /// </summary>
        public void OnWinClosing(object? sender, CancelEventArgs e);

        public bool AllowAutoSave { get; set; }

        public Task RequeryAsync();

        public C? GetSubController<C>(int index) where C : IAbstractFormController;

    }

    /// <summary>
    /// This Interface extends <see cref="IAbstractFormController"/> and adds a set of ICommand properties.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public interface IAbstractFormController<M> : IAbstractFormController where M : AbstractModel, new()
    {
        /// <summary>
        /// Gets the <see cref="IAbstractDatabase.MasterSource"/> as an <see cref="IEnumerable{T}"/> 
        /// </summary>
        public IEnumerable<M>? MasterSource { get; }

        /// <summary>
        /// A more concrete version of <see cref="IAbstractSQLModelController.CurrentModel"/>
        /// </summary>
        /// <value>The actual object that implements <see cref="IAbstractSQLModelController.CurrentModel"/></value>
        public M? CurrentRecord { get; set; }

        /// <summary>
        /// Gets and Sets the Command to perform CRUD operations such as Insert or Update.
        /// </summary>
        public ICommand UpdateCMD { get; set; }

        /// <summary>
        /// Gets and Sets the Command to perform Delete CRUD operation.
        /// </summary>
        public ICommand DeleteCMD { get; set; }

        /// <summary>
        /// Gets and Sets the Command to requery the database table.
        /// </summary>
        public ICommand RequeryCMD { get; set; }

        public RecordSource<M> AsRecordSource();

    }
}
