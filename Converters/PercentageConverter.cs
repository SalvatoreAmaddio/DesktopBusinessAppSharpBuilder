using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    /// <summary>
    /// Converts a double value to a percentage string and vice versa.
    /// </summary>
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                // Convert double to percentage string
                return doubleValue.ToString("P1", culture);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Remove percentage sign and convert back to double
                if (double.TryParse(stringValue.Replace("%", ""), NumberStyles.Any, culture, out double result))
                {
                    return result / 100;
                }
            }
            return value;
        }
    }
}
