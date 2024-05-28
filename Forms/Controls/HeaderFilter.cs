using FrontEnd.Controller;
using FrontEnd.FilterSource;
using FrontEnd.Source;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This class extends <see cref="AbstractControl"/> and displays a <see cref="SourceOption"/> object in a <see cref="Lista"/>'s header.
    /// <para/>
    /// For Example:
    /// <code>
    /// &lt;fr:Lista.Header>
    ///     ...
    ///     &lt;fr:HeaderFilter Grid.Column="4" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding GenderOptions}" Text="Gender"/>
    ///     
    ///     &lt;fr:HeaderFilter Grid.Column= "5" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding TitleOptions}" Text="Job Title"/>
    ///     
    ///     &lt;fr:HeaderFilter Grid.Column= "6" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding DepartmentOptions}" Text="Department"/>
    ///     ...
    ///&lt;/fr:Lista.Header>
    /// </code>
    /// This class works in conjunction with <seealso cref="IFilterOption"/>, <seealso cref="FilterOption"/>
    /// </summary>
    public class HeaderFilter : AbstractControl, IUIControl
    {
        private Button? PART_DropDownButton;
        private ListBox? PART_ListBox;

        private readonly Image Filter = new()
        {
            Source = Helper.LoadFromImages("filter") 
        };
        
        private readonly Image ClearFilter = new()
        {
            Source = Helper.LoadFromImages("clearfilter")
        };

        static HeaderFilter() => DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderFilter), new FrameworkPropertyMetadata(typeof(HeaderFilter)));

        #region IsWithinList
        /// <summary>
        /// This property works as a short-hand to set a Relative Source Binding between the FilterOption's DataContext and the <see cref="Lista"/>'s DataContext.
        /// </summary>
        public bool IsWithinList
        {
            private get => (bool)GetValue(IsWithinListProperty);
            set => SetValue(IsWithinListProperty, value);
        }

        public static readonly DependencyProperty IsWithinListProperty =
            DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(HeaderFilter), new PropertyMetadata(false, OnIsWithinListPropertyChanged));

        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isWithinList = (bool)e.NewValue;
            if (isWithinList)
                ((HeaderFilter)d).SetBinding(DataContextProperty, new Binding(nameof(DataContext))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else BindingOperations.ClearBinding(d, DataContextProperty);
        }
        #endregion

        private void ResetDropDownButtonAppereance()
        {
            if (PART_DropDownButton == null) throw new Exception("DropDownButton is null");
            PART_DropDownButton.Content = Filter;
            ToolTip = "Filter";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_DropDownButton = (Button)GetTemplateChild(nameof(PART_DropDownButton));
            PART_DropDownButton.Click += OnDropdownButtonClicked;
            ResetDropDownButtonAppereance();

            if (GetTemplateChild("PART_ClearButton") is Button clearButton)
                clearButton.Click += OnClearButtonClicked;

            PART_ListBox = (ListBox)GetTemplateChild(nameof(PART_ListBox));
            System.Diagnostics.PresentationTraceSources.SetTraceLevel(PART_ListBox.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
        }

        #region Events
        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach(var item in ItemsSource) 
                item.Deselect();

            ((IAbstractFormListController)DataContext).OnOptionFilter();
            IsOpen = false;
            ResetDropDownButtonAppereance();
        }
        private void OnDropdownButtonClicked(object sender, RoutedEventArgs e) => IsOpen = !IsOpen;            
        #endregion

        #region IsOpen
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(HeaderFilter), new PropertyMetadata(false));

        /// <summary>
        /// Gets and Sets a boolean indicating if the Popup is open or not/>
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IFilterOption>), typeof(HeaderFilter), new PropertyMetadata(ItemSourceChanged));

        /// <summary>
        /// Gets and sets the <see cref="SourceOption"/> object.
        /// <para/>
        /// see also <seealso cref="IFilterOption"/>, <seealso cref="FilterOption"/>, and <seealso cref="SourceOption"/>.
        /// </summary>
        public IEnumerable<IFilterOption> ItemsSource
        {
            get => (IEnumerable<IFilterOption>)GetValue(ItemsSourceProperty); 
            set => SetValue(ItemsSourceProperty, value); 
        }

        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HeaderFilter)d).BindEvents(e.NewValue);

        private void BindEvents(object new_source)
        {
            if (new_source == null) return;
            SourceOption source = (SourceOption)new_source;
            source.AddUIControlReference(this);
            foreach (IFilterOption option in source)
                option.OnSelectionChanged += OnOptionSelected;
        }

        private void OnOptionSelected(object? sender, EventArgs e)
        {
            if (PART_DropDownButton == null) throw new Exception("DropDownButton is null");
            PART_DropDownButton.Content = ClearFilter;
            ToolTip = "Clear Filter";
            ((IAbstractFormListController)DataContext).OnOptionFilter();
            if (!ItemsSource.Any(s=>s.IsSelected)) ResetDropDownButtonAppereance();
        }

        public void OnItemSourceUpdated(object[] args)
        {
            if (args.Length == 1) 
            {
                IFilterOption option = (IFilterOption) args[0];
                option.OnSelectionChanged += OnOptionSelected;
                return;
            }
            if (PART_ListBox == null) throw new Exception($"{nameof(PART_ListBox)} cannot be null.");
            DataTemplate tempDataTemplate = PART_ListBox.ItemTemplate;
            PART_ListBox.ItemTemplate = null;
            PART_ListBox.ItemTemplate = tempDataTemplate;
        }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(HeaderFilter), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets and Sets the string value to be displayed. This would usually be the <see cref="IFilterOption.Value"/> property.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) 
            {
                if (PART_DropDownButton != null)
                    PART_DropDownButton.Click -= OnDropdownButtonClicked;

                if (GetTemplateChild("PART_ClearButton") is Button clearButton)
                    clearButton.Click -= OnClearButtonClicked;

                foreach (IFilterOption option in ItemsSource)
                    option.OnSelectionChanged -= OnOptionSelected;

                ItemsSource = [];
            }
        }

        ~HeaderFilter() => Dispose(false);
    }

}