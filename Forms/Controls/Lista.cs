using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Model;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends the <see cref="ListView"/> class and adds extra functionalities.
    /// Such as column's header, see the <see cref="Header"/> property and handles some user's inputs.
    /// The DataContext of this object is meant to be an instance of <see cref="IAbstractFormListController"/>.
    /// <para/>
    /// Its ItemsSource property should be a IEnumerable&lt;<see cref="AbstractModel"/>&gt; such as a <see cref="Backend.Source.RecordSource"/>
    /// </summary>
    public class Lista : ListView
    {
        /// <summary>
        /// Flag used to bypass the GotFocusEvent of a ListViewItem object.
        /// </summary>
        bool skipFocusEvent = false;

        #region Header
        /// <summary>
        /// Gets and Sets a <see cref="Grid"/> object which serves as column's header. See also the <see cref="HeaderFilter"/> class.
        /// </summary>
        public Grid Header
        {
            get => (Grid)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(Grid), typeof(Lista), new PropertyMetadata(OnHeaderPropertyChanged));

        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Style the columns with a default style.
            ResourceDictionary resourceDict = Helper.GetDictionary("controls");
            Style? labelStyle = null;

            if (resourceDict["ColumnStyle"] is Style columnStyle)
                labelStyle = new Style(targetType: typeof(Label), basedOn: columnStyle);

            Grid grid = (Grid)e.NewValue;
            grid.Resources.MergedDictionaries.Add(resourceDict);
            grid.Resources.Add(typeof(Label), new Style(typeof(Label), labelStyle));
            grid.Name = "listHeader"; //tells WPF that this is Grid is an Header for the Lista object, therefore it should always have an extra column which represent the RecordStatus object.
        }
        #endregion

        /// <summary>
        /// Gets the DataContext, which should be the Controller that is associated with this object.
        /// </summary>
        private IAbstractFormListController? Controller => (IAbstractFormListController)DataContext;

        private readonly ResourceDictionary styleDictionary = Helper.GetDictionary(nameof(Lista));

        /// <summary>
        /// olds a reference to the previously selected object.
        /// </summary>
        private object? OldSelection;
    
        public Lista()
        {
            Style listaItem = (Style)styleDictionary["ListaItemStyle"];
            listaItem.Setters.Add(new EventSetter
            {
                Event = ListViewItem.GotFocusEvent,
                Handler = new RoutedEventHandler(OnListViewItemGotFocus)
            });

            listaItem.Setters.Add(new EventSetter
            {
                Event = ListViewItem.LostKeyboardFocusEvent,
                Handler = new KeyboardFocusChangedEventHandler(ListViewItemKeyboardFocusChanged)
            });
            ItemContainerStyle = listaItem;
            Style = (Style)styleDictionary["ListaStyle"];
        }
        
        /// <summary>
        /// Handles the switching from one row to another by clicking on them.
        /// </summary>
        private void ListViewItemKeyboardFocusChanged(object sender, KeyboardFocusChangedEventArgs e)
        {
            // do not proceed if the newly Focused Element is not a FramewWork element, also VERY IMPORTANT if the OldFocus is an AbstractButton then exit.
            if (e.NewFocus is not FrameworkElement newlyFocusedElement || e.OldFocus is AbstractButton) return;
            bool isTabItem = (newlyFocusedElement is Frame frame && frame.Parent is TabItem); //check if the Lista is within a TabControl

            if (isTabItem || (newlyFocusedElement.DataContext is IAbstractFormController && newlyFocusedElement is not AbstractButton)) //Only if the DataContext of the List is an instance of IAbstractFormController we can continue. Also, very important the newlyFocusedElement must not be an AbstractButton
            {
                AbstractModel ListViewItemDataContext = (AbstractModel)((ListViewItem)sender).DataContext; //get the Record displayed by ListViewItem.
                OnListViewItemLostFocus(ListViewItemDataContext, isTabItem); // perform record's integrity checks before switching to a new record.
            }
            e.Handled = true;
        }

        /// <summary>
        /// override the default behaviour for OnSelectionChanged event.
        /// </summary>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            int lastRemovedIndex = e.RemovedItems.Count - 1;
            if (lastRemovedIndex >= 0)
                OldSelection = e.RemovedItems[lastRemovedIndex]; //hold a reference to the previously selected item as it might be needed to force the user to go back to that.

            int lastIndex = e.AddedItems.Count - 1;

            if (lastIndex >= 0 && e.AddedItems[lastIndex] is AbstractModel lastSelectedObject)
                ScrollIntoView(lastSelectedObject); //usefull for a large list where the user is selecting a record which is out of the current view. Scroll to it to make it visible
        }

        /// <summary>
        /// Performs some record's integrity checks before allowing the user to switch.
        /// </summary>
        /// <param name="record">The record to check</param>
        /// <param name="isTabItem">tells if the list is within a TabControl</param>
        /// <returns>true if the switch is allowed </returns>
        private bool OnListViewItemLostFocus(AbstractModel? record, bool isTabItem)
        {
            if (record is null) return true; //record is null, nothing to check, exit the method.
            if (!record.IsDirty && !record.IsNewRecord()) return true; //The user is on a record which has not been changed and it is not a new Record. No need for checking.

            //The user is attempting to switch to another Record without saving the changes to the previous record.
            DialogResult result = UnsavedDialog.Ask();

            if (result == DialogResult.Yes) //The user has decided to save the record before switching.
            {
                bool? updateResult = Controller?.PerformUpdate(); //perform the update.
                if (!updateResult!.Value) //The update failed due to conditions not met defined in the record AllowUpdate() method.
                {
                    Refocus(isTabItem); //force the user to stay on the Record and do not switch.
                    return false; // cannot switch.
                }
            }
            else //The user has decided NOT to save the record. rollback to the previous selecteditem.
            {
                if (record.IsDirty && !record.IsNewRecord()) //but if the user was updating a record which is not new then
                { 
                    Refocus(isTabItem); // force the user to stay on the record.
                    return false; // cannot switch.
                }
                AbstractModel? oldModel = (AbstractModel?)OldSelection; // get the previous selection.
                Controller?.CleanSource(); //remove new records from the source as it was decided to not save them.
                Controller?.GoAt(oldModel); // tell the controller to select the previous selection.
                return false; // cannot switch because the user decided to abort the new Record.
            }
            return true; //switch allowed.
        }

        /// <summary>
        /// This method is called within scenarios where the <see cref="Lista"/> is within a <see cref="TabControl"/> and the user attempted to switch tab without ensuring record integrity has been kept within the <see cref="Lista"/>
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SwitchTabBack()
        {
            TabItem? tab = Helper.FindAncestor<TabItem>(this) ?? throw new Exception($"Failed to get {nameof(TabItem)} object");
            TabControl? tabs = Helper.FindAncestor<TabControl>(tab) ?? throw new Exception($"Failed to get {nameof(TabControl)} object");
            tabs.SelectedItem = tab;
        }

        /// <summary>
        /// Reset the focus to the current selected ListViewItem by passing the <see cref="OnListViewItemGotFocus(object, RoutedEventArgs)"/>
        /// </summary>
        private void Refocus(bool isTabItem = false) 
        {
            if (isTabItem)
                SwitchTabBack();

            ListViewItem listViewItem = (ListViewItem)ItemContainerGenerator.ContainerFromItem(SelectedItem);
            skipFocusEvent = true; //no need to trigger the OnListViewItemGotFocus event.
            listViewItem.Focus(); //do the focus without triggering the OnListViewItemGotFocus event.
            skipFocusEvent = false; //bypass has finished, reset the flag to its default value.
        }

        /// <summary>
        /// When clicking on an Row.
        /// </summary>
        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
        {
            if (skipFocusEvent) return; //A focus has been requested by the event should not be fired.
            if (((ListViewItem)sender).DataContext is not AbstractModel record) return; //not and AbstractModel, therefore useless.
            if (!record.Equals(SelectedItem)) // The user is selecting a different item.
            {
                bool result = OnListViewItemLostFocus((AbstractModel)SelectedItem, false); //perform record's integrity checks.
                if (!result) return; // no need to continue, the OnListViewItemLostFocus has handled it.

                //The switch was succesful and mandatory condition for record's integrity have been met.
                Controller?.GoAt(record); // notify the controller that the SelectedItem has changed and therefore updates other linked controls such as RecordTracker
            }
        }

        ~Lista()
        {

        }
    }
}