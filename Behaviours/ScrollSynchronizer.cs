using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Behaviours
{
    /// <summary>
    /// Provides attached properties to synchronize the horizontal scrolling of two <see cref="ScrollViewer"/> controls.
    /// This behavior is used in the <c>Lista</c> object to keep two <see cref="ScrollViewer"/> instances in sync.
    /// </summary>
    public static class ScrollSynchronizer
    {
        /// <summary>
        /// Identifies the <see cref="SynchronizeWithProperty"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty SynchronizeWithProperty =
        DependencyProperty.RegisterAttached("SynchronizeWith", typeof(ScrollViewer), typeof(ScrollSynchronizer), new PropertyMetadata(null, OnSynchronizeWithChanged));

        /// <summary>
        /// Gets the <see cref="ScrollViewer"/> with which the specified <see cref="ScrollViewer"/> is synchronized.
        /// </summary>
        /// <param name="obj">The dependency object from which to get the property value.</param>
        /// <returns>The <see cref="ScrollViewer"/> with which synchronization is set.</returns>
        public static ScrollViewer GetSynchronizeWith(DependencyObject obj) =>
        (ScrollViewer)obj.GetValue(SynchronizeWithProperty);

        /// <summary>
        /// Sets the <see cref="ScrollViewer"/> with which the specified <see cref="ScrollViewer"/> should be synchronized.
        /// </summary>
        /// <param name="obj">The dependency object on which to set the property value.</param>
        /// <param name="value">The <see cref="ScrollViewer"/> with which synchronization should be set.</param>
        public static void SetSynchronizeWith(DependencyObject obj, ScrollViewer value) =>
        obj.SetValue(SynchronizeWithProperty, value);

        /// <summary>
        /// Handles changes to the <see cref="SynchronizeWithProperty"/> attached property.
        /// </summary>
        /// <param name="d">The dependency object on which the property value changed.</param>
        /// <param name="e">Event arguments containing information about the property change.</param>
        private static void OnSynchronizeWithChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer && e.NewValue is ScrollViewer targetScrollViewer)
            {
                scrollViewer.ScrollChanged += (sender, args) =>
                {
                    if (args.HorizontalChange != 0)
                    {
                        targetScrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset);
                    }
                };

                targetScrollViewer.ScrollChanged += (sender, args) =>
                {
                    if (args.HorizontalChange != 0)
                    {
                        scrollViewer.ScrollToHorizontalOffset(targetScrollViewer.HorizontalOffset);
                    }
                };
            }
        }
    }
}