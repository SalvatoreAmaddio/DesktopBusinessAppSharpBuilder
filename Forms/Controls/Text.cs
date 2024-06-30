using FrontEnd.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends a <see cref="TextBox"/> and adds extra functionalities,
    /// such as a Placeholder property.
    /// <para/>
    /// It also overrides the TextProperty by setting its Binding to <see cref="UpdateSourceTrigger.PropertyChanged"/>.
    /// </summary>
    public partial class Text : TextBox, IDisposable
    {
        private Button? PART_clear_button;

        private readonly Image ClearImg = new()
        {
            Source = Helper.LoadFromImages("close")
        };

        /// <summary>
        /// Provides the resource dictionary for the control.
        /// </summary>
        protected virtual ResourceDictionary ResourceDict => Helper.GetDictionary(nameof(Text));

        #region Placeholder
        /// <summary>
        /// Gets and sets the Placeholder to be displayed when the TextBox is empty.
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Text), new PropertyMetadata(string.Empty));
        #endregion

        #region PlaceholderVisibility
        /// <summary>
        /// Gets or sets the visibility of the Placeholder.
        /// </summary>
        public Visibility PlaceholderVisibility
        {
            get => (Visibility)GetValue(PlaceholderVisibilityProperty);
            set => SetValue(PlaceholderVisibilityProperty, value);
        }

        public static readonly DependencyProperty PlaceholderVisibilityProperty =
            DependencyProperty.Register(nameof(PlaceholderVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ClearButtonVisibility
        /// <summary>
        /// Gets or sets the visibility of the clear button.
        /// </summary>
        public Visibility ClearButtonVisibility
        {
            get => (Visibility)GetValue(ClearButtonVisibilityProperty);
            set => SetValue(ClearButtonVisibilityProperty, value);
        }

        public static readonly DependencyProperty ClearButtonVisibilityProperty =
            DependencyProperty.Register(nameof(ClearButtonVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

        /// <summary>
        /// Static constructor for the <see cref="Text"/> class.
        /// Overrides the metadata for the TextProperty to use <see cref="UpdateSourceTrigger.PropertyChanged"/>.
        /// </summary>
        static Text() 
        {
                TextProperty.OverrideMetadata(
                typeof(Text),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null,
                    null,
                    false,
                    UpdateSourceTrigger.PropertyChanged
                ));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class.
        /// </summary>
        public Text()
        {
            Style = FetchStyle();
            Binding PlaceholderVisibilityBinding = new(nameof(Text))
            {
                Source = this,
                Converter = new TextToVisibility()
            };

            SetBinding(PlaceholderVisibilityProperty, PlaceholderVisibilityBinding);

            MultiBinding ClearButtonVisibilityMultiBinding = new()
            {
                Converter = new MultiBindingConverter()
            };

            Binding TextPropertyBinding = new(nameof(Text))
            {
                Source = this,
            };

            Binding IsKeyboardFocusWithinBinding = new(nameof(IsKeyboardFocusWithin))
            {
                Source = this,
            };

            ClearButtonVisibilityMultiBinding.Bindings.Add(TextPropertyBinding);
            ClearButtonVisibilityMultiBinding.Bindings.Add(IsKeyboardFocusWithinBinding);
            SetBinding(ClearButtonVisibilityProperty, ClearButtonVisibilityMultiBinding);
            Unloaded += OnUnloaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_clear_button = (Button)GetTemplateChild("PART_clear_button");
            PART_clear_button.Click += OnClearButtonClicked;
            PART_clear_button.Content = ClearImg;
        }

        /// <summary>
        /// Fetches the style for the Text control.
        /// </summary>
        /// <returns>The style to be applied to the control.</returns>
        protected virtual Style FetchStyle() 
        { 
            return (Style)ResourceDict["TextTemplateStyle"]; 
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly) return;
            Text = string.Empty;
        }

        public void Dispose()
        {
            if (PART_clear_button != null)
                PART_clear_button.Click -= OnClearButtonClicked;
            Unloaded -= OnUnloaded;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts the value of a <see cref="TextBox.Text"/> property to a <see cref="Visibility"/> object.
        /// It is used in the Converter for Binding of the <see cref="PlaceholderVisibilityProperty"/>.
        /// </summary>
        internal class TextToVisibility : IValueConverter
        {
            private string? _txt;
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                _txt = value.ToString();
                if (_txt == null) return Visibility.Visible;

                return (_txt.Length > 0) ? Visibility.Hidden : Visibility.Visible;
            }

            public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Visibility visibility = (Visibility)value;
                return (visibility == Visibility.Visible) ? "" : _txt;
            }
        }

        /// <summary>
        /// Handles multibinding involving strings and booleans to be converted as a <see cref="Visibility"/> object.
        /// </summary>
        internal class MultiBindingConverter : IMultiValueConverter
        {
            private string? _txt;
            private bool _hasFocus;
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                _txt = values[0].ToString();
                _hasFocus = System.Convert.ToBoolean(values[1].ToString());
                if (_txt == null) return Visibility.Hidden;
                return (_txt.Length > 0 && _hasFocus) ? Visibility.Visible : Visibility.Hidden;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                List<object?> list = [];
                Visibility visibility = (Visibility)value;
                list.Add((visibility == Visibility.Visible) ? _txt : "");
                return [.. list];
            }
        }
    }
}