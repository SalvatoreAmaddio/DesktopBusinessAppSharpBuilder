using Backend.Controller;
using FrontEnd.Model;
using FrontEnd.Notifier;
using FrontEnd.Source;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend.Model;

namespace FrontEnd.Controller
{
    /// <summary>
    /// Defines a set of methods for <see cref="IAbstractFormController"/> objects that act as parent controllers for other controllers.
    /// </summary>
    public interface IParentController
    {
        /// <summary>
        /// Retrieves a <see cref="ISubFormController"/> based on its zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the sub-controller.</param>
        /// <returns>A <see cref="ISubFormController"/> object.</returns>
        ISubFormController GetSubController(int index);

        /// <summary>
        /// Retrieves a <see cref="ISubFormController"/> of type <typeparamref name="C"/> based on its zero-based index.
        /// </summary>
        /// <typeparam name="C">The type of <see cref="IAbstractModel"/> handled by the controller.</typeparam>
        /// <param name="index">The zero-based index of the sub-controller.</param>
        /// <returns>An instance of type <typeparamref name="C"/>.</returns>
        C? GetSubController<C>(int index) where C : IAbstractFormController;

        /// <summary>
        /// Adds a sub-controller to the parent controller.
        /// </summary>
        /// <param name="controller">The sub-controller to add.</param>
        void AddSubControllers(ISubFormController controller);

        /// <summary>
        /// Removes a sub-controller from the parent controller.
        /// </summary>
        /// <param name="controller">The sub-controller to remove.</param>
        void RemoveSubControllers(ISubFormController controller);
    }

    /// <summary>
    /// Extends <see cref="IAbstractSQLModelController"/> and adds properties and methods to bridge between <see cref="IAbstractModel"/> objects and <see cref="Forms.Form"/> objects.
    /// </summary>
    public interface IAbstractFormController : IAbstractSQLModelController, IParentController, INotifier
    {
        /// <summary>
        /// Gets or sets a value indicating whether updates to the record should be saved automatically.
        /// </summary>
        bool AllowAutoSave { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the associated <see cref="Forms.Form"/> object is read-only, preventing CRUD operations.
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets a reference to a <see cref="Window"/> or <see cref="Page"/> object associated with the controller.
        /// If the element is a Window, its Closing event is subscribed to <see cref="OnWinClosing"/>. 
        /// If it is a Page, its Unloaded event is subscribed to ensure <see cref="IDisposable.Dispose"/> is called.
        /// </summary>
        UIElement? UI { get; set; }

        /// <summary>
        /// Notifies the GUI that a process involving an instance of <see cref="AbstractForm"/> is running.
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Defines an <see cref="AbstractClause"/> object.
        /// </summary>
        /// <returns>An instance of <see cref="AbstractClause"/>.</returns>
        AbstractClause InstantiateSearchQry();

        /// <summary>
        /// Calls the <see cref="InstantiateSearchQry"/> method to reload the search query.
        /// </summary>
        void ReloadSearchQry();

        /// <summary>
        /// Performs an insert/update CRUD operation on the <see cref="IAbstractSQLModelController.CurrentModel"/> property.
        /// </summary>
        /// <returns>True if the operation was successful; otherwise, false.</returns>
        bool PerformUpdate();

        /// <summary>
        /// Performs an insert/update CRUD operation on the <see cref="IAbstractSQLModelController.CurrentModel"/> property asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{bool}"/> representing the asynchronous operation.</returns>
        Task<bool> PerformUpdateAsync();

        /// <summary>
        /// Handles record integrity checks before the Window closes.
        /// For example, subscribe the Closing event of your Window and implement it as follows:
        /// <code>
        /// private void Window_Closing(object sender, CancelEventArgs e)
        /// {
        ///     ((EmployeeController) DataContext).OnWindowClosing(e);
        /// }
        /// </code>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        void OnWinClosing(object? sender, CancelEventArgs e);

        /// <summary>
        /// Requeries the <see cref="IAbstractSQLModelController.Source"/> object asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RequeryAsync();

        /// <summary>
        /// Retrieves a <see cref="ISubFormController"/> of type <typeparamref name="C"/> based on its zero-based index.
        /// </summary>
        /// <typeparam name="C">The type of <see cref="IAbstractModel"/> handled by the controller.</typeparam>
        /// <param name="index">The zero-based index of the sub-controller.</param>
        /// <returns>An instance of type <typeparamref name="C"/>.</returns>
        new C? GetSubController<C>(int index) where C : IAbstractFormController;

        /// <summary>
        /// Sets the <see cref="IsLoading"/> property to the specified value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void SetLoading(bool value);
    }

    /// <summary>
    /// Extends <see cref="IAbstractFormController"/> and adds a set of <see cref="ICommand"/> properties.
    /// </summary>
    /// <typeparam name="M">An <see cref="IAbstractModel"/> object.</typeparam>
    public interface IAbstractFormController<M> : IAbstractFormController where M : IAbstractModel, new()
    {
        /// <summary>
        /// Gets the master source as an <see cref="IEnumerable{M}"/>.
        /// </summary>
        IEnumerable<M>? MasterSource { get; }

        /// <summary>
        /// Gets the source object as a <see cref="RecordSource{M}"/>, which handles binding operations for GUI elements.
        /// </summary>
        RecordSource<M> RecordSource { get; }

        /// <summary>
        /// Gets or sets the record on which the <see cref="Navigator"/> is currently pointing.
        /// </summary>
        M? CurrentRecord { get; set; }

        /// <summary>
        /// Gets or sets the command to perform CRUD operations such as insert or update.
        /// </summary>
        ICommand UpdateCMD { get; set; }

        /// <summary>
        /// Gets or sets the command to perform delete CRUD operations.
        /// </summary>
        ICommand DeleteCMD { get; set; }

        /// <summary>
        /// Gets or sets the command to requery the database table.
        /// </summary>
        ICommand RequeryCMD { get; set; }
    }
}