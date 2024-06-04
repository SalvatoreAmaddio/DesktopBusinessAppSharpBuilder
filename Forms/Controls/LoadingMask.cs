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
    /// This class instantiate a ContentControl for a Window object to show a loading process.
    /// For Example, in your xaml:
    /// <code>
    /// &lt;!--IMPORT THE NAMESPACE-->
    /// xmlns:fr="clr-namespace:FrontEnd.Forms;assembly=FrontEnd"
    /// 
    /// &lt;Window x:Class="MyApplication.View.LoadingForm"
    ///     ....
    ///     ResizeMode="NoResize"
    ///     WindowStartupLocation="CenterScreen"
    ///     Title="Welcome" Height="450" Width="450">
    ///     
    ///     &lt;fr:LoadingMask MainWindow="LoginForm">    
    ///         &lt;!--YOUR CONTENT HERE-->
    ///     &lt;/fr:LoadingMask>
    /// &lt;/Window>
    /// </code>
    /// see also the <see cref="MainWindow"/> property.
    /// </summary>
    public class LoadingMask : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        static LoadingMask() => DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingMask), new FrameworkPropertyMetadata(typeof(LoadingMask)));

        /// <summary>
        /// Sets the name of the Window to open once the loading process has completed. <para/>
        /// <c>IMPORTANT:</c> the Window to open must be in a folder named 'View'.
        /// </summary>
        public string MainWindow
        {
            private get => (string)GetValue(MainWindowProperty);
            set => SetValue(MainWindowProperty, value);
        }

        public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(nameof(MainWindow), typeof(string), typeof(LoadingMask), new PropertyMetadata(string.Empty));

        public LoadingMask() 
        {
            Loaded += OnLoading;
            Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

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
                Loaded -= OnLoading;
                Unloaded -= OnUnloaded;
            }

            _disposed = true;
        }

        ~LoadingMask() => Dispose(false);
    }
}
