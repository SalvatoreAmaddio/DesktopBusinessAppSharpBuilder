using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractContentControl : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));

        public AbstractContentControl() 
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += UnsubscribeEvents;
        }

        protected virtual void UnsubscribeEvents(object sender, RoutedEventArgs e) => Dispose(true);

        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Unsubscribe from events
                DataContextChanged -= OnDataContextChanged;
                Unloaded -= UnsubscribeEvents;
            }

            _disposed = true;
        }

    }
}