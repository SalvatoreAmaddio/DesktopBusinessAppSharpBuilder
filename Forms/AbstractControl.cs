using System.Windows.Controls;
using System.Windows;
using FrontEnd.Utils;

namespace FrontEnd.Forms
{
    public abstract class AbstractControl : Control, IDisposable
    {
        protected bool _disposed = false;
        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        public AbstractControl() 
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += UnsubscribeEvents;
        }
        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }

        protected virtual void UnsubscribeEvents(object sender, RoutedEventArgs e) => Dispose(true);

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
                DataContextChanged -= OnDataContextChanged;
                Unloaded -= UnsubscribeEvents;
            }

            _disposed = true;
        }

        ~AbstractControl() => Dispose(false);
    }
}
