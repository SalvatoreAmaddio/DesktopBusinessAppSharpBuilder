using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FrontEnd.Forms
{
    public static class Definition
    {
        public static readonly DependencyProperty RowDefinitionsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(Definition),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        public static string GetRowDefinitions(DependencyObject obj) => (string)obj.GetValue(RowDefinitionsProperty);

        public static void SetRowDefinitions(DependencyObject obj, string value) => obj.SetValue(RowDefinitionsProperty, value);

        public static readonly DependencyProperty ColumnDefinitionsProperty =
            DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(Definition),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange |
                                                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                                                    FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        public static string GetColumnDefinitions(DependencyObject obj) => (string)obj.GetValue(ColumnDefinitionsProperty);

        public static void SetColumnDefinitions(DependencyObject obj, string value) => obj.SetValue(ColumnDefinitionsProperty, value);

        private static void OnDefinitionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grid)
            {
                // Defer the operation so it happens after the binding has resolved
                grid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    bool isRow = e.Property == RowDefinitionsProperty;
                    string definitions = (string)e.NewValue;

                    UpdateDefinitions(grid, isRow, definitions);
                }));
            }
        }

        private static void UpdateDefinitions(Grid grid, bool isRow, string definitions)
        {
            if (!string.IsNullOrEmpty(definitions))
            {
                if (isRow)
                {
                    grid.RowDefinitions.Clear();
                }
                else
                {
                    grid.ColumnDefinitions.Clear();
                }

                string[] parts = definitions.Split(',');

                foreach (string part in parts)
                {
                    GridLength gridLength = ParseGridLength(part.Trim());

                    if (isRow)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = gridLength });
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = gridLength });
                    }
                }
            }
        }

        private static GridLength ParseGridLength(string value)
        {
            if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return GridLength.Auto;
            }
            else if (value.EndsWith("*"))
            {
                string starValue = value.TrimEnd('*');
                if (double.TryParse(starValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return new GridLength(result, GridUnitType.Star);
                }
                else
                {
                    return new GridLength(1, GridUnitType.Star);
                }
            }
            else if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double absoluteValue))
            {
                return new GridLength(absoluteValue, GridUnitType.Pixel);
            }
            else
            {
                return GridLength.Auto;
            }
        }
    }
}
