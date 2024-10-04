using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using FrontEnd.Controller;
using System.ComponentModel;
using Backend.Utils;
using FrontEnd.ExtensionMethods;
using FrontEnd.Dialogs;
using FrontEnd.Forms;
using FrontEnd.Model;
using System.IO;

namespace FrontEnd.Utils
{
    /// <summary>
    /// Provides utility methods for common operations in a WPF application, including managing tab closing,
    /// loading resources, finding visual tree elements, interacting with windows, and handling images.
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Subscribes to the Closing event of the <see cref="Window"/> containing the <see cref="TabControl"/>.
        /// For each tab, the event invokes the <see cref="IAbstractFormController.OnWinClosing(object?, CancelEventArgs)"/> method
        /// to determine if the window can close or not.
        /// </summary>
        /// <param name="tabControl">The TabControl object.</param>
        public static void ManageTabClosing(TabControl tabControl)
        {
            //Get the Window
            Window? window = Window.GetWindow(tabControl);

            //subscribe the Closing Event
            window.Closing += (sender, e) =>
            {
                //get only the Tabs who have an instance of IAbstractFormController associated with.
                IEnumerable<TabItem> tabWithControllers = tabControl.Items.Cast<TabItem>().Where(item => item.Content is Frame frame && frame.Content is FrameworkElement element && element.DataContext is IAbstractFormController);

                foreach (TabItem item in tabWithControllers) //loop through each tab.
                {
                    FrameworkElement element = (FrameworkElement)((Frame)item.Content).Content; //extract the FrameworkElement.
                    IAbstractFormController controller = (IAbstractFormController)element.DataContext; //extract the DataContext from the FrameworkElement and cast it to IAbstractFormController.
                    controller.OnWinClosing(sender, e); //call the OnWindowClosing() method to ask the user on what to do about unsaved changes.
                    if (e.Cancel) //if the method changed the e.Cancel property to true, the user attempted to exit the program breaking data integrity rules.
                    {
                        tabControl.SelectedItem = item; //force the user to stay in the Tab.
                        return;//The window cannot close until the user fix the Data Integrity issues in the given tab.
                    }
                }
            };
        }

        #region Resource Dictionary
        /// <summary>
        /// Loads a resource string from the Strings.xaml dictionary.
        /// </summary>
        /// <param name="strKey">The key of the resource string.</param>
        /// <returns>The loaded string.</returns>
        public static string LoadFromStrings(string strKey)
        {
            string? str = GetDictionary("Strings")[strKey].ToString();
            return string.IsNullOrEmpty(str) ? string.Empty : str;
        }

        /// <summary>
        /// Loads a BitmapImage from the Images.xaml dictionary.
        /// </summary>
        /// <param name="imgKey">The key of the image resource.</param>
        /// <returns>The loaded BitmapImage.</returns>
        public static BitmapImage? LoadFromImages(string imgKey) => LoadImg(GetDictionary("Images")[imgKey]?.ToString());

        /// <summary>
        /// Retrieves a ResourceDictionary from the Framework Themes directory.
        /// </summary>
        /// <param name="name">The name of the dictionary.</param>
        /// <returns>The retrieved ResourceDictionary.</returns>
        /// <remarks>
        /// <c>ATTENTION:</c> it does not retrieve the ResourceDictionary from the current application Themes directory.
        /// </remarks>
        public static ResourceDictionary GetDictionary(string name) =>
        new()
        {
            Source = new Uri($"pack://application:,,,/FrontEnd;component/Themes/{name}.xaml")
        };
        #endregion

        /// <summary>
        /// Finds an ancestor of type T in the visual or logical tree.
        /// </summary>
        /// <typeparam name="T">The type of ancestor to find.</typeparam>
        /// <param name="current">The current DependencyObject to start searching from.</param>
        /// <returns>The found ancestor of type T, or null if not found.</returns>
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
        /// Finds the first child of type T in the visual tree under the given parent.
        /// </summary>
        /// <typeparam name="T">The type of child to find.</typeparam>
        /// <param name="parent">The parent DependencyObject to search under.</param>
        /// <returns>The found child of type T, or null if not found.</returns>
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
        /// Loads a BitmapImage from the specified path.
        /// </summary>
        /// <param name="path">The path to the image file.</param>
        /// <returns>The loaded BitmapImage, or null if loading fails.</returns>
        /// <exception cref="ArgumentException">Thrown when the path is null or empty.</exception>
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

        #region Window object.
        /// <summary>
        /// Retrieves the currently active window in the application.
        /// </summary>
        /// <returns>The active Window object.</returns>
        public static Window? GetActiveWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.IsActive)
                    return window;
            }
            return null;
        }

        /// <summary>
        /// Opens a modal dialog window with specified title, content, size, and resize mode.
        /// </summary>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="content">The content to display within the dialog window.</param>
        /// <param name="width">Optional width of the dialog window.</param>
        /// <param name="height">Optional height of the dialog window.</param>
        /// <param name="mode">Optional resize mode of the dialog window.</param>
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
        #endregion

        /// <summary>
        /// Logs out the current user and removes the saved login <see cref="Credential"/> from the local computer.
        /// </summary>
        /// <param name="loginForm">The window to open after logout, typically a login window.</param>
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

        public static string? PickPicture<M>(string fileName, string folderName, IAbstractFormController<M> controller, FilePickerCatch? filePicked) where M : IAbstractModel, new()
        {
            if (controller.CurrentRecord == null || filePicked == null) return null;

            if (filePicked.FileRemoved)
                return null;

            if (controller.CurrentRecord.IsDirty)
                if (!controller.PerformUpdate()) return null;

            if (string.IsNullOrEmpty(filePicked.FilePath)) return null;

            string folderPath = Path.Combine(Sys.AppPath(), folderName);

            Sys.CreateFolder(folderPath);

            FileTransfer fileTransfer = new()
            {
                SourceFilePath = filePicked.FilePath,
                DestinationFolder = folderPath,
                NewFileName = $"{fileName}.{filePicked.Extension}"
            };

            fileTransfer.Copy();

            return fileTransfer.NewFileName;
        }
    }
}