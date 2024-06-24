using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Forms
{
    public abstract class AbstractControl : Control, IDisposable
    {
        protected Window? ParentWindow;
        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        public AbstractControl() 
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += UnsubscribeEvents;
        }
        protected virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        { }

        protected virtual void UnsubscribeEvents(object sender, RoutedEventArgs e) => DisposeEvents();

        public virtual void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeEvents()
        {
            DataContextChanged -= OnDataContextChanged;
            Unloaded -= UnsubscribeEvents;
        }

    }
}
