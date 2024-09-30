using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class provides shorthand attached properties to set a <see cref="Grid"/>'s Row and Column Definitions.
    /// </summary>
    public static class Definition
    {
        public static readonly DependencyProperty RowDefinitionsProperty =
        DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(Definition),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        public static string GetRowDefinitions(DependencyObject obj) => (string)obj.GetValue(RowDefinitionsProperty);

        public static void SetRowDefinitions(DependencyObject obj, string value) => obj.SetValue(RowDefinitionsProperty, value);

        public static readonly DependencyProperty ColumnDefinitionsProperty =
        DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(Definition),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        public static string GetColumnDefinitions(DependencyObject obj) => (string)obj.GetValue(ColumnDefinitionsProperty);

        public static void SetColumnDefinitions(DependencyObject obj, string value) => obj.SetValue(ColumnDefinitionsProperty, value);

        private static void OnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid)
            {
                grid.OnApplyTemplate();
                bool isRow = e.Property.Name.Equals("Rows");
                string? definitions = e.NewValue as string;

                if (grid.Name.Equals("listHeader") && !isRow)
                    definitions = $"24,{definitions}";

                if (!string.IsNullOrEmpty(definitions))
                {
                    if (isRow) grid.RowDefinitions.Clear();
                    else grid.ColumnDefinitions.Clear();

                    string[] parts = definitions.Split(',');

                    foreach (string part in parts)
                    {
                        GridLength gridLength = ParseGridLength(part.Trim());

                        if (isRow)
                            grid.RowDefinitions.Add(new RowDefinition() { Height = gridLength });
                        else
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridLength });
                    }
                }
            }
        }

        /// <summary>
        /// Parses a string into a <see cref="GridLength"/>. Handles *, Auto, and fixed sizes.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <returns>A GridLength representing the parsed value.</returns>
        private static GridLength ParseGridLength(string value)
        {
            if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return GridLength.Auto;
            }
            else if (value.EndsWith("*"))
            {
                // Handle star sizing
                string starValue = value.TrimEnd('*');
                if (double.TryParse(starValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return new GridLength(result, GridUnitType.Star);
                }
                else
                {
                    // Default to 1* if no valid number is provided before the '*'
                    return new GridLength(1, GridUnitType.Star);
                }
            }
            else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double absoluteValue))
            {
                // Handle fixed sizes
                return new GridLength(absoluteValue, GridUnitType.Pixel);
            }
            else
            {
                // Fallback to auto if parsing fails
                return GridLength.Auto;
            }
        }
    }
}
