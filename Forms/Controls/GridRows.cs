using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class is a short-hand property to set Grid's Row and Column Definitions.
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
                        if (isRow)
                            grid.RowDefinitions.Add(new RowDefinition() { Height = new(Convert.ToDouble(part)) });
                        else
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new(Convert.ToDouble(part)) });
                    }
                }
            }
        }

    }
}