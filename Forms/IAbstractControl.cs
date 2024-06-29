using System.Windows;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Interface for UI controls, extending <see cref="Control"/> and implementing <see cref="IDisposable"/>.
    /// </summary>
    public interface IAbstractControl : IDisposable
    {
        /// <summary>
        /// The Window that hosts the control. This property is set in <see cref="FrameworkElement.OnApplyTemplate"/> 
        /// </summary>
        Window? ParentWindow { get; }

        /// <summary>
        /// Event handler for when the control is unloaded from the visual tree.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        void UnsubscribeEvents(object sender, RoutedEventArgs e);

        /// <summary>
        /// Event handler for when <see cref="ParentWindow"/> closes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        void OnClosed(object? sender, EventArgs e);

        /// <summary>
        /// Disposes event subscriptions to prevent memory leaks.
        /// </summary>
        void DisposeEvents();

        /// <summary>
        /// Event handler for changes in the DataContext of the control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e);
    }
}