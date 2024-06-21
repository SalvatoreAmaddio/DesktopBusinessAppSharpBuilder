using System.Globalization;
using System.Windows.Data;

namespace FrontEnd.Converters
{
    /// <summary>
    /// Use this converter to deal with TimeSpan.<para/>
    /// For Example in your xaml file:
    /// <code>
    /// &lt;fr:Text Grid.Column="4" Text="{Binding TOA, Converter={StaticResource TimeBox}}"/>
    /// </code>
    /// TOA is a TimeSpan property.
    /// </summary>
    public class StringToTimeConverter : IValueConverter
    {
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
                {
                    str = $"{str}:00 AM";
                }
                DateTime parsedDateTime;
                if (DateTime.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                {
                    TimeSpan timeSpan = parsedDateTime.TimeOfDay;
                    return timeSpan;
                }
                return value;
            }
            else
            {
                return value;
            }
        }


    }

}
