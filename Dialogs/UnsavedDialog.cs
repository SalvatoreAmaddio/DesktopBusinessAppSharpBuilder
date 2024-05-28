using Backend.Utils;
using FrontEnd.Forms;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// Custom Window Dialog to ask the user what action they would like to perform in case some data are missing.
    /// This dialog is usually called when the user attempt to leave a record without saving it.<para/>
    /// This dialog is used in: <see cref="Lista"/>, <see cref="Controller.IAbstractFormController.OnWindowClosing(object?, System.ComponentModel.CancelEventArgs)"/> 
    /// </summary>
    public class UnsavedDialog : AbstractDialog
    {
        protected Button? yesButton;
        protected Button? noButton;
        static UnsavedDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(UnsavedDialog), new FrameworkPropertyMetadata(typeof(UnsavedDialog)));

        protected UnsavedDialog(string? text = null, string? title = null) : base(text, title)
        { }

        public override Button? ButtonToFocusOn() => yesButton;

        protected virtual void OnNoClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.No;
            DialogResult = false;
        }

        protected virtual void OnYesClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.Yes;
            DialogResult = true;
        }

        public override void OnLoadedTemplate()
        {
            yesButton = (Button?)GetTemplateChild("Yes");
            noButton = (Button?)GetTemplateChild("No");
            if (yesButton == null || noButton == null) throw new Exception("Failed to find the buttons");
            yesButton.Click += OnYesClicked;
            noButton.Click += OnNoClicked;
        }
        public static DialogResult Ask(string? text = null, string? title = "Wait") => _ask(new UnsavedDialog(text, title));

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                if (yesButton!=null)
                    yesButton.Click -= OnYesClicked;
                if (noButton != null)
                    noButton.Click -= OnNoClicked;
            }
        }
    }

}