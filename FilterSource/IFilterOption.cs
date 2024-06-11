using Backend.Model;
using FrontEnd.Events;
using System.ComponentModel;
using Backend.Database;
using FrontEnd.Forms;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// This interface extends <see cref="INotifyPropertyChanged"/> and defines the properties and methods to be implemented by the <see cref="FilterOption"/> class.
    /// <para/>
    /// This interface works in conjunction with the <see cref="HeaderFilter"/> GUI Control.
    /// </summary>
    public interface IFilterOption : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets and sets a boolean indicating if an option has been selected. This property triggers the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets the ISQLModel that can be selected as option.
        /// </summary>
        public ISQLModel Record { get; }

        /// <summary>
        /// Gets the value of the <see cref="Record"/> property to be displayed.
        /// </summary>
        public object? Value { get; }        
        
        /// <summary>
        /// Occurs when an option is selected or deselected.
        /// </summary>
        public event SelectionChangedEventHandler? OnSelectionChanged;
        
        /// <summary>
        /// Deselects an option bypassing the <see cref="OnSelectionChanged"/> event.
        /// </summary>
        public void Deselect();

        /// <summary>
        /// It copies the <see cref="Value"/> and <see cref="Record"/> properties from the provided argument. This method is called in <see cref="SourceOption.Update(CRUD, ISQLModel)"/>
        /// </summary>
        /// <param name="obj"></param>
        public void Copy(IFilterOption obj);
        
        public void Select();
    }
}