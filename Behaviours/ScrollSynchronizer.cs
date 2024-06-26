using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Behaviours
{
    public static class ScrollSynchronizer
    {
        public static readonly DependencyProperty SynchronizeWithProperty =
            DependencyProperty.RegisterAttached("SynchronizeWith", typeof(ScrollViewer), typeof(ScrollSynchronizer), new PropertyMetadata(null, OnSynchronizeWithChanged));

        public static ScrollViewer GetSynchronizeWith(DependencyObject obj)
        {
            return (ScrollViewer)obj.GetValue(SynchronizeWithProperty);
        }

        public static void SetSynchronizeWith(DependencyObject obj, ScrollViewer value)
        {
            obj.SetValue(SynchronizeWithProperty, value);
        }

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
