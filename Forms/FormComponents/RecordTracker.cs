using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace FrontEnd.Forms.FormComponents
{
    /// <summary>
    /// This class represents a RecordTracker object. A RecordTracker object tells on which record the user is within the RecordSource object.
    /// This control is used within a <see cref="AbstractForm"/> object.
    /// </summary>
    public class RecordTracker : AbstractControl
    {
        static RecordTracker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RecordTracker), new FrameworkPropertyMetadata(typeof(RecordTracker)));

        public RecordTracker()
        {
            OnClickCommand = new TrackerClickCommand(OnClicked);
            InternetConnection.Event.InternetStatusChanged += OnInternetStatusChanged;
        }

        private void OnInternetStatusChanged(object? sender, Backend.Events.InternetConnectionStatusArgs e)
        {
           NoInternetVisibility = (e.IsConnected) ? Visibility.Hidden : Visibility.Visible;
        }

        #region OnClickCommand
        public ICommand OnClickCommand
        {
            get => (ICommand)GetValue(OnClickProperty);
            set => SetValue(OnClickProperty, value);
        }

        public static readonly DependencyProperty OnClickProperty =
            DependencyProperty.Register(nameof(OnClickCommand), typeof(ICommand), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region Records
        public string Records
        {
            get => (string)GetValue(RecordsProperty);
            set => SetValue(RecordsProperty, value);
        }

        public static readonly DependencyProperty RecordsProperty =
            DependencyProperty.Register(nameof(Records), typeof(string), typeof(RecordTracker), new PropertyMetadata("Record 1 of 100"));
        #endregion

        #region GoNewVisibility
        public Visibility GoNewVisibility
        {
            get => (Visibility)GetValue(GoNewVisibilityProperty);
            set => SetValue(GoNewVisibilityProperty, value);
        }

        public static readonly DependencyProperty GoNewVisibilityProperty =
            DependencyProperty.Register(nameof(GoNewVisibility), typeof(Visibility), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region NoInternetVisibility
        public Visibility NoInternetVisibility
        {
            get => (Visibility)GetValue(NoInternetVisibilityProperty);
            set => SetValue(NoInternetVisibilityProperty, value);
        }

        public static readonly DependencyProperty NoInternetVisibilityProperty = DependencyProperty.Register(nameof(NoInternetVisibility), typeof(Visibility), typeof(RecordTracker), new PropertyMetadata(Visibility.Hidden));
        #endregion

        #region Message
        public static readonly DependencyProperty MessageProperty =
         DependencyProperty.Register(nameof(Message), typeof(string), typeof(RecordTracker), new PropertyMetadata(string.Empty));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        #endregion

        protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IAbstractFormController) throw new Exception("DataContext should be an instance of IAbstractFormController.");

            SetBinding(RecordsProperty, new Binding("Records") { Source = e.NewValue });

            SetBinding(GoNewVisibilityProperty, new Binding("AllowNewRecord") 
            {
                Source = e.NewValue,
                Converter = new AllowNewRecordConverter()
            });
        }

        private static MessageBoxResult Ask() => MessageBox.Show("Do you want to save the record before moving?", "Confirm", MessageBoxButton.YesNo);

        protected virtual void OnClicked(int movement)
        {
            if (DataContext is not IAbstractFormController Controller) return;

            AbstractModel? record = (AbstractModel?)Controller.CurrentModel;

            if (record != null && record.IsNewRecord() && record.IsDirty) 
            {
                if (Ask() == MessageBoxResult.No)
                    record.IsDirty = false;
                else
                    if (!Controller.PerformUpdate()) return;
            }

            switch (movement)
            {
                case 1:
                    Controller.GoFirst();
                    break;
                case 2:
                    Controller.GoPrevious();
                    break;
                case 3:
                    if (Controller.Source.Navigate().EOF) 
                    {
                            if (Controller is IAbstractFormListController listController && listController.OpenWindowOnNew)
                                break;
                            Controller.GoNew();
                    }
                    else Controller.GoNext();
                    break;
                case 4:
                    Controller.GoLast();
                    break;
                case 5:
                    Controller.GoNew();
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                InternetConnection.Event.InternetStatusChanged -= OnInternetStatusChanged;
            }
        }

        ~RecordTracker() => Dispose(false);

        internal class TrackerClickCommand(Action<int> _execute) : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private readonly Action<int> _execute = _execute;

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                if (parameter == null) throw new Exception("Parameter was null");
                string? str = parameter.ToString();
                if (string.IsNullOrEmpty(str)) throw new Exception("Parameter was null");
                _execute(int.Parse(str));
            }
        }

        internal class AllowNewRecordConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool boolValue)
                {
                    return boolValue ? Visibility.Visible : Visibility.Hidden;
                }
                return Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is Visibility visibility)
                    return visibility.Equals(Visibility.Visible) ? true : false;
                return ">";
            }
        }
    }
}