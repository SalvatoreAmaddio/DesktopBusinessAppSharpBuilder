using Backend.Source;
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
    /// This class extends <see cref="ComboBox"/> and adds some extra functionalities for dealing with the SelectedItem property.
    /// Furthermore, its ItemsSource is meant to be a <see cref="Backend.Source.RecordSource"/> object.
    /// Also, this class implements <see cref="IUIControl"/> to provide a better communication between the <see cref="RecordSource"/> and the Combo.
    /// </summary>
    public partial class Combo : ComboBox, IUIControl
    {
        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(Combo));

        public Combo() 
        {
            ItemContainerStyle = (Style)resourceDict["ComboItemContainerStyle"];
            Style = (Style)resourceDict["ComboStyle"];
        }

        public AbstractModel? ParentModel => DataContext as AbstractModel;
        
        /// <summary>
        /// Adjust the <see cref="ComboBox.Text"/> property to relect the Selected Item.
        /// </summary>
        /// <param name="model">The selected item whose ToString() method should be displayed</param>
        /// <returns>A Task</returns>
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
        
        private void ResetTemplate() 
        {
            ControlTemplate temp = Template;
            Template = null;
            Template = temp;
        }

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

        #region Placeholder
        /// <summary>
        /// Gets and sets the Placeholder
        /// </summary>
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty));
        #endregion

        #region ControllerSource
        /// <summary>
        /// This property works as a short-hand to set a Relative Source Binding between the combo's ItemSource and a <see cref="Lista"/>'s DataContext's IEnumerable Property.
        /// </summary>
        public string ControllerRecordSource
        {
            private get => (string)GetValue(ControllerRecordSourceProperty);
            set => SetValue(ControllerRecordSourceProperty, value);
        }

        public static readonly DependencyProperty ControllerRecordSourceProperty = DependencyProperty.Register(nameof(ControllerRecordSource), typeof(string), typeof(Combo), new PropertyMetadata(string.Empty, OnControllerRecordSourcePropertyChanged));
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
        #endregion

        /// <summary>
        /// This method has been overriden to associate this object to the RecordSource.
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null && newValue is IUISource source)
                source.AddUIControlReference(this);
        }

        public async void OnItemSourceUpdated(object[] args)
        {
            ResetTemplate();
            await AdjustText(null);
        }

        ~Combo()
        {

        }
    }
}