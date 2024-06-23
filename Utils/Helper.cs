using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using FrontEnd.Controller;
using System.ComponentModel;
using Backend.Utils;
using FrontEnd.ExtensionMethods;
using FrontEnd.Dialogs;
using System.IO;

namespace FrontEnd.Utils
{
    public class Helper
    {
        /// <summary>
        /// Subscribes the Closing Event of the <see cref="Window"/> containg the <see cref="TabControl"/>. <para/>
        /// For each Tab, the event calls the <see cref="IAbstractFormController.OnWindowClosing(object?, CancelEventArgs)"/> method
        /// which determines if the Window can close or not.
        /// </summary>
        /// <param name="tabControl">A TabControl object</param>
        /// <exception cref="Exception"></exception>
        public static void ManageTabClosing(TabControl tabControl) 
        {
            //Get the Window
            Window? window = FindAncestor<Window>(tabControl) ?? throw new Exception("Failed to find the window"); //this Exception should not happen.

            //subscribe the Closing Event
            window.Closing += (sender, e) =>
            {
                //get only the Tabs who have an instance of IAbstractFormController associated with.
                IEnumerable<TabItem> tabWithControllers = tabControl.Items.Cast<TabItem>().Where(item => item.Content is Frame frame && frame.Content is FrameworkElement element && element.DataContext is IAbstractFormController);

                foreach (TabItem item in tabWithControllers) //loop through each tab.
                {
                    FrameworkElement element = (FrameworkElement)((Frame)item.Content).Content; //extract the FrameworkElement.
                    IAbstractFormController controller = (IAbstractFormController)element.DataContext; //extract the DataContext from the FrameworkElement and cast it to IAbstractFormController.
                    controller.OnWindowClosing(sender, e); //call the OnWindowClosing() method to ask the user on what to do about unsaved changes.
                    if (e.Cancel) //if the method changed the e.Cancel property to true, the user attempted to exit the program breaking data integrity rules.
                    {
                        tabControl.SelectedItem = item; //force the user to stay in the Tab.
                        return;//The window cannot close until the user fix the Data Integrity issues in the given tab.
                    }
                }
            };
        }

        /// <summary>
        /// Load a Resource string from the Strings.xaml dictionary.
        /// </summary>
        /// <param name="strKey">The resource's key</param>
        /// <returns>A string</returns>
        public static string? LoadFromStrings(string strKey) =>
        GetDictionary("Strings")[strKey]?.ToString();

        /// <summary>
        /// Load a BitmapImage from the Images.xaml dictionary.
        /// </summary>
        /// <param name="imgKey">The resource's key</param>
        /// <returns>A BitmapImage</returns>
        public static BitmapImage LoadFromImages(string imgKey) =>
        LoadImg(GetDictionary("Images")[imgKey]?.ToString());

        /// <summary>
        /// Gets a dictionary from the Themes directory.
        /// </summary>
        /// <param name="name">The name of the dictionary</param>
        /// <returns>A ResourceDictionary</returns>
        public static ResourceDictionary GetDictionary(string name) =>
        new() 
        {
            Source = new Uri($"pack://application:,,,/FrontEnd;component/Themes/{name}.xaml")
        };

        public static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                    return (T)current;
                    // Check visual tree
                DependencyObject parent = VisualTreeHelper.GetParent(current);

                if (parent == null)
                    parent = LogicalTreeHelper.GetParent(current);
                
                current = parent;
            }
            return null;
            
        }

        /// <summary>
        /// Gets the active window.
        /// </summary>
        /// <returns>A Window</returns>
        public static Window? GetActiveWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                    return window;
            }
            return null;
        }

        public static T? FindFirstChildOfType<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null) return null;
            if (parent is T)
                return parent as T;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T? result = FindFirstChildOfType<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Loads a BitmapImage which can be used as a Source in a Image Control.
        /// </summary>
        /// <param name="path">The path to the image</param>
        /// <returns>A BitmapImage</returns>
        /// <exception cref="ArgumentException">Path cannot be null</exception>
        public static BitmapImage? LoadImg(string? path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            try 
            {
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensure the file handle is not locked
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                return bitmap;
            }
            catch 
            { 
                return null;
            }

        }

        public static void OpenWindowDialog(string title, object content, double? width = null, double? height = null, ResizeMode mode = ResizeMode.CanResize) 
        {
            Window window = new()
            {
                Title = title,
                Content = content,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = mode,
            };

            if (width != null) window.Width = width.Value;
            if (height != null) window.Height = height.Value;
                        
            window.ShowDialog();
        }

        /// <summary>
        /// It logs the user out and remove the login <see cref="Credential"/> saved in the local computer.
        /// </summary>
        /// <param name="loginForm">The window to open once logged out occured, usually a Login Window</param>
        public static void Logout(Window loginForm)
        {
            DialogResult result = ConfirmDialog.Ask("Are you sure you want to logout?");
            if (result == DialogResult.No) 
            {
                loginForm.Close(); //Windows in WPF stay in memory even if they are not shown.
                return;
            }
            CurrentUser.Logout();
            GetActiveWindow()?.GoToWindow(loginForm);
        }

    }
}