using FrontEnd.Controller;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class that defines a set of common properties and methods for custom buttons 
    /// which are to be bound to the Command objects defined in the <see cref="AbstractFormController{M}"/> and <see cref="AbstractFormListController{M}"/>.
    /// </summary>
    public abstract class AbstractButton : Button, IAbstractControl
    {
        public Window? ParentWindow { get; protected set; }

        /// <summary>
        /// Gets the tooltip text for the button. This property is implemented in derived classes.
        /// </summary>
        protected abstract string ToolTipText { get; }

        /// <summary>
        /// Gets the image key for the button. This property is implemented in derived classes.
        /// </summary>
        protected abstract string ImgKey { get; }

        /// <summary>
        /// Gets the command name for the button. This property is implemented in derived classes.
        /// </summary>
        protected abstract string CommandName { get; }

        #region IsWithinList
        /// <summary>
        /// Gets or sets a value indicating whether the button is within a list.
        /// This property establishes a relative source binding between the button's DataContext and the <see cref="Lista"/>'s DataContext.
        /// </summary>
        public bool IsWithinList
        {
            private get => (bool)GetValue(IsWithinListProperty);
            set => SetValue(IsWithinListProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsWithinList"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWithinListProperty =
        DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(AbstractButton), new PropertyMetadata(false, OnIsWithinListPropertyChanged));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractButton"/> class.
        /// </summary>
        public AbstractButton() 
        {
            DataContextChanged += OnDataContextChanged;
            ToolTip = ToolTipText;
            Content = new Image()
            {
                Source = Helper.LoadFromImages(ImgKey),
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        /// <summary>
        /// Handles changes to the <see cref="IsWithinList"/> property.
        /// </summary>
        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isWithinList = (bool)e.NewValue;
            if (isWithinList)
                ((AbstractButton)d).SetBinding(DataContextProperty, new Binding(nameof(DataContext))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else BindingOperations.ClearBinding(d, DataContextProperty);
        }

        /// <summary>
        /// Creates a binding for the specified property and source.
        /// </summary>
        private static Binding CreateBinding(string property, object source) => new(property) { Source = source };

        #region IAbstractControl
        /// <inheritdoc />
        public void OnUnloaded(object sender, RoutedEventArgs e) { }

        /// <inheritdoc />
        public void OnClosed(object? sender, EventArgs e) => Dispose();

        /// <inheritdoc />
        public void DisposeEvents()
        {
            DataContextChanged -= OnDataContextChanged;
            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }

        /// <inheritdoc />
        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IAbstractFormController controller) return;
            SetBinding(CommandParameterProperty, CreateBinding("CurrentRecord", controller));
            if (!string.IsNullOrEmpty(CommandName))
                SetBinding(CommandProperty, CreateBinding(CommandName, controller));
        }
        #endregion

        public void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Represents a button that is bound to the <see cref="AbstractFormController{M}.UpdateCMD"/> command.
    /// </summary>
    public class SaveButton : AbstractButton
    {
        /// <inheritdoc />
        protected override string CommandName => "UpdateCMD";

        /// <inheritdoc />
        protected override string ImgKey => "save";

        /// <inheritdoc />
        protected override string ToolTipText => "Save";
    }

    /// <summary>
    /// Represents a button that is bound to the <see cref="AbstractFormController{M}.DeleteCMD"/> command.
    /// </summary>
    public class DeleteButton : AbstractButton
    {
        /// <inheritdoc />
        protected override string ImgKey => "delete";

        /// <inheritdoc />
        protected override string ToolTipText => "Delete";

        /// <inheritdoc />
        protected override string CommandName => "DeleteCMD";
    }

    /// <summary>
    /// Represents a button that is bound to the <see cref="AbstractFormListController{M}.OpenCMD"/> command.
    /// </summary>
    public class OpenButton : AbstractButton
    {
        /// <inheritdoc />
        protected override string ToolTipText => "Open";

        /// <inheritdoc />
        protected override string CommandName => "OpenCMD";

        /// <inheritdoc />
        protected override string ImgKey => "folder";
    }

    /// <summary>
    /// Represents a button that is bound to the <see cref="AbstractFormController{M}.RequeryCMD"/> command.
    /// </summary>
    public class RequeryButton : AbstractButton
    {
        /// <inheritdoc />
        protected override string ToolTipText => "Requery";

        /// <inheritdoc />
        protected override string CommandName => "RequeryCMD";

        /// <inheritdoc />
        protected override string ImgKey => "requery";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequeryButton"/> class.
        /// Sets the background to transparent and removes the border.
        /// </summary>
        public RequeryButton() 
        {
            Background = Brushes.Transparent;
            BorderThickness = new(0);
        }
    }

    /// <summary>
    /// Represents a button that is used to open a report. This button does not bind to a specific command.
    /// </summary>
    public class ReportButton : AbstractButton
    {
        /// <inheritdoc />
        protected override string ToolTipText => "Open Report";

        /// <inheritdoc />
        protected override string CommandName => string.Empty;

        /// <inheritdoc />
        protected override string ImgKey => "report";
    }
}