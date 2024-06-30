using FrontEnd.Model;
using FrontEnd.Utils;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FrontEnd.Source;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Extends <see cref="ComboBox"/> and adds extra functionalities for dealing with the SelectedItem property.
    /// The ItemsSource is intended to be an <see cref="IUISource"/> object.
    /// This class implements <see cref="IUIControl"/> to facilitate better communication between the <see cref="DataSource"/> and the ComboBox.
    /// </summary>
    public partial class Combo : ComboBox, IUIControl
    {
        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(Combo));

        /// <summary>
        /// Gets the parent model from the DataContext, if it implements <see cref="IAbstractModel"/>.
        /// </summary>
        private IAbstractModel? ParentModel => DataContext as IAbstractModel;

        #region Placeholder
        /// <summary>
        /// Gets or sets the placeholder text for the ComboBox.
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Placeholder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaceholderProperty =
        DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty));
        #endregion

        #region ControllerSource
        /// <summary>
        /// Gets or sets the controller record source.
        /// This property sets up a relative source binding between the ComboBox's ItemsSource and a <see cref="Lista"/>'s DataContext IEnumerable property.
        /// </summary>
        public string ControllerRecordSource
        {
            private get => (string)GetValue(ControllerRecordSourceProperty);
            set => SetValue(ControllerRecordSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ControllerRecordSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControllerRecordSourceProperty = DependencyProperty.Register(nameof(ControllerRecordSource), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty, OnControllerRecordSourcePropertyChanged));
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Combo"/> class.
        /// Sets the item container style and the ComboBox style from the resource dictionary.
        /// </summary>
        public Combo() 
        {
            ItemContainerStyle = (Style)resourceDict["ComboItemContainerStyle"];
            Style = (Style)resourceDict["ComboStyle"];
        }

        /// <summary>
        /// Adjusts the <see cref="ComboBox.Text"/> property to reflect the selected item.
        /// </summary>
        /// <param name="model">The selected item whose <c>ToString()</c> method should be displayed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task AdjustText(object? model)
        {
            model ??= SelectedItem;
            object? item = null;
            foreach (object record in ItemsSource)
            {
                if (record.Equals(model))
                {
                    item = record;
                    break;
                }
            }
            Text = item?.ToString();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the selection changed event of the ComboBox.
        /// Adjusts the text to reflect the selected item.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override async void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            try
            {
                object? model = e.AddedItems[e.AddedItems.Count - 1];
                await AdjustText(model);
            }
            catch { }
        }

        /// <summary>
        /// Handles changes to the ItemsSource property.
        /// Associates this control with the new <see cref="IUISource"/>.
        /// </summary>
        /// <param name="oldValue">The old value of the ItemsSource property.</param>
        /// <param name="newValue">The new value of the ItemsSource property.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null && newValue is IUISource source)
                source.AddUIControlReference(this);
        }

        /// <summary>
        /// Handles changes to the <see cref="ControllerRecordSource"/> property.
        /// </summary>
        private static void OnControllerRecordSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isEmpty = string.IsNullOrEmpty(e.NewValue.ToString());
            Combo control = (Combo)d;
            if (!isEmpty)
                control.SetBinding(ItemsSourceProperty, new Binding($"{nameof(DataContext)}.{e.NewValue}")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else control.ClearValue(ItemsSourceProperty);
        }

        /// <summary>
        /// Resets the control template of the ComboBox.
        /// </summary>
        private void ResetTemplate()
        {
            ControlTemplate temp = Template;
            Template = null;
            Template = temp;
        }

        #region UIControl
        public async void OnItemSourceUpdated(object[] args)
        {
            ResetTemplate();
            await AdjustText(null);
        }
        #endregion
    }
}