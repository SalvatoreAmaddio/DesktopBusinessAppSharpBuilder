using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    public class HeaderFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool yesNo) 
            {
                return (yesNo) ? "Yes" : "No";
            }
            if (value is DateTime date)
                return date.ToString("dd/MM/yyyy");
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str) return value;
            if (str.Equals("Yes")) return true;
            if (str.Equals("No")) return false;
            return DateTime.Parse(str);
        }
    }
}
