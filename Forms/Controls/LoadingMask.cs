using Backend.Database;
using Backend.Utils;
using FrontEnd.Dialogs;
using FrontEnd.ExtensionMethods;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a ContentControl used to display a loading process within a Window.
    /// </summary>
    /// <remarks>
    /// To use this class in your XAML:
    /// <code>
    /// &lt;!-- Import the namespace -->
    /// xmlns:fr="clr-namespace:FrontEnd.Forms;assembly=FrontEnd"
    /// 
    /// &lt;Window x:Class="MyApplication.View.LoadingForm"
    ///         ...
    ///         ResizeMode="NoResize"
    ///         WindowStartupLocation="CenterScreen"
    ///         Title="Welcome" Height="450" Width="450">
    ///     
    ///     &lt;fr:LoadingMask MainWindow="LoginForm">    
    ///         &lt;!-- Your content here -->
    ///     &lt;/fr:LoadingMask>
    /// &lt;/Window>
    /// </code>
    /// </remarks>
    public class LoadingMask : ContentControl, IAbstractControl
    {
        public Window? ParentWindow { get; private set; }

        public static readonly DependencyProperty FooterStringProperty = DependencyProperty.Register(nameof(FooterString), typeof(string), typeof(LoadingMask), new PropertyMetadata(string.Empty));
        public string FooterString
        {
            get => (string)GetValue(FooterStringProperty);
            set => SetValue(FooterStringProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(LoadingMask), new PropertyMetadata());
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty); 
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Gets or sets the name of the Window to open once the loading process is complete.
        /// <para><c>IMPORTANT:</c> The specified Window must be in a folder named 'View'.</para>
        /// </summary>
        public string MainWindow
        {
            private get => (string)GetValue(MainWindowProperty);
            set => SetValue(MainWindowProperty, value);
        }

        /// <summary>
        /// Identifies the MainWindow dependency property.
        /// </summary>
        public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(nameof(MainWindow), typeof(string), typeof(LoadingMask), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Initializes static members of the <see cref="LoadingMask"/> class.
        /// </summary>
        static LoadingMask() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadingMask"/> class.
        /// </summary>
        public LoadingMask()
        {
            Loaded += OnLoading;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Handles the Loaded event of the LoadingMask control.
        /// </summary>
        protected virtual async void OnLoading(object sender, RoutedEventArgs e)
        {
            string? assemblyName = Sys.AppName;
            string? Namespace = Sys.AppAssembly?.EntryPoint?.DeclaringType?.Namespace;
            Type? mainWinType = Type.GetType($"{Namespace}.View.{MainWindow}, {assemblyName}"); 
            if (mainWinType == null) 
            {
                Failure.Allert($"Could not find the Type of {MainWindow}","Ouch!");
                return;
            }
            
            await Task.Run(DatabaseManager.FetchData);
            Helper.GetActiveWindow()?.GoToWindow((Window?)Activator.CreateInstance(mainWinType));
        }

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

        public void OnClosed(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void DisposeEvents()
        {
            Loaded -= OnLoading;
            Unloaded -= OnUnloaded;
        }

        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

        public virtual void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }
}