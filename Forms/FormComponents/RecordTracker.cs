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
    /// Represents a RecordTracker object used to indicate the current record position within a RecordSource object.
    /// This control is typically used within an <see cref="AbstractForm"/> object.
    /// </summary>
    public class RecordTracker : AbstractControl
    {
        #region OnClickCommand
        /// <summary>
        /// Command triggered by clicking the one of the <see cref="RecordTracker"/>'s buttons.
        /// </summary>
        public ICommand OnClickCommand
        {
            get => (ICommand)GetValue(OnClickProperty);
            set => SetValue(OnClickProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="OnClickCommand"/>.
        /// </summary>
        public static readonly DependencyProperty OnClickProperty =
            DependencyProperty.Register(nameof(OnClickCommand), typeof(ICommand), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region Records
        /// <summary>
        /// Represents the current record's position information.
        /// </summary>
        public string Records
        {
            get => (string)GetValue(RecordsProperty);
            set => SetValue(RecordsProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="Records"/>.
        /// </summary>
        public static readonly DependencyProperty RecordsProperty =
            DependencyProperty.Register(nameof(Records), typeof(string), typeof(RecordTracker), new PropertyMetadata("Record 1 of 100"));
        #endregion

        #region GoNewVisibility
        /// <summary>
        /// Visibility property for the 'New' button.
        /// </summary>
        public Visibility GoNewVisibility
        {
            get => (Visibility)GetValue(GoNewVisibilityProperty);
            set => SetValue(GoNewVisibilityProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="GoNewVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty GoNewVisibilityProperty =
            DependencyProperty.Register(nameof(GoNewVisibility), typeof(Visibility), typeof(RecordTracker), new PropertyMetadata());
        #endregion

        #region NoInternetVisibility
        /// <summary>
        /// Visibility property indicating the status of internet connectivity.
        /// </summary>
        public Visibility NoInternetVisibility
        {
            get => (Visibility)GetValue(NoInternetVisibilityProperty);
            set => SetValue(NoInternetVisibilityProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="NoInternetVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty NoInternetVisibilityProperty = DependencyProperty.Register(nameof(NoInternetVisibility), typeof(Visibility), typeof(RecordTracker), new PropertyMetadata(Visibility.Hidden));
        #endregion

        #region Message
        /// <summary>
        /// Message property used for additional information display.
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>
        /// Dependency property for the <see cref="Message"/>.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(string), typeof(RecordTracker), new PropertyMetadata(string.Empty));
        #endregion

        /// <summary>
        /// Static constructor that overrides the default style key property for instances of <see cref="RecordTracker"/>.
        /// </summary>
        static RecordTracker() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RecordTracker), new FrameworkPropertyMetadata(typeof(RecordTracker)));

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordTracker"/> class.
        /// </summary>
        public RecordTracker()
        {
            OnClickCommand = new TrackerClickCommand(OnClicked);
            InternetConnection.Me.InternetStatusChanged += OnInternetStatusChanged;
        }

        /// <summary>
        /// Handles the event when internet status changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Event arguments containing the new internet status.</param>
        private void OnInternetStatusChanged(object? sender, Backend.Events.InternetConnectionStatusArgs e)
        {
           NoInternetVisibility = (e.IsConnected) ? Visibility.Hidden : Visibility.Visible;
        }

        #region IAbstractControl
        public override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IAbstractFormController) throw new ArgumentException("DataContext should be an instance of IAbstractFormController.");

            SetBinding(RecordsProperty, new Binding("Records") { Source = e.NewValue });

            SetBinding(GoNewVisibilityProperty, new Binding("AllowNewRecord") 
            {
                Source = e.NewValue,
                Mode = BindingMode.TwoWay,
                Converter = new AllowNewRecordConverter()
            });
        }
        public override void DisposeEvents()
        {
            base.DisposeEvents();
            InternetConnection.Me.InternetStatusChanged -= OnInternetStatusChanged;
        }
        #endregion

        private static MessageBoxResult Ask() => MessageBox.Show("Do you want to save the record before moving?", "Confirm", MessageBoxButton.YesNo);

        /// <summary>
        /// Performs the action when the tracker control is clicked.
        /// </summary>
        /// <param name="movement">The movement code indicating the direction or action.</param>
        protected virtual void OnClicked(int movement)
        {
            if (DataContext is not IAbstractFormController Controller) return;

            IAbstractModel? record = (IAbstractModel?)Controller.GetCurrentRecord();
            if (record != null && record.IsNewRecord() && movement == 5) return;
            if (record != null && record.IsNewRecord() && movement == 3) return;

            if (!Controller.ReadOnly)
            {
                if (record != null && record.IsDirty)
                {
                    if (Ask() == MessageBoxResult.No)
                        record.IsDirty = false;
                    else
                        if (!Controller.PerformUpdate()) return;
                }
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
                    if (Controller.EOF)
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

        /// <summary>
        /// Command implementation for handling click events with a specified action.
        /// </summary>
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

        /// <summary>
        /// Converter class to convert boolean values to visibility properties.
        /// </summary>
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