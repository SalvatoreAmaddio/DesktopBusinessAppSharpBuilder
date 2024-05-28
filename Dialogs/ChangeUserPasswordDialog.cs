using Backend.Utils;
using System.Windows.Controls;
using System.Windows;

namespace FrontEnd.Dialogs
{
    public class ChangeUserPasswordDialog : Window
    {
        PasswordBox? PART_OldPassword;
        PasswordBox? PART_NewPassword;
        Button? PART_Change;
        static ChangeUserPasswordDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ChangeUserPasswordDialog), new FrameworkPropertyMetadata(typeof(ChangeUserPasswordDialog)));

        public ChangeUserPasswordDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_OldPassword = (PasswordBox?)GetTemplateChild(nameof(PART_OldPassword));
            PART_NewPassword = (PasswordBox?)GetTemplateChild(nameof(PART_NewPassword));
            PART_Change = (Button?)GetTemplateChild(nameof(PART_Change));

            if (PART_Change != null)
                PART_Change.Click += OnChangeClicked;
        }

        private void OnChangeClicked(object sender, RoutedEventArgs e)
        {
            if (PART_OldPassword == null) throw new Exception($"Failed to fetch {PART_OldPassword}");
            if (!CurrentUser.Password.Equals(PART_OldPassword.Password))
            {
                Failure.Throw("The old Password does not match the current Password.", "Wrong Input");
                PART_OldPassword.Password = string.Empty;
                return;
            }

            if (PART_NewPassword == null) throw new Exception($"Failed to fetch {PART_NewPassword}");

            CurrentUser.ChangePassword(PART_NewPassword.Password);
            SuccessDialog.Display("User's Password successfully updated! You will need to login again when you launch the Application.");
            Close();
        }
    }

}
