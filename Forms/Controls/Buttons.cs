using FrontEnd.Controller;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Abstract class that defines a set of common properties and methods for custom buttons which are to be bound to the Command objects defined in the <see cref="AbstractFormController{M}"/> and <see cref="AbstractFormListController{M}"/>.
    /// </summary>
    public abstract class AbstractButton : Button 
    {
        protected abstract string ToolTipText { get; }
        protected abstract string ImgKey { get; }
        protected abstract string CommandName { get; }
        public AbstractButton() 
        {
            Unloaded += OnUnloaded;
            DataContextChanged += OnDataContextChanged;
            ToolTip = ToolTipText;
            Content = new Image()
            {
                Source = Helper.LoadFromImages(ImgKey),
            };
        }
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DataContextChanged -= OnDataContextChanged;
            Unloaded -= OnUnloaded;
        }

        #region IsWithinList
        /// <summary>
        /// This property works as a short-hand to set a Relative Source Binding between the button's DataContext and the <see cref="Lista"/>'s DataContext.
        /// </summary>
        public bool IsWithinList
        {
            private get => (bool)GetValue(IsWithinListProperty);
            set => SetValue(IsWithinListProperty, value);
        }

        public static readonly DependencyProperty IsWithinListProperty =
            DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(AbstractButton), new PropertyMetadata(false, OnIsWithinListPropertyChanged));

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
        #endregion

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not IAbstractFormController controller) return;
            SetBinding(CommandParameterProperty, CreateBinding("CurrentRecord", controller));
            if (!string.IsNullOrEmpty(CommandName))
                SetBinding(CommandProperty, CreateBinding(CommandName, controller));
        }

        private static Binding CreateBinding(string property, object source) => new(property) { Source = source };
    }

    /// <summary>
    /// Instantiate SaveButton and binds it to the <see cref="AbstractFormController{M}.UpdateCMD"/> Command.
    /// </summary>
    public class SaveButton : AbstractButton
    {
        protected override string CommandName => "UpdateCMD";
        protected override string ImgKey => "save";
        protected override string ToolTipText => "Save";
    }

    /// <summary>
    /// Instantiate DeleteButton and binds it to the <see cref="AbstractFormController{M}.DeleteCMD"/> Command.
    /// </summary>
    public class DeleteButton : AbstractButton
    {
        protected override string ImgKey => "delete";
        protected override string ToolTipText => "Delete";
        protected override string CommandName => "DeleteCMD";
    }

    /// <summary>
    /// Instantiate OpenButton and binds it to the <see cref="AbstractFormListController{M}.OpenCMD"/> Command.
    /// </summary>
    public class OpenButton : AbstractButton
    {
        protected override string ToolTipText => "Open";
        protected override string CommandName => "OpenCMD";
        protected override string ImgKey => "folder";
    }

    /// <summary>
    /// Instantiate RequeryButton and binds it to the <see cref="AbstractFormController{M}.RequeryCMD"/> Command.
    /// </summary>
    public class RequeryButton : AbstractButton
    {
        protected override string ToolTipText => "Requery";
        protected override string CommandName => "RequeryCMD";
        protected override string ImgKey => "requery";

        public RequeryButton() 
        {
            Background = Brushes.Transparent;
            BorderThickness = new(0);
        }
    }

    public class ReportButton : AbstractButton
    {
        protected override string ToolTipText => "Open Report";
        protected override string CommandName => string.Empty;
        protected override string ImgKey => "report";
    }
}