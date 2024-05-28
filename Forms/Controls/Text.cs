using FrontEnd.Utils;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends a <see cref="TextBox"/> and adds extra functionalities.
    /// Such as a Placeholder property.
    /// <para/>
    /// It also overrides the TextProperty by setting its Binding to <see cref="UpdateSourceTrigger.PropertyChanged"/>.
    /// </summary>
    public partial class Text : TextBox, IDisposable
    {
        protected bool _disposed = false;
        private Button? ClearButton;

        private readonly Image ClearImg = new()
        {
            Source = Helper.LoadFromImages("close")
        };

        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(Text));

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
        public Text()
        {
            Style = (Style)resourceDict["TextTemplateStyle"];
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
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ClearButton = (Button)GetTemplateChild("PART_clear_button");
            ClearButton.Click += OnClearButtonClicked;
            ClearButton.Content = ClearImg;
        }

        private void OnClearButtonClicked(object sender, RoutedEventArgs e) => Text = string.Empty;

        #region Placeholder
        /// <summary>
        /// Gets and Sets the Placeholder to be displayed when the TextBox is empty.
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
        public Visibility PlaceholderVisibility
        {
            get => (Visibility)GetValue(PlaceholderVisibilityProperty);
            set => SetValue(PlaceholderVisibilityProperty, value);
        }

        public static readonly DependencyProperty PlaceholderVisibilityProperty =
            DependencyProperty.Register(nameof(PlaceholderVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ClearButtonVisibility
        public Visibility ClearButtonVisibility
        {
            get => (Visibility)GetValue(ClearButtonVisibilityProperty);
            set => SetValue(ClearButtonVisibilityProperty, value);
        }

        public static readonly DependencyProperty ClearButtonVisibilityProperty =
            DependencyProperty.Register(nameof(ClearButtonVisibility), typeof(Visibility), typeof(Text), new PropertyMetadata(Visibility.Visible));
        #endregion

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (ClearButton != null)
                    ClearButton.Click -= OnClearButtonClicked;
            }

            _disposed = true;
        }

        ~Text() => Dispose(false);

        /// <summary>
        /// This class converts the value of a <see cref="TextBox.Text"/> property to a <see cref="Visibility"/> object.
        /// It is used in the Converter for Binding of the <see cref="PlaceholderVisibilityProperty"/>
        /// </summary>
        internal class TextToVisibility : IValueConverter
        {
            string? txt;
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                txt = value.ToString();
                if (txt == null) return Visibility.Visible;

                return (txt.Length > 0) ? Visibility.Hidden : Visibility.Visible;
            }

            public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                Visibility visibility = (Visibility)value;
                return (visibility == Visibility.Visible) ? "" : txt;
            }
        }

        /// <summary>
        /// This class is meant to handle multibinding involving strings and booleans to be converted as a <see cref="Visibility"/> object.
        /// </summary>
        internal class MultiBindingConverter : IMultiValueConverter
        {
            private string? txt;
            private bool focus;
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                txt = values[0].ToString();
                focus = System.Convert.ToBoolean(values[1].ToString());
                if (txt == null) return Visibility.Hidden;
                return (txt.Length > 0 && focus) ? Visibility.Visible : Visibility.Hidden;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                List<object?> list = [];
                Visibility visibility = (Visibility)value;
                list.Add((visibility == Visibility.Visible) ? txt : "");
                return [.. list];
            }
        }
    }
}