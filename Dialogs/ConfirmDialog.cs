using System.Windows;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// This class extends <see cref="UnsavedDialog"/> and it is used to ask a user if they want to proceed with a given action or not.
    /// This dialog is usally used to ask a user if they want to delete a record or logout from the system.
    /// </summary>
    public class ConfirmDialog : UnsavedDialog
    {
        protected ConfirmDialog(string? text = null, string? title = null) : base(text, title)
        { }

        static ConfirmDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ConfirmDialog), new FrameworkPropertyMetadata(typeof(ConfirmDialog)));
        public static new DialogResult Ask(string? text = null, string? title = "Confirm") => _ask(new ConfirmDialog(text, title));

    }
}
