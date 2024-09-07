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
    /// <summary>
    /// This class extends the <see cref="Text"/> class and adds date-related functionalities.
    /// It includes properties for selecting a date and commands for toggling a popup calendar.
    /// </summary>
    public partial class TextBoxDate : Text
    {
        private readonly TextBoxCalendar _calendar = new();
        private Button? PART_Button;
        private Popup? PART_Popup;

        #region Date
        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        public DateTime? Date
        {
            get => (DateTime?)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Date"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
        DependencyProperty.Register(nameof(Date), typeof(DateTime?), typeof(TextBoxDate),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region TogglePopupCommand
        /// <summary>
        /// Gets or sets the command to toggle the popup calendar.
        /// </summary>
        public ICommand TogglePopupCommand
        {
            get => (ICommand)GetValue(TogglePopupCommandProperty);
            set => SetValue(TogglePopupCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TogglePopupCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TogglePopupCommandProperty =
        DependencyProperty.Register(nameof(TogglePopupCommand), typeof(ICommand), typeof(TextBoxDate), new PropertyMetadata(null));
        #endregion

        protected override ResourceDictionary ResourceDict => Helper.GetDictionary(nameof(TextBoxDate));

        /// <summary>
        /// Static constructor for the <see cref="TextBoxDate"/> class.
        /// Overrides the metadata for the TextProperty to use <see cref="UpdateSourceTrigger.LostFocus"/>.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TextBoxDate"/> class.
        /// Sets up bindings and commands for the control.
        /// </summary>
        public TextBoxDate() : base()
        {
            Style = FetchStyle();
            TogglePopupCommand = new Command(TogglePopup);

            Binding binding = new(nameof(Date))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
            };

            this._calendar.SetBinding(TextBoxCalendar.SelectedDateProperty, binding);

            Binding textBinding = new(nameof(Date))
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = new ConvertDateToString()
            };
            this.SetBinding(TextProperty, textBinding);
        }
        
        protected override Style FetchStyle() => (Style)ResourceDict["TextBoxDateTemplateStyle"];

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_Button = (Button?)GetTemplateChild(nameof(PART_Button)); //retrive the button from the Template
            PART_Popup = (Popup?)GetTemplateChild(nameof(PART_Popup)); //retrive the popup from the Template
            if (PART_Popup != null)
            {
                PART_Popup.Child = _calendar; //add the Calendar to the popup
                Binding binding = new("IsOpen")
                {
                    Source = PART_Popup,
                    Mode = BindingMode.TwoWay,
                };

                _calendar.SetBinding(TextBoxCalendar.IsOpenPopupProperty, binding);
            }
        }

        /// <summary>
        /// Allows only numeric input.
        /// </summary>
        /// <param name="e">Provides data about the text composition event.</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            bool allowed = Int32.TryParse(e.Text, out _);
            if (!allowed) 
            {
                e.Handled = true; //cancel the input
                return;
            }

            if (Text.Length == 10)
            {
                try 
                {
                    if (Text[CaretIndex].Equals('/'))
                    {
                        CaretIndex = CaretIndex + 1;
                        e.Handled = true; //cancel the input
                        return;
                    }
                }
                catch 
                {
                    e.Handled = true;
                    return;
                }

                int position = CaretIndex;
                Text = Text.ReplaceCharAt(position, e.Text[0]);
                CaretIndex = position + 1;

                try
                {
                    if (Text[CaretIndex].Equals('/')) 
                    {
                        CaretIndex = CaretIndex + 1;
                    }
                }
                catch { }
                e.Handled = true; //cancel the input
                return;
            }
        }

        /// <summary>
        /// Adds the "/" character as the user types the date.
        /// </summary>
        /// <param name="e">Provides data about the text changed event.</param>
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

        /// <summary>
        /// Toggles the visibility of the popup calendar.
        /// </summary>
        private void TogglePopup()
        {
            if (PART_Popup != null)
                PART_Popup.IsOpen = !PART_Popup.IsOpen;
            if (Date != null)
                _calendar.DisplayDate = Date.Value; // update calendar
            else
                _calendar.DisplayDate = DateTime.Today; // update calendar
            _calendar.Focus(); //move focus from TextBox to the Calendar object
        }

        /// <summary>
        /// A custom Calendar control used within the <see cref="TextBoxDate"/> control.
        /// </summary>
        internal class TextBoxCalendar : System.Windows.Controls.Calendar
        {
            #region IsOpenPopup
            /// <summary>
            /// Gets or sets a value indicating whether the popup is open.
            /// </summary>
            public bool IsOpenPopup
            {
                get => (bool)GetValue(IsOpenPopupProperty);
                set => SetValue(IsOpenPopupProperty, value);
            }

            /// <summary>
            /// Identifies the <see cref="IsOpenPopup"/> dependency property.
            /// </summary>
            public static readonly DependencyProperty IsOpenPopupProperty =
            DependencyProperty.Register(nameof(TogglePopupCommand), typeof(bool), typeof(TextBoxCalendar), new PropertyMetadata(false));
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="TextBoxCalendar"/> class.
            /// </summary>
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

        /// <summary>
        /// A command implementation for toggling the popup calendar.
        /// </summary>
        internal class Command : ICommand
        {
            public event EventHandler? CanExecuteChanged;
            private Action _action;

            /// <summary>
            /// Initializes a new instance of the <see cref="Command"/> class.
            /// </summary>
            /// <param name="action">The action to execute when the command is invoked.</param>
            public Command(Action action) => _action = action;
            public bool CanExecute(object? parameter) => true;

            public void Execute(object? parameter) => _action?.Invoke();
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> value to a string and vice versa.
        /// </summary>
        internal class ConvertDateToString : IValueConverter
        {
            public object? Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value is DateTime date)
                    return date.ToString("dd/MM/yyyy", culture);
                return value;
            }

            /// <summary>
            /// Analyzes and adjusts a date string to ensure proper formatting.
            /// </summary>
            /// <param name="dateString">The date string to analyze and adjust.</param>
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

            /// <summary>
            /// Adjusts the year part of a date string.
            /// </summary>
            /// <param name="chunk">The year part of the date string.</param>
            private static void AdjustYear(ref string chunk)
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

            /// <summary>
            /// Adjusts the month part of a date string.
            /// </summary>
            /// <param name="chunk">The month part of the date string.</param>
            private static void AdjustMonth(ref string chunk)
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

            /// <summary>
            /// Adjusts the day part of a date string.
            /// </summary>
            /// <param name="chunk">The day part of the date string.</param>
            private static void AdjustDay(ref string chunk)
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

            public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string dateString)
                {
                    AnalyseDateString(ref dateString);
                    if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                        return dateTime;
                    if (!string.IsNullOrEmpty(dateString))
                        MessageBox.Show($"The value {dateString} is not in the correct format.", "Wrong Format");
                }
                return null;
            }
        }
    }
}