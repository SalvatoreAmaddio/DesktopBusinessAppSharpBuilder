using System.Globalization;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using FrontEnd.Utils;
using Backend.ExtensionMethods;

namespace FrontEnd.Forms
{
    public partial class TextBoxDate : Text
    {
        private readonly TextBoxCalendar Calendar = new();
        private Button? PART_Button;
        private Popup? PART_Popup;

        #region Date
        public DateTime? Date
        {
            get => (DateTime?)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register(nameof(Date), typeof(DateTime?), typeof(TextBoxDate),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region TogglePopupCommand
        public ICommand TogglePopupCommand
        {
            get => (ICommand)GetValue(TogglePopupCommandProperty);
            set => SetValue(TogglePopupCommandProperty, value);
        }

        public static readonly DependencyProperty TogglePopupCommandProperty =
            DependencyProperty.Register(nameof(TogglePopupCommand), typeof(ICommand), typeof(TextBoxDate), new PropertyMetadata(null));
        #endregion

        protected override ResourceDictionary resourceDict => Helper.GetDictionary(nameof(TextBoxDate));
        static TextBoxDate()
        {
            TextProperty.OverrideMetadata(
            typeof(TextBoxDate),
            new FrameworkPropertyMetadata(
                DateTime.Today.ToString("dd/MM/yyyy"),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                null,
                null,
                false,
                UpdateSourceTrigger.LostFocus
            ));
        }
        public TextBoxDate() : base()
        {
            Style = FetchStyle();
            TogglePopupCommand = new Command(TogglePopup);

            Binding binding = new(nameof(Date))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
            };

            this.Calendar.SetBinding(TextBoxCalendar.SelectedDateProperty, binding);

            Binding textBinding = new(nameof(Date))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new ConvertDateToString()
            };
            this.SetBinding(TextProperty, textBinding);
        }

        protected override Style FetchStyle() => (Style)resourceDict["TextBoxDateTemplateStyle"];

        /// <summary>
        /// Allow numbers only.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            int number; 
            bool allowed = Int32.TryParse(e.Text, out number);
            if (!allowed) 
            {
                e.Handled = true; //cancel the input
                return;
            }

            if (Text.Length == 10)
            {
                if (Text[CaretIndex].Equals('/'))
                {
                    CaretIndex = CaretIndex + 1;
                    e.Handled = true; //cancel the input
                    return;
                }

                int position = CaretIndex;
                Text = Text.ReplaceCharAt(position, e.Text[0]);
                CaretIndex = position;
                e.Handled = true; //cancel the input
                return;
            }
        }

        /// <summary>
        /// Add the "/" as the user types.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (Text.Length == 2) 
            {
                Text = $"{Text}/";
                CaretIndex = 3; //move caret to next position
            }

            if (Text.Length == 5)
            {
                Text = $"{Text}/";
                CaretIndex = 6; //move caret to next position
            }
        }

        private void TogglePopup()
        {
            if (PART_Popup != null)
                PART_Popup.IsOpen = !PART_Popup.IsOpen;
            if (Date != null)
                Calendar.DisplayDate = Date.Value; // update calendar
            else
                Calendar.DisplayDate = DateTime.Today; // update calendar
            Calendar.Focus(); //move focus from TextBox to the Calendar object
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Button = (Button?)GetTemplateChild(nameof(PART_Button)); //retrive the button from the Template
            PART_Popup = (Popup?)GetTemplateChild(nameof(PART_Popup)); //retrive the popup from the Template
            if (PART_Popup != null)
            {
                PART_Popup.Child = Calendar; //add the Calendar to the popup
                Binding binding = new("IsOpen")
                {
                    Source = PART_Popup,
                    Mode = BindingMode.TwoWay,
                };

                Calendar.SetBinding(TextBoxCalendar.IsOpenPopupProperty, binding);
            }
        }

        internal class TextBoxCalendar : System.Windows.Controls.Calendar
        {
            #region IsOpenPopup
            public bool IsOpenPopup
            {
                get => (bool)GetValue(IsOpenPopupProperty);
                set => SetValue(IsOpenPopupProperty, value);
            }

            public static readonly DependencyProperty IsOpenPopupProperty =
                DependencyProperty.Register(nameof(TogglePopupCommand), typeof(bool), typeof(TextBoxCalendar), new PropertyMetadata(false));
            #endregion

            public TextBoxCalendar() => IsTodayHighlighted = true;
            protected override void OnSelectedDatesChanged(SelectionChangedEventArgs e)
            {
                base.OnSelectedDatesChanged(e);
                try
                {
                    SelectedDate = (DateTime?)e.AddedItems[0];
                    DisplayDate = (DateTime)e.AddedItems[0];
                }
                catch
                {
                    SelectedDate = null;
                }
                finally
                {
                    IsOpenPopup = false;
                }
            }
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