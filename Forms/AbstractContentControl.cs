using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractContentControl : ContentControl, IDisposable
    {
        protected Window? ParentWindow;
        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));

        public AbstractContentControl() 
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += UnsubscribeEvents;
        }

        protected virtual void UnsubscribeEvents(object sender, RoutedEventArgs e) => DisposeEvents();

        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public virtual void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeEvents()
        {
            // Unsubscribe from events
            DataContextChanged -= OnDataContextChanged;
            Unloaded -= UnsubscribeEvents;
        }

    }
}