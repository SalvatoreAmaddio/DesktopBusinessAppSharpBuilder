using Backend.Model;
using FrontEnd.Events;
using System.ComponentModel;
using FrontEnd.Forms;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// Defines properties and methods required by options in a filter control, extending <see cref="INotifyPropertyChanged"/> and <see cref="IDisposable"/>.
    /// <para/>
    /// This interface is used in conjunction with the <see cref="HeaderFilter"/> GUI control.
    /// </summary>
    public interface IFilterOption : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets or sets a boolean indicating whether the option is selected. 
        /// Changing this property triggers the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the underlying <see cref="ISQLModel"/> associated with the filter option.
        /// </summary>
        ISQLModel Record { get; }

        /// <summary>
        /// Gets the value of the <see cref="Record"/> property to be displayed.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// Occurs when the selection state of the filter option changes.
        /// </summary>
        event SelectionChangedEventHandler? OnSelectionChanged;

        /// <summary>
        /// Deselects the option without triggering the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        void Deselect();

        /// <summary>
        /// Copies the <see cref="Value"/> and <see cref="Record"/> properties from another <see cref="IFilterOption"/> instance.
        /// This method is typically called in <see cref="SourceOption.Update(CRUD, ISQLModel)"/>.
        /// </summary>
        /// <param name="obj">The <see cref="IFilterOption"/> instance to copy from.</param>
        void Copy(IFilterOption obj);

        /// <summary>
        /// Selects the option by setting the <see cref="IsSelected"/> property to true.
        /// </summary>
        void Select();
    }
}