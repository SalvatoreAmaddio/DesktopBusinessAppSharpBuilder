using Backend.Model;
using FrontEnd.Events;
using FrontEnd.Notifier;

namespace FrontEnd.Model
{
    /// <summary>
    /// Represents a model interface that extends <see cref="ISQLModel"/> and provides notification functionalities.
    /// </summary>
    public interface IAbstractModel : ISQLModel, INotifier
    {
        /// <summary>
        /// Gets or sets a value indicating if any property marked with <see cref="UpdateProperty{T}(ref T, ref T, string)"/> has changed.
        /// </summary>
        /// <value>True if any property has changed; otherwise, false.</value>
        bool IsDirty { get; set; }

        /// <summary>
        /// Event raised when the <see cref="IsDirty"/> property changes from true to false.
        /// Subscribed and triggered by the <see cref="Forms.SubForm"/> class.
        /// </summary>
        event OnDirtyChangedEventHandler? OnDirtyChanged;

        /// <summary>
        /// Sets the latest changed properties to their previous values.
        /// </summary>
        void Undo();

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property to true.
        /// </summary>
        void Dirt();

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property to false.
        /// </summary>
        void Clean();
    }

}
