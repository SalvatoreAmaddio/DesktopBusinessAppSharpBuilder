using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Dialogs
{
    /// <summary>
    /// Custom Window Dialog to tells the user they have missed some mandatory fields.
    /// This dialog is usually called when the user attempt to save a record without meeting Record's Integrity criteria. <para/>
    /// This dialog is used in: <see cref="Model.AbstractModel.AllowUpdate()"/> 
    /// </summary>
    public class Failure : AbstractDialog
    {
        private Button? okButton;

        static Failure() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Failure), new FrameworkPropertyMetadata(typeof(Failure)));

        private Failure(string? text = null, string? title = null) : base(text, title)
        {
        }

        public override Button? ButtonToFocusOn() => okButton;

        public override void OnLoadedTemplate()
        {
            okButton = (Button?)GetTemplateChild("Okay");
            if (okButton == null) throw new Exception("Failed to find the button");
            okButton.Click += OnOkClicked;
        }

        private void OnOkClicked(object sender, RoutedEventArgs e)
        {
            Result = Dialogs.DialogResult.Ok;
            DialogResult = true;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (okButton != null)
                    okButton.Click -= OnOkClicked;
            }
        }

        public static DialogResult Throw(string? text = null, string? title = "Something is missing") => _ask(new Failure(text, title));

    }
}
