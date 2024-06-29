using System.Windows;
using System.Windows.Data;
using System.Globalization;
using FrontEnd.Controller;
using FrontEnd.Utils;
using FrontEnd.Model;

namespace FrontEnd.Forms.FormComponents
{
    /// <summary>
    /// Represents a control that indicates whether the current record, usually an <see cref="IAbstractModel"/>, is being changed.
    /// This control is typically used within an <see cref="AbstractForm"/> object.
    /// </summary>
    public class RecordStatus : AbstractControl
    {
        #region IsDirty
        /// <summary>
        /// Gets or sets a value indicating whether the current record is being changed.
        /// </summary>
        /// <value>True if the record is being changed; otherwise, false.</value>
        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        /// <summary>
        /// Dependency property for binding the <see cref="IsDirty"/> property.
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(RecordStatus), new PropertyMetadata(false, null));
        #endregion

        /// <summary>
        /// Static constructor that overrides the default style key property for instances of <see cref="RecordStatus"/>.
        /// </summary>
        static RecordStatus() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RecordStatus), new FrameworkPropertyMetadata(typeof(RecordStatus)));

        public override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string path = (e.NewValue is IAbstractFormController) ? "CurrentRecord.IsDirty" : nameof(IsDirty);
            SetBinding(IsDirtyProperty,
                new Binding(path)
                {
                    Source = e.NewValue,
                });
        }
    }

    /// <summary>
    /// Converts the boolean value of the <see cref="IsDirty"/> property into a localized string format.
    /// This converter is used in the generic.xaml file located in the Themes folder.
    /// </summary>
    public class IsDirtyConverter : IValueConverter
    {
        private readonly string? _nextStr = Helper.LoadFromStrings("next");
        private readonly string? _editing = Helper.LoadFromStrings("editing");

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? _editing : _nextStr;
            }
            return _nextStr;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? _editing : _nextStr;
            return _nextStr;
        }
    }
}