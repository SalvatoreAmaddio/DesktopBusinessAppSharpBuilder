using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace FrontEnd.Behaviours
{
    /// <summary>
    /// Represents a behavior that can be attached to a <see cref="PasswordBox"/> to provide a placeholder text.
    /// Since <see cref="PasswordBox"/> is a sealed class and cannot be inherited, this behavior allows adding placeholder functionality.
    /// <example>
    /// Example usage in your XAML file:
    /// <code>
    /// &lt;!-- Import the following namespaces: -->
    ///    xmlns:i="http://schemas.microsoft.com/xaml/behaviors"
    ///    xmlns:b="clr-namespace:FrontEnd.Behaviours;assembly=FrontEnd"
    ///    ...
    ///    &lt;PasswordBox>
    ///     &lt;i:Interaction.Behaviors>
    ///         &lt;b:PlaceholderPasswordBox Placeholder="Password..."/>
    ///     &lt;/i:Interaction.Behaviors>
    ///    &lt;/PasswordBox>
    /// </code>
    /// </example>
    /// </summary>
    public class PlaceholderPasswordBox : Behavior<PasswordBox>
    {
        /// <summary>
        /// Identifies the <see cref="Placeholder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(PlaceholderPasswordBox), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the placeholder text to be displayed when the <see cref="PasswordBox"/> is empty.
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

        /// <summary>
        /// Handles the Loaded event of the <see cref="PasswordBox"/>.
        /// Toggles the placeholder visibility.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e) => TogglePlaceholder();

        /// <summary>
        /// Handles the PasswordChanged event of the <see cref="PasswordBox"/>.
        /// Toggles the placeholder visibility.
        /// </summary>
        private void OnPasswordChanged(object sender, RoutedEventArgs e) => TogglePlaceholder();

        /// <summary>
        /// Toggles the placeholder visibility based on whether the <see cref="PasswordBox"/> is empty.
        /// </summary>
        private void TogglePlaceholder()
        {
            if (AssociatedObject.Template == null) return;
            if (AssociatedObject.Template.FindName("Placeholder", AssociatedObject) is not Label placeholder) return;
            if (AssociatedObject.Password.Length == 0)
                placeholder.Content = Placeholder;
            else
                placeholder.Content = "";
        }
    }
}