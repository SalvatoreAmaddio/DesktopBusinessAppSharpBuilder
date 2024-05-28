using FrontEnd.Model;

namespace FrontEnd.Events
{
    public delegate void NewRecordEventHandler(object? sender, EventArgs e);
    public delegate void ParentRecordChangedEventHandler(object? sender, ParentRecordChangedArgs e);
    public delegate void AfterUpdateEventHandler(object? sender, AfterUpdateArgs e);
    public delegate void BeforeUpdateEventHandler(object? sender, BeforeUpdateArgs e);
    public delegate void SelectionChangedEventHandler(object? sender, EventArgs e);
    public delegate void OnDirtyChangedEventHandler(object? sender, OnDirtyChangedEventArgs e);

    /// <summary>
    /// This delegate works as a bridge between the <see cref="FrontEnd.Controller.AbstractFormListController{M}"/> and a <see cref="FrontEnd.Source.RecordSource{M}"/>.
    /// <para/>
    /// If any filter operations has been implemented in the Controller, The RecordSource can trigger them.
    /// <para/>
    /// This delegate is called in the <see cref="Backend.Source.IChildSource.Update(Backend.Database.CRUD, Backend.Model.ISQLModel)"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void FilterEventHandler(object? sender, EventArgs e);

    public class OnDirtyChangedEventArgs(AbstractModel Model) : EventArgs
    {
        public AbstractModel Model { get; } = Model;
    }

    public class ParentRecordChangedArgs(object? oldValue, object? newValue) : EventArgs 
    {
        public AbstractModel? OldValue { get; } = (AbstractModel?)oldValue;
        public AbstractModel? NewValue { get; } = (AbstractModel?)newValue;
    }

    public abstract class UpdateArgs(string propertyName) : EventArgs
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
        public object? value = value;
        public readonly object? backProperty = backProperty;
    }

    public class BeforeUpdateArgs(object? value, object? backProperty, string propertyName) : AfterUpdateArgs(value, backProperty, propertyName)
    {
        /// <summary>
        /// Gets and sets a boolean to notify if an update can be processed or not.
        /// </summary>
        /// <value>true is the <see cref="AbstractModel"/>'s property can be updated,</value>
        public bool Cancel { get; set; } = false;
    }
}
