using Backend.Events;
using FrontEnd.Model;
using System.ComponentModel;
using System.Windows;
using Backend.Source;
using Backend.Enums;
using Backend.Model;
using FrontEnd.Controller;
using FrontEnd.Source;

namespace FrontEnd.Events
{
    /// <summary>
    /// Occurs just before a window is closed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void WindowClosingEventHandler(object? sender, CancelEventArgs e);

    /// <summary>
    /// Occurs after a window is closed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void WindowClosedEventHandler(object? sender, EventArgs e);

    /// <summary>
    /// Occurs after a window is loaded.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void WindowLoadedEventHandler(object? sender, RoutedEventArgs e);

    /// <summary>
    /// Occurs after filtering in a sub-form.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void AfterSubFormFilterEventHandler(object? sender, EventArgs e);

    /// <summary>
    /// Occurs to notify the parent controller.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void NotifyParentControllerEventHandler(object? sender, EventArgs e);

    /// <summary>
    /// Occurs when the parent record changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Arguments associated with the event.</param>
    public delegate void ParentRecordChangedEventHandler(object? sender, ParentRecordChangedArgs e);

    /// <summary>
    /// Occurs just before preparing a calendar form.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Arguments associated with the event.</param>
    public delegate void OnPreparingCalendarFormEventHandler(object sender, OnPreparingCalendarFormEventArgs e);

    /// <summary>
    /// Occurs when the selection changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void SelectionChangedEventHandler(object? sender, EventArgs e);

    /// <summary>
    /// Occurs after an update's property operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Arguments associated with the event.</param>
    public delegate void AfterUpdateEventHandler(object? sender, AfterUpdateArgs e);

    /// <summary>
    /// Occurs just before an update's property operation.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Arguments associated with the event.</param>
    public delegate void BeforeUpdateEventHandler(object? sender, BeforeUpdateArgs e);

    /// <summary>
    /// Occurs when the <see cref="IAbstractModel.IsDirty"/> property state changes.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Arguments associated with the event.</param>
    public delegate void OnDirtyChangedEventHandler(object? sender, OnDirtyChangedEventArgs e);


    /// <summary>
    /// This delegate works as a bridge between the <see cref="AbstractFormListController{M}"/> and a <see cref="RecordSource{M}"/>.
    /// <para/>
    /// <para/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    /// The <see cref="AbstractFormListController{M}"/> automatically subscribes to this event in its constructor.
    /// If <see cref="AbstractFormListController{M}.OnOptionFilterClicked(FilterEventArgs)"/> has been implemented, The <see cref="RecordSource{M}"/> can trigger them when a CRUD operation occurs.
    /// This delegate is called in the <see cref="IChildSource.Update(CRUD, ISQLModel)"/>
    /// </remarks>
    public delegate void FilterEventHandler(object? sender, FilterEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="OnPreparingCalendarFormEventHandler"/> event.
    /// </summary>
    public class OnPreparingCalendarFormEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the date associated with the calendar form preparation event.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Gets or sets the collection of records associated with the calendar form.
        /// </summary>
        public IEnumerable<IAbstractModel>? Records { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnPreparingCalendarFormEventArgs"/> class
        /// with the specified date, routed event, and event source.
        /// </summary>
        /// <param name="date">The date associated with the event.</param>
        /// <param name="evt">The routed event that occurred.</param>
        /// <param name="source">The object that raised the event.</param>
        public OnPreparingCalendarFormEventArgs(DateTime date, RoutedEvent evt, object source) : base(evt, source) =>
        Date = date;
    }

    /// <summary>
    /// Provides an empty data container for the <see cref="FilterEventHandler"/> event.
    /// </summary>
    public class FilterEventArgs : AbstractEventArgs
    {
        // This class is intentionally left empty.
    }

    /// <summary>
    /// Provides data for the <see cref="OnDirtyChangedEventHandler"/> event.
    /// </summary>
    public class OnDirtyChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="IAbstractModel"/> associated with the event.
        /// </summary>
        public IAbstractModel Model { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnDirtyChangedEventArgs"/> class
        /// with the specified model.
        /// </summary>
        /// <param name="model">The model associated with the event.</param>
        public OnDirtyChangedEventArgs(IAbstractModel model) => Model = model;
    }

    /// <summary>
    /// Provides data for the <see cref="ParentRecordChangedEventHandler"/>.
    /// </summary>
    public class ParentRecordChangedArgs : EventArgs
    {
        /// <summary>
        /// Gets the old value of the parent record.
        /// </summary>
        public IAbstractModel? OldValue { get; }

        /// <summary>
        /// Gets the new value of the parent record.
        /// </summary>
        public IAbstractModel? NewValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParentRecordChangedArgs"/> class
        /// with the specified old and new values.
        /// </summary>
        /// <param name="oldValue">The old value of the parent record.</param>
        /// <param name="newValue">The new value of the parent record.</param>
        public ParentRecordChangedArgs(object? oldValue, object? newValue)
        {
            OldValue = (IAbstractModel?)oldValue;
            NewValue = (IAbstractModel?)newValue;
        }
    }

    /// <summary>
    /// Provides base functionalities for arguments related to <see cref="AfterUpdateEventHandler"/> and <see cref="BeforeUpdateEventHandler"/> events.
    /// </summary>
    public abstract class UpdateArgs(string propertyName) : AbstractEventArgs
    {
        /// <summary>
        /// The name of the property being updated.
        /// </summary>
        public readonly string propertyName = propertyName;

        /// <summary>
        /// Checks if the current property's name is equal to the given value.
        /// </summary>
        /// <param name="value">The name of a property.</param>
        /// <returns>True if the current property name is equal to the given value; otherwise, false.</returns>
        public bool Is(string value) => propertyName.Equals(value);
    }

    /// <summary>
    /// Provides data for the <see cref="AfterUpdateEventHandler"/> event.
    /// </summary>
    public class AfterUpdateArgs(object? value, object? backProperty, string propertyName) : UpdateArgs(propertyName)
    {
        /// <summary>
        /// The new value of the property after the update.
        /// </summary>
        protected object? NewValue = value;

        /// <summary>
        /// The old value of the property before the update.
        /// </summary>
        protected readonly object? OldValue = backProperty;

        /// <summary>
        /// Converts the <see cref="NewValue"/> property to the type <typeparamref name="T"/>. It can return null.
        /// </summary>
        /// <typeparam name="T">The type to convert the new value to.</typeparam>
        /// <returns>The new value converted to the specified type <typeparamref name="T"/>.</returns>
        public T? GetNewValueAs<T>() => (T?)this.NewValue;

        /// <summary>
        /// Converts the <see cref="NewValue"/> property to the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the new value to.</typeparam>
        /// <returns>The new value converted to the specified type <typeparamref name="T"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown if <see cref="NewValue"/> is null.</exception>
        public T ConvertNewValueTo<T>() => (this.NewValue == null) ? throw new NullReferenceException() : (T)this.NewValue;
    }

    /// <summary>
    /// Provides data for the <see cref="BeforeUpdateEventHandler"/> event.
    /// </summary>
    public class BeforeUpdateArgs(object? value, object? backProperty, string propertyName) : AfterUpdateArgs(value, backProperty, propertyName)
    {
        /// <summary>
        /// Gets or sets a value indicating whether the update operation should be canceled.
        /// </summary>
        /// <value>True if the update operation should be canceled; otherwise, false.</value>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Converts the <see cref="OldValue"/> property to the type <typeparamref name="T"/>. It can return null.
        /// </summary>
        /// <typeparam name="T">The type to convert the old value to.</typeparam>
        /// <returns>The old value converted to the specified type <typeparamref name="T"/>.</returns>
        public T? GetOldValueAs<T>() => (T?)this.OldValue;

        /// <summary>
        /// Converts the <see cref="OldValue"/> property to the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the old value to.</typeparam>
        /// <returns>The old value converted to the specified type <typeparamref name="T"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown if <see cref="OldValue"/> is null.</exception>
        public T ConvertOldValueTo<T>() => (this.OldValue == null) ? throw new NullReferenceException() : (T)this.OldValue;

    }
}
