using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Dialogs
{
    public class SuccessDialog : AbstractDialog
    {
        Button? PART_OK;
        static SuccessDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SuccessDialog), new FrameworkPropertyMetadata(typeof(SuccessDialog)));

        public override Button? ButtonToFocusOn() => PART_OK;
        public override void OnLoadedTemplate()
        {
            PART_OK = (Button?)GetTemplateChild(nameof(PART_OK));
            if (PART_OK == null) throw new Exception("Failed to find the button");
            PART_OK.Click += OnOkClicked;
        }

        private void OnOkClicked(object sender, RoutedEventArgs e) => Close();

        protected SuccessDialog(string? text = null, string? title = null) : base(text, title)
        { }

        public static DialogResult Display(string? text = null, string? title = "Done!") => _ask(new SuccessDialog(text, title));

    }

}
