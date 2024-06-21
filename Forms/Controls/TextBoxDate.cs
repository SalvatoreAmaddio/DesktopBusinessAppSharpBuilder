using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using FrontEnd.Utils;

namespace FrontEnd.Forms
{
    public class TextBoxDate : Text
    {
        public System.Windows.Controls.Calendar Calendar = new();
        private Button? PART_Button;
        private Popup? PART_Popup;

        #region SelectedDate
        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(TextBoxDate),
                new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDatePropertyChanged));
        private static void OnSelectedDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBoxDate control = (TextBoxDate)d;
            if (e.NewValue == null)
            {
                control.Calendar.DisplayDate = DateTime.Today;
                return;
            }
            control.Calendar.DisplayDate = (DateTime)e.NewValue;
        }
        #endregion

        #region DropDownButtonClickCommand
        public ICommand DropDownButtonClickCommand
        {
            get => (ICommand)GetValue(DropDownButtonClickCommandProperty);
            set => SetValue(DropDownButtonClickCommandProperty, value);
        }

        public static readonly DependencyProperty DropDownButtonClickCommandProperty =
            DependencyProperty.Register(nameof(DropDownButtonClickCommand), typeof(ICommand), typeof(TextBoxDate), new PropertyMetadata(null));
        #endregion

        protected new readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(TextBoxDate));
        static TextBoxDate()
        {
            TextProperty.OverrideMetadata(
            typeof(TextBoxDate),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                null,
                null,
                false,
                UpdateSourceTrigger.LostFocus
            ));
        }
        public TextBoxDate() : base()
        {
            DropDownButtonClickCommand = new Command(OpenPopup);
            Binding binding = new(nameof(SelectedDate))
            {
                Source = Calendar,
                Mode = BindingMode.TwoWay,
            };

            this.SetBinding(SelectedDateProperty, binding);

            Binding textBinding = new(nameof(SelectedDate))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new ConvertDateToString()
            };
            this.SetBinding(TextProperty, textBinding);
            Calendar.SelectedDatesChanged += Calendar_SelectedDatesChanged;
        }

        protected override Style FetchStyle()
        {
            return (Style)resourceDict["TextBoxDateTemplateStyle"];
        }

        private void Calendar_SelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
        {
           PART_Popup.IsOpen = false;
        }

        private void OpenPopup()
        {
            PART_Popup.IsOpen = !PART_Popup.IsOpen;
            Calendar.Focus();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Button = (Button?)GetTemplateChild(nameof(PART_Button));
            PART_Popup = (Popup?)GetTemplateChild(nameof(PART_Popup));
            if (PART_Popup != null)
                PART_Popup.Child = Calendar;
        }

        internal class Command : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private Action _action;
            public Command(Action action)
            {
                _action = action;
            }
            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                _action?.Invoke();
            }
        }

        internal class ConvertDateToString : IValueConverter
        {
            public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is DateTime date)
                {
                    return date.ToString("dd/MM/yyyy", culture);
                }
                return value;
            }

            private void AnalyseDateString(ref string dateString)
            {
                string[] chunks = dateString.Split("/");
                if (chunks.Length == 0) return;

                try
                {
                    chunks[0] = chunks[0].Trim();
                    AdjustDay(ref chunks[0]);
                    chunks[1] = chunks[1].Trim();
                    AdjustMonth(ref chunks[1]);
                    chunks[2] = chunks[2].Trim();
                    AdjustYear(ref chunks[2]);
                }
                catch
                {

                }
                finally
                {
                    dateString = string.Join("/", chunks);
                }
            }
            private void AdjustYear(ref string chunk)
            {
                int value = Int32.Parse(chunk);
                if (value < 100)
                {
                    if (value == 0)
                    {
                        chunk = $"20{value}0";
                        return;
                    }
                    chunk = $"19{value}";
                }
            }

            private void AdjustMonth(ref string chunk)
            {
                int value = Int32.Parse(chunk);
                if (value >= 1 && value <= 9)
                {
                    chunk = $"0{value}";
                }

                if (value > 12)
                {
                    chunk = $"01";
                }
            }

            private void AdjustDay(ref string chunk)
            {
                int value = Int32.Parse(chunk);
                if (value >= 1 && value <= 9)
                {
                    chunk = $"0{value}";
                }

                if (value > 31)
                {
                    chunk = $"{chunk[1]}{chunk[0]}";
                }
            }

            public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is string dateString)
                {
                    AnalyseDateString(ref dateString);
                    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                    {
                        return dateTime;
                    }
                    if (!string.IsNullOrEmpty(dateString))
                    {
                        MessageBox.Show($"The value {dateString} is not in the correct format.", "Wrong Format");
                    }
                }
                return null;
            }
        }

    }

}
