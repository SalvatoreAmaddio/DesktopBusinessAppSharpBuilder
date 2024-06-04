using System.Windows;
using System.Windows.Data;
using System.Globalization;
using FrontEnd.Controller;
using FrontEnd.Utils;

namespace FrontEnd.Forms.FormComponents
{
    /// <summary>
    /// This class represent a RecordStatus object. This object is used to tell if the current record is being changed or not.
    /// This control is used within a <see cref="AbstractForm"/> object.
    /// </summary>
    public class RecordStatus : AbstractControl
    {
        static RecordStatus() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RecordStatus), new FrameworkPropertyMetadata(typeof(RecordStatus)));

        /// <summary>
        /// Tells if the Current Record is being changed or not.
        /// </summary>
        /// <value>True if the record is being changed.</value>
        public bool IsDirty
        {
            get => (bool)GetValue(IsDirtyProperty);
            set => SetValue(IsDirtyProperty, value);
        }

        /// <summary>
        /// DepedencyProperty for binding the <see cref="IsDirty"/> property.
        /// </summary>
        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(RecordStatus), new PropertyMetadata(false, null));

        protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string path = (e.NewValue is IAbstractFormController) ? "CurrentRecord.IsDirty" : nameof(IsDirty);
            SetBinding(IsDirtyProperty, 
                new Binding(path) 
                { 
                    Source = e.NewValue, 
                });
        }

        ~RecordStatus() { }

    }

    /// <summary>
    /// This class converts the boolean value of the <see cref="IsDirty"/> property into a string format.
    /// This class is used in the generic.xaml in the Themes folder.
    /// </summary>
    public class IsDirtyConverter : IValueConverter
    {
        private string? NextStr = Helper.LoadFromStrings("next");
        private string? editing = Helper.LoadFromStrings("editing");

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? editing : NextStr;
            }
            return NextStr;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? editing : NextStr;
            return NextStr;
        }
    }
}