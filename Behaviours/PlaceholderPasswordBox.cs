using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Behaviours
{
    /// <summary>
    /// Due to <see cref="PasswordBox"/> being a sealed class, and therefore not overridable,
    /// the following class represents a Behavior object which can be attached to the PasswordBox 
    /// in order to provide a Placeholder.
    /// For example in your xaml:
    /// <code>
    ///    //import the following namespaces:
    ///    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    ///    xmlns:b="clr-namespace:FrontEnd.Behaviours;assembly=FrontEnd"
    ///    ...
    ///    &lt;PasswordBox>
    ///     &lt;i:Interaction.Behaviors>
    ///         &lt;b:PlaceholderPasswordBox Placeholder="Password..."/>
    ///     &lt;/i:Interaction.Behaviors>
    ///    &lt;/PasswordBox>
    /// </code>
    /// </summary>
    public class PlaceholderPasswordBox : Behavior<PasswordBox>
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderPasswordBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets and Sets a Placeholder.
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += OnPasswordChanged;
            AssociatedObject.Loaded += OnLoaded;
            TogglePlaceholder();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= OnPasswordChanged;
            AssociatedObject.Loaded -= OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) => TogglePlaceholder();
        private void OnPasswordChanged(object sender, RoutedEventArgs e) => TogglePlaceholder();
        
        private void TogglePlaceholder()
        {
            if (AssociatedObject.Template == null) return;
            Label? placeholder = AssociatedObject.Template.FindName("Placeholder", AssociatedObject) as Label;
            if (placeholder == null) return;
                if (AssociatedObject.Password.Length == 0)
                    placeholder.Content = Placeholder;
                else
                    placeholder.Content = "";
        }
    }

}
