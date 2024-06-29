using FrontEnd.Forms;

namespace FrontEnd.Source
{
    /// <summary>
    /// Interface that serves as a bridge between an <see cref="IUISource"/> object and a <see cref="Combo"/> object in GUI development.
    /// The primary purpose is to update the <see cref="Combo"/> control to reflect changes in its ItemSource, which is an instance of <see cref="IUISource"/>.
    /// </summary>
    public interface IUIControl
    {
        /// <summary>
        /// Notifies the control to update itself when the parent record source is updated.
        /// </summary>
        /// <param name="args">Additional arguments passed during the update.</param>
        public void OnItemSourceUpdated(object[] args);
    }
}
