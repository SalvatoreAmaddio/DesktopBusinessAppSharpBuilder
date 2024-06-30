using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    /// <summary>
    /// Converts <see cref="TimeSpan"/> values to and from their string representation for use in XAML bindings.
    /// <example>
    /// Example usage in your XAML file:
    /// <code>
    /// &lt;fr:Text Grid.Column="4" Text="{Binding TOA, Converter={StaticResource TimeBox}}"/>
    /// </code>
    /// Here, TOA is a <see cref="TimeSpan"/> property.
    /// </example>
    /// </summary>
    /// <remarks>
    /// This converter is declared in the FrontEndDictionary.xaml file located in the Themes folder:
    /// <code>
    /// &lt;converter:StringToTimeConverter x:Key="TimeBox"/>
    /// </code>
    /// </remarks>
    public class StringToTimeConverter : IValueConverter
    {
        /// <summary>
        /// The format string used for displaying <see cref="TimeSpan"/> values.
        /// </summary>
        private const string format = "h:mm tt";
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan time) 
            {
                DateTime date = DateTime.Today.Add(time);
                return date.ToString(format, CultureInfo.InvariantCulture);
            }
            return value;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str.Length == 1)
                    str = $"{str}:00 AM";
                if (DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                {
                    TimeSpan timeSpan = parsedDateTime.TimeOfDay;
                    return timeSpan;
                }
                return value;
            }
            else return value;
        }
    }
}
