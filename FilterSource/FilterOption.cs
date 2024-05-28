using Backend.Model;
using FrontEnd.Events;
using System.ComponentModel;
using FrontEnd.Forms;

namespace FrontEnd.FilterSource
{
    /// <summary>
    /// Concrete impementation of the <see cref="IFilterOption"/>
    /// <para/>
    /// This class works in conjunction with the <see cref="HeaderFilter"/> GUI Control.
    /// </summary>
    public class FilterOption : IFilterOption, IDisposable
    {
        protected bool _disposed = false;
        private bool _isSelected = false;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        public event SelectionChangedEventHandler? OnSelectionChanged;

        public FilterOption(ISQLModel record, string displayProperty)
        {
            Record = record;
            ITableField Field = Record.GetTableFields().First(s => s.Name.Equals(displayProperty));
            Value = Field.GetValue();
        }

        public void Deselect()
        {
            _isSelected = false;
            PropertyChanged?.Invoke(this, new(nameof(IsSelected)));
        }

        public override bool Equals(object? obj) =>
        obj is FilterOption option && Record.Equals(option.Record);

        public override int GetHashCode() => HashCode.Combine(Record);

        public void Copy(IFilterOption obj)
        {
            Value = obj.Value;
            Record = obj.Record;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                OnSelectionChanged = null;
                PropertyChanged = null;
            }

            _disposed = true;
        }

        ~FilterOption() => Dispose(false);
    }
}