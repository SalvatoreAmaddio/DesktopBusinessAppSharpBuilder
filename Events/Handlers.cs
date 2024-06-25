using Backend.Events;
using FrontEnd.Model;
using System.ComponentModel;
using System.Windows;

namespace FrontEnd.Events
{
    public delegate void WindowClosingEventHandler(object? sender, CancelEventArgs e);
    public delegate void WindowClosedEventHandler(object? sender, EventArgs e);
    public delegate void WindowLoadedEventHandler(object? sender, RoutedEventArgs e);
    public delegate void AfterSubFormFilterEventHandler(object? sender, EventArgs e);
    public delegate void NotifyParentControllerEventHandler(object? sender, EventArgs e);
    public delegate void OnPreparingCalendarFormEventHandler(object sender, OnPreparingCalendarFormEventArgs e);
    
    public delegate void ParentRecordChangedEventHandler(object? sender, ParentRecordChangedArgs e);
    public delegate void AfterUpdateEventHandler(object? sender, AfterUpdateArgs e);
    public delegate void BeforeUpdateEventHandler(object? sender, BeforeUpdateArgs e);
    public delegate void SelectionChangedEventHandler(object? sender, EventArgs e);
    public delegate void OnDirtyChangedEventHandler(object? sender, OnDirtyChangedEventArgs e);


    /// <summary>
    /// This delegate works as a bridge between the <see cref="Controller.AbstractFormListController{M}"/> and a <see cref="Source.RecordSource{M}"/>.
    /// <para/>
    /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
    /// <para/>
    /// This delegate is called in the <see cref="Backend.Source.IChildSource.Update(Backend.Database.CRUD, Backend.Model.ISQLModel)"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FilterEventHandler(object? sender, FilterEventArgs e);

    public class OnPreparingCalendarFormEventArgs : RoutedEventArgs
    {
        public IEnumerable<AbstractModel>? Records { get; set; }

        public DateTime Date { get; }

        public OnPreparingCalendarFormEventArgs(DateTime date, RoutedEvent evt, object source)
        {
            Date = date;
            RoutedEvent = evt;
            Source = source;
        }
    }

    public class FilterEventArgs() : AbstractEventArgs
    {
    }

    public class OnDirtyChangedEventArgs(AbstractModel Model) : EventArgs
    {
        public AbstractModel Model { get; } = Model;
    }

    public class ParentRecordChangedArgs(object? oldValue, object? newValue) : EventArgs 
    {
        public AbstractModel? OldValue { get; } = (AbstractModel?)oldValue;
        public AbstractModel? NewValue { get; } = (AbstractModel?)newValue;
    }

    public abstract class UpdateArgs(string propertyName) : AbstractEventArgs
    {
        public readonly string propertyName = propertyName;

        /// <summary>
        /// Check the if current property's name is equal to the given value. 
        /// </summary>
        /// <param name="value">the name of a Property</param>
        /// <returns>bool if the current property name is equal to the given value</returns>
        public bool Is(string value) => propertyName.Equals(value);

    }

    public class AfterUpdateArgs(object? value, object? backProperty, string propertyName) : UpdateArgs(propertyName)
    {
        /// <summary>
        /// The new Property's Value.
        /// </summary>
        protected object? NewValue = value;

        /// <summary>
        /// The Old Property's Value.
        /// </summary>
        protected readonly object? OldValue = backProperty;

        /// <summary>
        /// Converts the <see cref="NewValue"/> property to the type <typeparamref name="T"/>. It can return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetNewValueAs<T>() => (T?)this.NewValue;

        /// <summary>
        /// Converts the <see cref="NewValue"/> property to the type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public T ConvertNewValueTo<T>() => (this.NewValue == null) ? throw new NullReferenceException() : (T)this.NewValue;
    }

    public class BeforeUpdateArgs(object? value, object? backProperty, string propertyName) : AfterUpdateArgs(value, backProperty, propertyName)
    {
        /// <summary>
        /// Gets and sets a boolean to notify if an update can be processed or not.
        /// </summary>
        /// <value>true is the <see cref="AbstractModel"/>'s property can be updated,</value>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// Converts the <see cref="OldValue"/> property to the type <typeparamref name="T"/>. It can return null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? GetOldValueAs<T>() => (T?)this.OldValue;

        /// <summary>
        /// Converts the <see cref="OldValue"/> property to the type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public T ConvertOldValueTo<T>() => (this.OldValue == null) ? throw new NullReferenceException() : (T)this.OldValue;

    }
}
