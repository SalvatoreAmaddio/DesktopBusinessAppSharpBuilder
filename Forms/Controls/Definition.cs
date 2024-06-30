using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class provides shorthand attached properties to set a <see cref="Grid"/>'s Row and Column Definitions.
    /// </summary>
    public static class Definition
    {
        /// <summary>
        /// Identifies the RowDefinitions attached dependency property.
        /// This property allows setting row definitions on a <see cref="Grid"/> using a comma-separated string.
        /// </summary>
        public static readonly DependencyProperty RowDefinitionsProperty =
        DependencyProperty.RegisterAttached("Rows", typeof(string), typeof(Definition),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        /// <summary>
        /// Gets the value of the RowDefinitions attached property for a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to retrieve the property value from.</param>
        /// <returns>The value of the RowDefinitions property.</returns>
        public static string GetRowDefinitions(DependencyObject obj) => (string)obj.GetValue(RowDefinitionsProperty);

        /// <summary>
        /// Sets the value of the RowDefinitions attached property for a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to set the property value on.</param>
        /// <param name="value">The new value for the RowDefinitions property.</param>
        public static void SetRowDefinitions(DependencyObject obj, string value) => obj.SetValue(RowDefinitionsProperty, value);

        /// <summary>
        /// Identifies the ColumnDefinitions attached dependency property.
        /// This property allows setting column definitions on a <see cref="Grid"/> using a comma-separated string.
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionsProperty =
        DependencyProperty.RegisterAttached("Columns", typeof(string), typeof(Definition),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnDefinitionsChanged));

        /// <summary>
        /// Gets the value of the ColumnDefinitions attached property for a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to retrieve the property value from.</param>
        /// <returns>The value of the ColumnDefinitions property.</returns>
        public static string GetColumnDefinitions(DependencyObject obj) => (string)obj.GetValue(ColumnDefinitionsProperty);

        /// <summary>
        /// Sets the value of the ColumnDefinitions attached property for a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> to set the property value on.</param>
        /// <param name="value">The new value for the ColumnDefinitions property.</param>
        public static void SetColumnDefinitions(DependencyObject obj, string value) => obj.SetValue(ColumnDefinitionsProperty, value);

        /// <summary>
        /// Handles changes to the RowDefinitions and ColumnDefinitions properties.
        /// Updates the corresponding <see cref="Grid"/> with the new definitions.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> on which the property value has changed.</param>
        /// <param name="e">Event data for the property change.</param>
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