using FrontEnd.Model;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class instantiate a CommandPanel object which contains a <see cref="SaveButton"/> and a <see cref="DeleteButton"/>
    /// </summary>
    public class CommandPanel : Control
    {
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand UpdateCMD 
        {
            get => (ICommand)GetValue(UpdateCMDProperty);
            set => SetValue(UpdateCMDProperty, value);
        }


        public static readonly DependencyProperty UpdateCMDProperty =
        DependencyProperty.Register(nameof(UpdateCMD), typeof(ICommand), typeof(CommandPanel), new PropertyMetadata());

        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand DeleteCMD
        {
            get => (ICommand)GetValue(DeleteCMDProperty);
            set => SetValue(DeleteCMDProperty, value);
        }


        public static readonly DependencyProperty DeleteCMDProperty =
        DependencyProperty.Register(nameof(DeleteCMD), typeof(ICommand), typeof(CommandPanel), new PropertyMetadata());

        public AbstractModel CommandParameter
        {
            get => (AbstractModel)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(AbstractModel), typeof(CommandPanel), new PropertyMetadata());

        static CommandPanel() => DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandPanel), new FrameworkPropertyMetadata(typeof(CommandPanel)));
    }
}