using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool yesNo) return value;
            return (yesNo) ? "Yes" : "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str) return value;
            return (str.Equals("Yes")) ? true : false;
        }
    }
}
