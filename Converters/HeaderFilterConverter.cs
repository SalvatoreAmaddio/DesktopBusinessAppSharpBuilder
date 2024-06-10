using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    public class HeaderFilterConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case bool yesNo:
                    return yesNo ? "Yes" : "No";
                case DateTime date:
                    return date.ToString("dd/MM/yyyy", culture);
                default:
                    return value?.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string str)
                return value;

            if (str.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (str.Equals("No", StringComparison.OrdinalIgnoreCase))
                return false;

            if (DateTime.TryParse(str, culture, DateTimeStyles.None, out DateTime date))
                return date;

            return value;
        }
    }
}
