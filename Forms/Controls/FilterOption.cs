using FrontEnd.Controller;
using FrontEnd.FilterSource;
using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Custom control that sets a List of options that can be used in a <see cref="Lista"/>'s header.
    /// <para/>
    /// For Example:
    /// <code>
    /// &lt;fr:Lista.Header>
    ///     ...
    ///     &lt;fr:FilterOption Grid.Column="4" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding GenderOptions}" Text="Gender"/>
    ///     
    ///     &lt;fr:FilterOption Grid.Column= "5" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding TitleOptions}" Text="Job Title"/>
    ///     
    ///     &lt;fr:FilterOption Grid.Column= "6" DataContext= "{Binding RelativeSource={RelativeSource AncestorType=fr:Lista}, Path=DataContext}" Controller= "{Binding}" ItemsSource="{Binding DepartmentOptions}" Text="Department"/>
    ///     ...
    ///&lt;/fr:Lista.Header>
    /// </code>
    /// This class works in conjunction with <seealso cref="IFilterOption"/>, <seealso cref="FilterOption"/>
    /// </summary>
    public class FilterOption : AbstractControl
    {
        private Button? DropDownButton;
        private readonly Image Filter = new()
        {
            Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/filter.png")
        };
        private readonly Image ClearFilter = new()
        {
            Source = Helper.LoadImg("pack://application:,,,/FrontEnd;component/Images/clear_filter.png")
        };

        static FilterOption() => DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterOption), new FrameworkPropertyMetadata(typeof(FilterOption)));

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
            DependencyProperty.Register(nameof(IsWithinList), typeof(bool), typeof(FilterOption), new PropertyMetadata(false, OnIsWithinListPropertyChanged));

        private static void OnIsWithinListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isWithinList = (bool)e.NewValue;
            if (isWithinList)
                ((FilterOption)d).SetBinding(DataContextProperty, new Binding(nameof(DataContext))
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Lista), 1)
                });
            else BindingOperations.ClearBinding(d, DataContextProperty);
        }
        #endregion

        private void ResetDropDownButtonAppereance()
        {
            if (DropDownButton == null) throw new Exception("DropDownButton is null");
            DropDownButton.Content = Filter;
            ToolTip = "Filter";
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DropDownButton = (Button)GetTemplateChild("PART_dropdown_button");
            DropDownButton.Click += OnDropdownButtonClicked;
            ResetDropDownButtonAppereance();

            if (GetTemplateChild("PART_clear_button") is Button clearButton)
                clearButton.Click += OnClearButtonClicked;
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
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(FilterOption), new PropertyMetadata(false));

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
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<IFilterOption>), typeof(FilterOption), new PropertyMetadata(ItemSourceChanged));

        /// <summary>
        /// Gets and sets the List of options.
        /// <para/>
        /// see also <seealso cref="IFilterOption"/>, <seealso cref="FilterOption"/>, and <seealso cref="SourceOption"/>.
        /// </summary>
        public IEnumerable<IFilterOption> ItemsSource
        {
            get => (IEnumerable<IFilterOption>)GetValue(ItemsSourceProperty); 
            set => SetValue(ItemsSourceProperty, value); 
        }

        private static void ItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FilterOption)d).BindEvents(e.NewValue);

        private void BindEvents(object new_source)
        {
            IEnumerable<IFilterOption> source = (IEnumerable<IFilterOption>)new_source;
            foreach (IFilterOption option in source)
                option.OnSelectionChanged += OnOptionSelected;
        }

        private void OnOptionSelected(object? sender, EventArgs e)
        {
            if (DropDownButton == null) throw new Exception("DropDownButton is null");
            DropDownButton.Content = ClearFilter;
            ToolTip = "Clear Filter";
            ((IAbstractFormListController)DataContext).OnOptionFilter();
            if (!ItemsSource.Any(s=>s.IsSelected)) ResetDropDownButtonAppereance();
        }
        #endregion

        #region Text
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(FilterOption), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets and Sets the string value to be displayed. This would usually be the <see cref="IFilterOption.Value"/> property.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        #endregion
    }
}