using System.Globalization;
using System.Windows.Data;
using FrontEnd.Controller;

namespace FrontEnd.Converters
{
    /// <summary>
    /// Class for converting the <see cref="IAbstractFormController.ReadOnly"/> property to string.
    /// </summary>
    public class ReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool readOnly) 
            { 
                return (readOnly) ? "Read Only" : string.Empty;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return string.IsNullOrEmpty(str) ? false : true;
            }
            return value;
        }
    }
}
