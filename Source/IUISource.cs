namespace FrontEnd.Source
{
    /// <summary>
    /// Interface for managing interactions between a collection of UI controls and their associated data. This interface is implemented in <see cref="RecordSource{M}"/>
    /// </summary>
    public interface IUISource
    {
        /// <summary>
        /// Notifies all registered UI controls about changes with the provided arguments.
        /// </summary>
        /// <param name="args">Additional arguments to pass to UI controls.</param>
        public void NotifyUIControl(object[] args);

        /// <summary>
        /// Adds a reference to an object implementing <see cref="IUIControl"/> to this UI source.
        /// </summary>
        /// <param name="control">The UI control to add.</param>        
        public void AddUIControlReference(IUIControl control);
    }

}
