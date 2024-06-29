using Backend.Model;
using System.ComponentModel;
using FrontEnd.Forms;
using System.Reflection;
using SelectionChangedEventHandler = FrontEnd.Events.SelectionChangedEventHandler;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// Represents a concrete implementation of the <see cref="IFilterOption"/> interface.
    /// <para/>
    /// This class is designed to work with the <see cref="HeaderFilter"/> GUI Control.
    /// </summary>
    public class FilterOption : IFilterOption
    {
        private bool _isSelected = false;

        #region Properties
        public object? Value { get; private set; }
        public ISQLModel Record { get; private set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
                OnSelectionChanged?.Invoke(this, new EventArgs());
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        public event SelectionChangedEventHandler? OnSelectionChanged;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterOption"/> class with the specified record and display property.
        /// </summary>
        /// <param name="record">The <see cref="ISQLModel"/> associated with this filter option.</param>
        /// <param name="displayProperty">The name of the property used to display the value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is null.</exception>
        public FilterOption(ISQLModel? record, string displayProperty)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            Record = record;
            PropertyInfo Field = Record.GetPropertiesInfo().First(s => s.Name.Equals(displayProperty));
            Value = Field.GetValue(Record);
        }

        public void Select() => IsSelected = true;

        public void Deselect()
        {
            _isSelected = false;
            PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
        }

        public override bool Equals(object? obj) =>
        obj is IFilterOption option && Record.Equals(option.Record);

        public override int GetHashCode() => HashCode.Combine(Record);

        public void Copy(IFilterOption obj)
        {
            Value = obj.Value;
            Record = obj.Record;
        }

        public virtual void Dispose()
        {
            OnSelectionChanged = null;
            PropertyChanged = null;
            GC.SuppressFinalize(this);
        }

        public override string ToString() => $"{Record} - {Value}";

    }
}