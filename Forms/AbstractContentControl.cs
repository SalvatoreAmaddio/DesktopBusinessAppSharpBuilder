using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    public abstract class AbstractContentControl : ContentControl, IAbstractControl
    {
        public Window? ParentWindow { get; protected set; }

        /// <summary>
        /// Static constructor that overrides the default style key property for instances of <see cref="AbstractContentControl"/>.
        /// </summary>
        static AbstractContentControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractContentControl), new FrameworkPropertyMetadata(typeof(AbstractContentControl)));

        /// <summary>
        /// Default constructor for <see cref="AbstractContentControl"/>.
        /// </summary>
        public AbstractContentControl() 
        {
            DataContextChanged += OnDataContextChanged;
            Unloaded += UnsubscribeEvents;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        #region IAbstractControl
        public virtual void DisposeEvents()
        {
            DataContextChanged -= OnDataContextChanged;
            Unloaded -= UnsubscribeEvents;
            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }

        public virtual void UnsubscribeEvents(object sender, RoutedEventArgs e) => DisposeEvents();
        public virtual void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { }
        public virtual void OnClosed(object? sender, EventArgs e) { }
        #endregion

        /// <summary>
        /// Performs disposal of resources used by the control.
        /// </summary>
        public virtual void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }
}