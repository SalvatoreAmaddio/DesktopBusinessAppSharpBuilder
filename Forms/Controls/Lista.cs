using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Model;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FrontEnd.Source;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends the <see cref="ListView"/> class and adds extra functionalities,
    /// such as column's header, see the <see cref="Header"/> property, and handles some user's inputs.
    /// The DataContext of this object is meant to be an instance of <see cref="IAbstractFormListController"/>.
    /// <para/>
    /// Its ItemsSource property should be a <see cref="RecordSource{M}"/> object.
    /// </summary>
    public class Lista : ListView, IAbstractControl
    {
        /// <summary>
        /// Flag used to bypass the GotFocusEvent of a ListViewItem object.
        /// </summary>
        private bool _skipFocusEvent = false;

        /// <summary>
        /// Holds a reference to the previously selected object.
        /// </summary>
        private object? _oldSelection;

        /// <summary>
        /// Gets the DataContext, which should be the Controller that is associated with this object.
        /// </summary>
        private IAbstractFormListController? Controller => DataContext as IAbstractFormListController;

        private readonly ResourceDictionary styleDictionary = Helper.GetDictionary(nameof(Lista));
        public Window? ParentWindow {  get; private set; }

        #region Header
        /// <summary>
        /// Gets and sets a <see cref="Grid"/> object which serves as the column's header. See also the <see cref="HeaderFilter"/> class.
        /// </summary>
        public Grid Header
        {
            get => (Grid)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(Grid), typeof(Lista), new PropertyMetadata(OnHeaderPropertyChanged));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Lista"/> class.
        /// </summary>
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        /// <summary>
        /// Handles changes to the <see cref="Header"/> property.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
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
                ListViewItem? item = sender as ListViewItem;
                IAbstractModel? ListViewItemDataContext = item?.DataContext as IAbstractModel;//get the Record displayed by ListViewItem.
                OnListViewItemLostFocus(ListViewItemDataContext, isTabItem); // perform record's integrity checks before switching to a new record.
            }
            e.Handled = true;
        }

        /// <summary>
        /// Overrides the default behavior for OnSelectionChanged event.
        /// </summary>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            int lastRemovedIndex = e.RemovedItems.Count - 1;
            if (lastRemovedIndex >= 0)
                _oldSelection = e.RemovedItems[lastRemovedIndex]; //hold a reference to the previously selected item as it might be needed to force the user to go back to that.

            int lastIndex = e.AddedItems.Count - 1;

            if (lastIndex >= 0 && e.AddedItems[lastIndex] is IAbstractModel lastSelectedObject)
                ScrollIntoView(lastSelectedObject); //usefull for a large list where the user is selecting a record which is out of the current view. Scroll to it to make it visible
        }

        /// <summary>
        /// Attempts to save the current record.
        /// </summary>
        /// <param name="isTabItem">Indicates if the list is within a TabControl.</param>
        /// <returns>True if the save was successful, otherwise false.</returns>
        private bool AttemptSave(bool isTabItem)
        {
            bool? updateResult = Controller?.PerformUpdate(); //perform the update.
            if (!updateResult!.Value) //The update failed due to conditions not met defined in the record AllowUpdate() method.
            {
                Refocus(isTabItem); //force the user to stay on the Record and do not switch.
                return false; // cannot switch.
            }
            return true;
        }

        /// <summary>
        /// Performs some record's integrity checks before allowing the user to switch.
        /// </summary>
        /// <param name="record">The record to check.</param>
        /// <param name="isTabItem">Indicates if the list is within a TabControl.</param>
        /// <returns>True if the switch is allowed, otherwise false.</returns>
        private bool OnListViewItemLostFocus(IAbstractModel? record, bool isTabItem)
        {
            if (record is null) return true; //record is null, nothing to check, exit the method.
            if (!record.IsDirty && record.IsNewRecord()) return true; //The user is on a new record which has not been changed and it is not a new Record. No need for checking.
            if (!record.IsDirty && !record.IsNewRecord()) return true; //The user is on a record which has not been changed and it is not a new Record. No need for checking.
            if (Controller == null) throw new NullReferenceException();

            if (Controller.ReadOnly) // if the Controller is in ReadOnly, changes are not allowed and reverted. 
            {
                record.Undo();
                return true;
            }

            if (Controller.AllowAutoSave)
                return AttemptSave(isTabItem);

            //The user is attempting to switch to another Record without saving the changes to the previous record.
            DialogResult result = UnsavedDialog.Ask();

            if (result == DialogResult.Yes) //The user has decided to save the record before switching.
            {
                return AttemptSave(isTabItem);
            }
            else //The user has decided NOT to save the record. rollback to the previous selecteditem.
            {
                if (record.IsDirty && !record.IsNewRecord()) //but if the user was updating a record which is not new then
                { 
                    Refocus(isTabItem); // force the user to stay on the record.
                    return false; // cannot switch.
                }
                IAbstractModel? oldModel = (IAbstractModel?)_oldSelection; // get the previous selection.
                Controller?.CleanSource(); //remove new records from the source as it was decided to not save them.
                Controller?.GoAt(oldModel); // tell the controller to select the previous selection.
                return false; // cannot switch because the user decided to abort the new Record.
            }
        }

        /// <summary>
        /// This method is called within scenarios where the <see cref="Lista"/> is within a <see cref="TabControl"/> 
        /// and the user attempted to switch tabs without ensuring record integrity has been kept within the <see cref="Lista"/>.
        /// </summary>
        /// <exception cref="Exception">Thrown when the TabItem or TabControl cannot be found.</exception>
        private void SwitchTabBack()
        {
            TabItem? tab = Helper.FindAncestor<TabItem>(this) ?? throw new Exception($"Failed to get {nameof(TabItem)} object");
            TabControl? tabs = Helper.FindAncestor<TabControl>(tab) ?? throw new Exception($"Failed to get {nameof(TabControl)} object");
            tabs.SelectedItem = tab;
        }

        /// <summary>
        /// Resets the focus to the current selected ListViewItem by passing the <see cref="OnListViewItemGotFocus(object, RoutedEventArgs)"/>.
        /// </summary>
        /// <param name="isTabItem">Indicates if the list is within a TabControl.</param>
        private void Refocus(bool isTabItem = false) 
        {
            if (isTabItem)
                SwitchTabBack();

            ListViewItem listViewItem = (ListViewItem)ItemContainerGenerator.ContainerFromItem(SelectedItem);
            _skipFocusEvent = true; //no need to trigger the OnListViewItemGotFocus event.
            listViewItem.Focus(); //do the focus without triggering the OnListViewItemGotFocus event.
            _skipFocusEvent = false; //bypass has finished, reset the flag to its default value.
        }

        /// <summary>
        /// Handles the GotFocus event for a ListViewItem.
        /// </summary>
        private void OnListViewItemGotFocus(object sender, RoutedEventArgs e)
        {
            if (_skipFocusEvent) return; //A focus has been requested by the event should not be fired.
            if (((ListViewItem)sender).DataContext is not IAbstractModel record) return; //not and AbstractModel, therefore useless.
            if (!record.Equals(SelectedItem)) // The user is selecting a different item.
            {
                bool result = OnListViewItemLostFocus((IAbstractModel)SelectedItem, false); //perform record's integrity checks.
                if (!result) return; // no need to continue, the OnListViewItemLostFocus has handled it.

                //The switch was succesful and mandatory condition for record's integrity have been met.
                Controller?.GoAt(record); // notify the controller that the SelectedItem has changed and therefore updates other linked controls such as RecordTracker
            }
        }

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e) { }

        public void OnClosed(object? sender, EventArgs e) => Dispose();

        public void DisposeEvents()
        {
            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }

        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { }
        #endregion

        public void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }
}