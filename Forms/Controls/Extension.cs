using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Provides attached properties and extension methods for WPF controls.
    /// </summary>
    public static class Extension
    {
        /// <summary>
        /// Identifies the IsWithinList attached dependency property.
        /// This property indicates whether a Button is within a Lista control.
        /// </summary>
        public static readonly DependencyProperty IsWithinListProperty =
        DependencyProperty.RegisterAttached("IsWithinList", typeof(bool), typeof(Extension),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, OnIsWithinListPropertyChanged));

        /// <summary>
        /// Gets the value of the IsWithinList attached property for a given DependencyObject.
        /// </summary>
        /// <param name="obj">The DependencyObject to retrieve the property value from.</param>
        /// <returns>The value of the IsWithinList property.</returns>
        public static bool GetIsWithinList(DependencyObject obj) => (bool)obj.GetValue(IsWithinListProperty);

        /// <summary>
        /// Sets the value of the IsWithinList attached property for a given DependencyObject.
        /// </summary>
        /// <param name="obj">The DependencyObject to set the property value on.</param>
        /// <param name="value">The new value for the IsWithinList property.</param>
        public static void SetIsWithinList(DependencyObject obj, bool value) => obj.SetValue(IsWithinListProperty, value);

        /// <summary>
        /// Handles changes to the IsWithinList property. If the property is set to true, 
        /// binds the Button's DataContext to the DataContext of the nearest Lista ancestor. 
        /// If the property is set to false, clears the binding on the Button's DataContext.
        /// </summary>
        /// <param name="d">The DependencyObject on which the property value has changed.</param>
        /// <param name="e">Event data for the property change.</param>
        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool IsWithinList = (bool)e.NewValue;
            Button btn = (Button)d;

            if (IsWithinList)
            {
                btn.SetBinding(Button.DataContextProperty, new Binding("DataContext")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            }
            else BindingOperations.ClearBinding(btn, Button.DataContextProperty);
        }
    }
}