using Backend.Database;
using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.ExtensionMethods
{
    /// <summary>
    /// Provides extension methods for various WPF framework classes, enhancing functionality related to window management,
    /// controller association, tab control navigation, and application lifecycle management.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Extension method for <see cref="Window"/> objects.
        /// Closes the current window, hides it, and shows a new window.
        /// </summary>
        /// <param name="win">The current window to close.</param>
        /// <param name="newWin">The new window to open.</param>
        public static void GoToWindow(this Window win, Window? newWin)
        {
            win.Hide();
            newWin?.Show();
            win.Close();
        }

        /// <summary>
        /// Registers a method to dispose of resources when the application exits.
        /// </summary>
        /// <param name="app">The current application instance.</param>
        public static void DisposeOnExit(this Application app) => app.Exit += OnExit;

        private static void OnExit(object sender, ExitEventArgs e)
        {
            Application application = (Application)sender;
            DatabaseManager.Dispose();
            application.Exit -= OnExit;
        }

        #region Controller
        /// <summary>
        /// Sets the data context and user interface reference for a <see cref="Window"/>.
        /// </summary>
        /// <param name="win">The window to set the controller for.</param>
        /// <param name="controller">The controller implementing <see cref="IAbstractFormController"/>.</param>
        public static void SetController(this Window win, IAbstractFormController controller)
        {
            win.DataContext = controller;
            controller.UI = win;
        }

        /// <summary>
        /// Sets the data context and user interface reference for a <see cref="Page"/>.
        /// </summary>
        /// <param name="page">The page to set the controller for.</param>
        /// <param name="controller">The controller implementing <see cref="IAbstractFormController"/>.</param>
        public static void SetController(this Page page, IAbstractFormController controller)
        {
            page.DataContext = controller;
            controller.UI = page;
        }

        /// <summary>
        /// Gets the controller of type C associated with a <see cref="Window"/>.
        /// </summary>
        /// <typeparam name="C">The type of controller implementing <see cref="IAbstractFormController"/>.</typeparam>
        /// <param name="win">The window to retrieve the controller from.</param>
        /// <returns>The controller of type C, or default(C) if not found or cast fails.</returns>
        public static C? GetController<C>(this Window win) where C : IAbstractFormController
        {
            try 
            {
                return (C)win.DataContext;
            }
            catch 
            {
                return default;
            }
        }

        /// <summary>
        /// Gets the controller of type C associated with a <see cref="Page"/>.
        /// </summary>
        /// <typeparam name="C">The type of controller implementing <see cref="IAbstractFormController"/>.</typeparam>
        /// <param name="page">The page to retrieve the controller from.</param>
        /// <returns>The controller of type C, or default(C) if not found or cast fails.</returns>
        public static C? GetController<C>(this Page page) where C : IAbstractFormController
        {
            try
            {
                return (C)page.DataContext;
            }
            catch 
            {
                return default;
            }
        }

        /// <summary>
        /// Retrieves the <see cref="IAbstractFormController"/> associated with the currently selected <see cref="TabItem"/> in a <see cref="TabControl"/>.
        /// </summary>
        /// <param name="tabControl">The TabControl containing the TabItem.</param>
        /// <returns>The <see cref="IAbstractFormController"/> associated with the current TabItem, or null if none found.</returns>
        public static IAbstractFormController? CurrentTabController(this TabControl tabControl)
        {
            Frame frame = (Frame)tabControl.SelectedContent;
            Page page = (Page)frame.Content;
            return page.DataContext as IAbstractFormController;
        }
        #endregion

    }
}