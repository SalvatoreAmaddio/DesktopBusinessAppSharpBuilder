using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class representing a Form Object.
    /// </summary>
    public abstract class AbstractForm() : AbstractContentControl
    {
        #region IsLoading
        /// <summary>
        /// Gets and Sets the <see cref="ProgressBar.IsIndeterminate"/> property.
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(AbstractForm), new PropertyMetadata(false));
        #endregion

        #region Header
        /// <summary>
        /// Gets and Sets the Form Header.
        /// </summary>
        public UIElement Header
        {
            get => (UIElement)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(UIElement), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        #region Menu
        /// <summary>
        /// Gets and sets a <see cref="System.Windows.Controls.Menu"/> object.
        /// </summary>
        public Menu Menu
        {
            get => (Menu)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(Menu), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        #region MenuRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Menu"/> property.
        /// </summary>
        public GridLength MenuRow
        {
            get => (GridLength)GetValue(MenuRowProperty);
            set => SetValue(MenuRowProperty, value);
        }

        public static readonly DependencyProperty MenuRowProperty =
            DependencyProperty.Register(nameof(MenuRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region HeaderRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="Header"/> property.
        /// </summary>
        public GridLength HeaderRow
        {
            get => (GridLength)GetValue(HeaderRowProperty);
            set => SetValue(HeaderRowProperty, value);
        }

        public static readonly DependencyProperty HeaderRowProperty =
            DependencyProperty.Register(nameof(HeaderRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region RecordTrackerRow
        /// <summary>
        /// Gets and Sets a <see cref="GridLength"/> object which regulates the height of the <see cref="FormComponents.RecordTracker"/> object.
        /// </summary>
        public GridLength RecordTrackerRow
        {
            get => (GridLength)GetValue(RecordTrackerRowProperty);
            set => SetValue(RecordTrackerRowProperty, value);
        }

        public static readonly DependencyProperty RecordTrackerRowProperty =
            DependencyProperty.Register(nameof(RecordTrackerRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(30), null));
        #endregion

        static AbstractForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractForm), new FrameworkPropertyMetadata(typeof(AbstractForm)));
        protected static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractForm control = (AbstractForm)d;
            switch (true)
            {
                case true when e.Property.Equals(MenuProperty):
                    control.MenuRow = new(SetRow(e.NewValue, 20));
                    break;
                case true when e.Property.Equals(HeaderProperty):
                    control.HeaderRow = new(SetRow(e.NewValue, 40));
                    break;
            }
        }

        protected static int SetRow(object value, int height)
        {
            if (value == null) return 0;
            else return height;
        }

        protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IAbstractFormController)
            {
                Binding isLoadingBinding = new(nameof(IsLoading))
                {
                    Source = e.NewValue,
                };
                SetBinding(IsLoadingProperty, isLoadingBinding);
            }
        }

        ~AbstractForm() { }
    }
}
