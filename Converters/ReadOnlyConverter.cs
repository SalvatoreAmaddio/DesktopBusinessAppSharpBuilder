using System.Globalization;
using System.Windows.Data;
using FrontEnd.Controller;

namespace FrontEnd.Converters
{
    /// <summary>
    /// Class for converting the <see cref="IAbstractFormController.ReadOnly"/> property to a string representation.
    /// This converter is used to display the read-only status as a string in the UI.
    /// </summary>
    /// <remarks>
    /// This converter is declared in the FrontEndDictionary.xaml file located in the Themes folder:
    /// <code>
    /// &lt;converter:ReadOnlyConverter x:Key="ReadOnlyConverter">
    /// </code>
    /// </remarks>
    public class ReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool readOnly)
                return readOnly ? "Read Only" : string.Empty;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
                return string.IsNullOrEmpty(str) ? false : true;
            return value;
        }
    }
}
