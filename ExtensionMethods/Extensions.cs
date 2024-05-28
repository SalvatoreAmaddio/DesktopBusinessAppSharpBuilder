using FrontEnd.Controller;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.ExtensionMethods
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method for Window objects.
        /// This method closes the current Window to open a new one.
        /// </summary>
        /// <param name="win"></param>
        /// <param name="newWin">The new window to open.</param>
        public static void GoToWindow(this Window win, Window? newWin)
        {
            win.Hide();
            newWin?.Show();
            win.Close();
        }

        /// <summary>
        /// Gets the Generic of the <see cref="CurrentTabController(TabControl)"/>.
        /// </summary>
        /// <returns>The Generic's <see cref="Type"/></returns>
        public static Type? GenericController(this TabControl tabControl) 
        {
            IAbstractFormController? controller = CurrentTabController(tabControl);
            Type? type = controller?.GetType().BaseType;
            if (type == null) return null;

            if (type.IsGenericType)
            {
                Type[] genericArguments = type.GetGenericArguments();
                if (genericArguments.Length > 0)
                   return genericArguments[0];
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="IAbstractFormController"/> associated with current <see cref="TabItem"/>.
        /// </summary>
        /// <returns>A <see cref="IAbstractFormController"/> object</returns>
        public static IAbstractFormController? CurrentTabController(this TabControl tabControl) 
        {
            Frame frame = (Frame) tabControl.SelectedContent;
            Page page = (Page)frame.Content;
            return page.DataContext as IAbstractFormController;
        }
    }
}
