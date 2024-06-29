using System.Windows.Controls;
using System.Windows;
using FrontEnd.Forms.FormComponents;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract base class extending <see cref="Control"/>.
    /// <para/>
    /// This class serves as the parent class for <see cref="RecordStatus"/>, <see cref="RecordTracker"/>, and <see cref="HeaderFilter"/>.
    /// </summary>
    public abstract class AbstractControl : Control, IAbstractControl
    {
        public Window? ParentWindow { get; protected set; }

        /// <summary>
        /// Static constructor that overrides the default style key property for instances of <see cref="AbstractControl"/>.
        /// </summary>
        static AbstractControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractControl), new FrameworkPropertyMetadata(typeof(AbstractControl)));

        /// <summary>
        /// Default constructor for <see cref="AbstractControl"/>.
        /// </summary>
        public AbstractControl()
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