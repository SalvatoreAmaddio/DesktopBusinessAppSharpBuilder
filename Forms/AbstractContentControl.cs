using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractContentControl : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));

        public AbstractContentControl() => DataContextChanged += OnDataContextChanged;

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
            }

            _disposed = true;
        }

        ~AbstractContentControl() => Dispose(false);

    }
}