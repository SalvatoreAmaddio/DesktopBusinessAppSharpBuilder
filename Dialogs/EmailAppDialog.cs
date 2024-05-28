using Backend.Utils;
using System.Windows.Controls;
using System.Windows;
using FrontEnd.Properties;

namespace FrontEnd.Dialogs
{
    public class EmailAppDialog : Window
    {
        Button? PART_Save;
        PasswordBox? PART_Password;
        static EmailAppDialog() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EmailAppDialog), new FrameworkPropertyMetadata(typeof(EmailAppDialog)));

        #region Username
        public string Username
        {
            get => (string)GetValue(UsernameProperty);
            set => SetValue(UsernameProperty, value);
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register(nameof(Username), typeof(string), typeof(EmailAppDialog), new PropertyMetadata(string.Empty));
        #endregion

        private Credential? credential;
        public EmailAppDialog()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Save = (Button?)GetTemplateChild(nameof(PART_Save));
            PART_Password = (PasswordBox?)GetTemplateChild(nameof(PART_Password));
            if (PART_Password == null) throw new Exception($"Failed to fetch {nameof(PART_Password)}");
            if (PART_Save != null)
                PART_Save.Click += OnSaveClicked;

            SysCredentailTargets.EmailApp = FrontEndSettings.Default.EmailUserName;
            credential = CredentialManager.Get(SysCredentailTargets.EmailApp);
            if (credential != null)
            {
                Username = credential.Username;
                Encrypter encrypter = new(credential.Password, SysCredentailTargets.EmailAppEncrypterKey, SysCredentailTargets.EmailAppEncrypterIV);
                PART_Password.Password = encrypter.Decrypt();
            }
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (PART_Password == null) throw new Exception($"Failed to fetch {nameof(PART_Password)}");
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(PART_Password.Password))
            {
                Failure.Throw("Username and/or Password have not been provided.");
                return;
            }

            SysCredentailTargets.EmailApp = Username;
            Encrypter encrypter = new(PART_Password.Password);
            CredentialManager.Replace(new(SysCredentailTargets.EmailApp, Username, encrypter.Encrypt()));
            encrypter.ReplaceStoredKeyIV(SysCredentailTargets.EmailAppEncrypterKey, SysCredentailTargets.EmailAppEncrypterIV);           
            FrontEndSettings.Default.EmailUserName = Username;
            FrontEndSettings.Default.Save();
            SuccessDialog.Display("Email's credentials successfully saved!");
            Close();
        }
    }
}