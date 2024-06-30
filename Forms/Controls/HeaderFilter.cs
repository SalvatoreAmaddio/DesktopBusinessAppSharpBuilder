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
    /// Represents a UI control that extends <see cref="AbstractControl"/> and displays a <see cref="SourceOption"/> object within a <see cref="Lista"/>'s header.
    /// <para/>
    /// Example usage within XAML:
    /// <code>
    /// &lt;fr:Lista.Header>
    ///     &lt;fr:HeaderFilter Grid.Column="4" DataContext="{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller="{Binding}" ItemsSource="{Binding GenderOptions}" Text="Gender"/>
    ///     &lt;fr:HeaderFilter Grid.Column="5" DataContext="{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller="{Binding}" ItemsSource="{Binding TitleOptions}" Text="Job Title"/>
    ///     &lt;fr:HeaderFilter Grid.Column="6" DataContext="{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller="{Binding}" ItemsSource="{Binding DepartmentOptions}" Text="Department"/>
    /// &lt;/fr:Lista.Header>
    /// </code>
    /// This class works in conjunction with <seealso cref="IFilterOption"/>, <seealso cref="FilterOption"/>, and <seealso cref="SourceOption"/>.
    /// </summary>
    public class HeaderFilter : AbstractControl, IUIControl
    {
        private Button? PART_ClearButton;
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

        #region IsWithinList
        /// <summary>
        /// Gets or sets a value indicating whether the filter is within a list.
        /// This property establishes a relative source binding between the FilterOption's DataContext and the <see cref="Lista"/>'s DataContext.
        /// </summary>
        public bool IsWithinList
        {
            private get => (bool)GetValue(IsWithinListProperty);
            set => SetValue(IsWithinListProperty, value);
        }

        public static readonly DependencyProperty IsWithinListProperty =
        DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(HeaderFilter), new PropertyMetadata(false, OnIsWithinListPropertyChanged));
        #endregion
        
        #region IsOpen
        /// <summary>
        /// Gets or sets a value indicating whether the popup is open.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(HeaderFilter), new PropertyMetadata(false));
        #endregion

        #region ItemsSource
        /// <summary>
        /// Gets or sets the collection of filter options.
        /// </summary>
        public IEnumerable<IFilterOption> ItemsSource
        {
            get => (IEnumerable<IFilterOption>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IFilterOption>), typeof(HeaderFilter), new PropertyMetadata(ItemSourceChanged));
        #endregion

        #region Text
        /// <summary>
        /// Gets and Sets the string value to be displayed. This would usually be the <see cref="IFilterOption.Value"/> property.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(HeaderFilter), new PropertyMetadata(string.Empty));
        #endregion

        static HeaderFilter() => DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderFilter), new FrameworkPropertyMetadata(typeof(HeaderFilter)));
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_DropDownButton = (Button)GetTemplateChild(nameof(PART_DropDownButton));
            PART_DropDownButton.Click += OnDropdownButtonClicked;
            ResetDropDownButtonAppereance();

            PART_ClearButton = (Button?)GetTemplateChild(nameof(PART_ClearButton));
            if (PART_ClearButton != null)
                PART_ClearButton.Click += OnClearButtonClicked;

            PART_ListBox = (ListBox)GetTemplateChild(nameof(PART_ListBox));
            System.Diagnostics.PresentationTraceSources.SetTraceLevel(PART_ListBox.ItemContainerGenerator, System.Diagnostics.PresentationTraceLevel.High);
        }

        #region ItemsSource changed
        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((HeaderFilter)d).BindEvents(e.OldValue, e.NewValue);

        private void BindEvents(object old_source, object new_source)
        {
            if (old_source != null)
                UnsubscribeEvents((SourceOption)old_source);

            if (new_source == null) return;

            SourceOption? source = new_source as SourceOption;
            source?.AddUIControlReference(this);

            if (source is not null)
            {
                foreach (IFilterOption option in source)
                    option.OnSelectionChanged += OnOptionSelected;

                if (ItemsSource.Any(s => s.IsSelected))
                {
                    if (PART_DropDownButton == null) throw new NullReferenceException("DropDownButton is null");
                    PART_DropDownButton.Content = ClearFilter;
                }
            }
        }

        private void UnsubscribeEvents(SourceOption source)
        {
            foreach (IFilterOption option in source)
                option.OnSelectionChanged -= OnOptionSelected;
        }

        public void OnItemSourceUpdated(object[] args)
        {
            if (args.Length == 1) // NEW OPTION WAS ADDED
            {
                if (args[0] is string) //UPDATE STRING
                {
                    foreach (IFilterOption option in ItemsSource)
                    {
                        option.OnSelectionChanged += OnOptionSelected;
                    }
                }
                else
                {
                    IFilterOption option = (IFilterOption)args[0];
                    option.OnSelectionChanged += OnOptionSelected;
                    return;
                }
            }

            if (PART_ListBox == null) throw new Exception($"{nameof(PART_ListBox)} cannot be null.");
            DataTemplate tempDataTemplate = PART_ListBox.ItemTemplate;
            PART_ListBox.ItemTemplate = null;
            PART_ListBox.ItemTemplate = tempDataTemplate;
        }
        #endregion

        #region IsWithinList changed
        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var headerFilter = (HeaderFilter)d;
            //handle LazyLoading drawbacks
            if (headerFilter.IsLoaded)
                headerFilter.UpdateDataContextBinding();
            else
                headerFilter.Loaded += OnLoaded;
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            HeaderFilter control = (HeaderFilter)sender;
            control.UpdateDataContextBinding();
        }

        private void UpdateDataContextBinding()
        {
            if (IsWithinList)
            {
                SetBinding(DataContextProperty, new Binding(nameof(DataContext))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            }
            else
                BindingOperations.ClearBinding(this, DataContextProperty);
        }
        #endregion

        #region Event Subscriptions
        private void OnClearButtonClicked(object sender, RoutedEventArgs e)
        {
            foreach (var item in ItemsSource)
                item.Deselect();

            ((IAbstractFormListController)DataContext).OnOptionFilterClicked(new());
            IsOpen = false;
            ResetDropDownButtonAppereance();
        }
        private void OnDropdownButtonClicked(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }
        public override void OnClosed(object? sender, EventArgs e) => Dispose();
        private void OnOptionSelected(object? sender, EventArgs e)
        {
            if (PART_DropDownButton == null) throw new NullReferenceException("DropDownButton is null");
            PART_DropDownButton.Content = ClearFilter;
            ToolTip = "Clear Filter";
            ((IAbstractFormListController)DataContext).OnOptionFilterClicked(new());
            if (!ItemsSource.Any(s => s.IsSelected)) ResetDropDownButtonAppereance();
        }
        #endregion

        private void ResetDropDownButtonAppereance()
        {
            if (PART_DropDownButton == null) throw new NullReferenceException("DropDownButton is null");
            PART_DropDownButton.Content = Filter;
            ToolTip = "Filter";
        }
        private static void DisposeSource(SourceOption source)
        {
            foreach (IFilterOption option in source)
                option.Dispose();

            source.Dispose();
        }

        #region IAbstractControl
        public override void OnUnloaded(object sender, RoutedEventArgs e) { }
        public override void DisposeEvents()
        {
            base.DisposeEvents();
            if (PART_DropDownButton != null)
                PART_DropDownButton.Click -= OnDropdownButtonClicked;

            if (PART_ClearButton != null)
                PART_ClearButton.Click -= OnClearButtonClicked;

            DisposeSource((SourceOption)ItemsSource);

            Loaded -= OnLoaded;
        }
        #endregion
    }
}