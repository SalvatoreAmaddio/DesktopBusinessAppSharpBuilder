using FrontEnd.Events;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FrontEnd.Model;
using FrontEnd.Controller;

namespace FrontEnd.Notifier
{
    /// <summary>
    /// Represents an interface that extends <see cref="INotifyPropertyChanged"/> and adds additional UI-related functionalities.
    /// Implemented by <see cref="IAbstractModel"/> and extended by <see cref="IAbstractFormController"/>.
    /// </summary>
    public interface INotifier : INotifyPropertyChanged
    {
        /// <summary>
        /// Updates a property's value and triggers <see cref="RaisePropertyChanged(string)"/>. Sets the <see cref="IAbstractModel.IsDirty"/> property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The new value to set.</param>
        /// <param name="_backProp">The backing field of the property.</param>
        /// <param name="propName">The name of the property.</param>
        public void UpdateProperty<T>(ref T value, ref T _backProp, [CallerMemberName] string propName = "");

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event to notify the UI of property changes.
        /// </summary>
        /// <param name="propName">The name of the property that changed.</param>
        public void RaisePropertyChanged(string propName);

        /// <summary>
        /// Occurs after a property, using <see cref="UpdateProperty{T}(ref T, ref T, string)"/>, has been updated.
        /// </summary>
        public event AfterUpdateEventHandler? AfterUpdate;

        /// <summary>
        /// Occurs before a property, using <see cref="UpdateProperty{T}(ref T, ref T, string)"/>, is updated.
        /// </summary>
        public event BeforeUpdateEventHandler? BeforeUpdate;
    }
}