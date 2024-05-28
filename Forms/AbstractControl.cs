using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Forms
{
    public abstract class AbstractControl : Control, IDisposable
    {
        protected bool _disposed = false;
        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        public AbstractControl() => DataContextChanged += OnDataContextChanged;
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

        ~AbstractControl() => Dispose(false);
    }
}
