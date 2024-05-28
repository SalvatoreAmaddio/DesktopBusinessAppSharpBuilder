using Backend.Controller;
using FrontEnd.Model;
using FrontEnd.Notifier;
using FrontEnd.Source;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Controller
{
    /// <summary>
    /// This interface extends <see cref="IAbstractSQLModelController"/> and adds properties and methods to work as a bridge between <see cref="AbstractModel"/> objects and <see cref="Form"/> objects.
    /// </summary>
    public interface IAbstractFormController : IAbstractSQLModelController, INotifier
    {
        /// <summary>
        /// Gets and sets a reference to a <see cref="Window"/> object that the Controller is associated to. 
        /// When this property is set, the Window's Closing event gets subscribed to the  <see cref="OnWindowClosing(object?, CancelEventArgs)"/> method.
        /// </summary>
        public Window? Window { get; set; }

        /// <summary>
        /// Notify the GUI that a process involving an instance of <see cref="AbstractForm"/> is running.
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Perform an Insert/Update CRUD operation on the <see cref="IAbstractSQLModelController.CurrentModel"/> property.
        /// </summary>
        /// <returns>true if the operation was successful.</returns>
        public bool PerformUpdate();

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
        public void OnWindowClosing(object? sender, CancelEventArgs e);
    }

    /// <summary>
    /// This Interface extends <see cref="IAbstractFormController"/> and adds a set of ICommand properties.
    /// </summary>
    /// <typeparam name="M">An <see cref="AbstractModel"/> object</typeparam>
    public interface IAbstractFormController<M> : IAbstractFormController where M : AbstractModel, new()
    {
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
