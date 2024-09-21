using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract base class for <see cref="Form"/>, <see cref="FormList"/>.
    /// Provides common properties and methods for form-related classes.
    /// </summary>
    public abstract class AbstractForm() : AbstractContentControl
    {
        #region IsLoading
        /// <summary>
        /// Gets or sets a value indicating whether the form is currently loading.
        /// This property is bound to the <see cref="ProgressBar.IsIndeterminate"/> property.
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsLoading"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(AbstractForm), new PropertyMetadata(false));
        #endregion

        #region Header
        /// <summary>
        /// Gets or sets the header content of the form.
        /// </summary>
        public UIElement Header
        {
            get => (UIElement)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(UIElement), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        #region Menu
        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Controls.Menu"/> associated with the form.
        /// </summary>
        public Menu Menu
        {
            get => (Menu)GetValue(MenuProperty);
            set => SetValue(MenuProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Menu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(Menu), typeof(AbstractForm), new PropertyMetadata(OnElementChanged));
        #endregion

        #region MenuRow
        /// <summary>
        /// Gets or sets the height of the <see cref="Menu"/> row.
        /// </summary>
        public GridLength MenuRow
        {
            get => (GridLength)GetValue(MenuRowProperty);
            set => SetValue(MenuRowProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MenuRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MenuRowProperty =
            DependencyProperty.Register(nameof(MenuRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region HeaderRow
        /// <summary>
        /// Gets or sets the height of the <see cref="Header"/> row.
        /// </summary>
        public GridLength HeaderRow
        {
            get => (GridLength)GetValue(HeaderRowProperty);
            set => SetValue(HeaderRowProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HeaderRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderRowProperty =
            DependencyProperty.Register(nameof(HeaderRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(0), null));
        #endregion

        #region RecordTrackerRow
        /// <summary>
        /// Gets or sets the height of the record tracker row.
        /// </summary>
        public GridLength RecordTrackerRow
        {
            get => (GridLength)GetValue(RecordTrackerRowProperty);
            set => SetValue(RecordTrackerRowProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RecordTrackerRow"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecordTrackerRowProperty =
            DependencyProperty.Register(nameof(RecordTrackerRow), typeof(GridLength), typeof(AbstractForm), new PropertyMetadata(new GridLength(30), null));
        #endregion

        /// <summary>
        /// Static constructor to override the default style key property.
        /// </summary>
        static AbstractForm() => DefaultStyleKeyProperty.OverrideMetadata(typeof(AbstractForm), new FrameworkPropertyMetadata(typeof(AbstractForm)));

        /// <summary>
        /// Called when a dependency property such as <see cref="Menu"/> or <see cref="Header"/> changes.
        /// Updates the corresponding <see cref="MenuRow"/> or <see cref="HeaderRow"/> property based on the new value.
        /// </summary>
        /// <param name="d">The dependency object that had its property changed.</param>
        /// <param name="e">Event arguments containing information about the property change.</param>
        private static void OnElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractForm control = (AbstractForm)d;
            switch (true)
            {
                case true when e.Property.Equals(MenuProperty):
                    control.MenuRow = new(SetRow(e.NewValue, 20));
                    break;
                case true when e.Property.Equals(HeaderProperty):
                    control.HeaderRow = new(SetRow(e.NewValue, control.HeaderRow.Value));
                    break;
            }
        }

        /// <summary>
        /// Determines the row height based on the presence of a value.
        /// </summary>
        /// <param name="value">The value of the property that was changed.</param>
        /// <param name="height">The height to set if the value is not null.</param>
        /// <returns>A double representing the new height of the row.</returns>
        private static double SetRow(object value, double height) => value == null ? 0 : height;

        public override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
    }
}