using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// Represents a ContentControl that displays a step-by-step guide to help the user perform a set of actions.
    /// </summary>
    /// <seealso cref="Pages"/>
    public class Walkthrough : ContentControl, IAbstractControl
    {
        #region Properties
        /// <summary>
        /// Gets or sets the current index of the step being displayed.
        /// </summary>
        private int Index { get; set; } = 0;

        /// <summary>
        /// Gets the index of the last step in the walkthrough.
        /// </summary>
        private int LastIndex => (Collection == null) ? throw new NullReferenceException() : Collection.Count - 1;

        /// <summary>
        /// Gets or sets the <see cref="Pages"/> object that contains the steps.
        /// </summary>
        /// <remarks>
        /// This property is set in <see cref="OnContentChanged(object, object)"/>
        /// </remarks>
        private Pages? Pages { get; set; }

        /// <summary>
        /// Gets or sets the collection changed notifier for the pages.
        /// </summary>
        /// <remarks>
        /// This property is set in <see cref="OnContentChanged(object, object)"/>
        /// </remarks>
        private INotifyCollectionChanged? CollectionChangedNotifier { get; set; }

        /// <summary>
        /// Gets or sets the item collection that contains the steps.
        /// </summary>
        private ItemCollection? Collection { get; set; }
        private Button? PART_PreviousButton { get; set; }
        private Button? PART_NextButton { get; set; }
        public Window? ParentWindow { get; private set; }
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="Walkthrough"/> class.
        /// Overrides the default style key property metadata for the <see cref="Walkthrough"/> class.
        /// </summary>
        static Walkthrough() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Walkthrough), new FrameworkPropertyMetadata(typeof(Walkthrough)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_PreviousButton = (Button)GetTemplateChild(nameof(PART_PreviousButton));
            PART_NextButton = (Button)GetTemplateChild(nameof(PART_NextButton));

            PART_NextButton.Click += OnNextButtonClicked;
            PART_PreviousButton.Click += OnPreviousButtonClicked;

            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        /// <summary>
        /// Handles changes to the content of the <see cref="Walkthrough"/>.
        /// Sets up or removes the <see cref="CollectionChangedNotifier"/> property as needed. It also sets the <see cref="Pages"/> property.
        /// </summary>
        /// <param name="oldContent">The previous content of the control.</param>
        /// <param name="newContent">The new content of the control.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (oldContent is Pages oldPages) 
            {
                if (oldPages.Items is INotifyCollectionChanged collectionChanged)
                {
                    CollectionChangedNotifier = collectionChanged;
                    CollectionChangedNotifier.CollectionChanged -= OnCollectionChanged;
                }
            }

            if (newContent is Pages newPages)
            {
                Pages = newPages;
                if (Pages.Items is INotifyCollectionChanged collectionChanged)
                {
                    Collection = Pages.Items;
                    CollectionChangedNotifier = collectionChanged;
                    CollectionChangedNotifier.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        #region Event Subscriptions
        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Collection == null) throw new Exception("");
            if (Index == 0) return;
            Index--;
            Content = Collection[Index];
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Collection == null) throw new Exception("");
            if (Index == LastIndex) return;
            Index++;
            Content = Collection[Index];
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {

            if (Collection == null) throw new Exception("");
            try
            {
                Content = Collection[Index];
            }
            catch 
            {
                throw new Exception("Something went wrong");
            }
        }
        #endregion

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e) { }
        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) { }
        public void OnClosed(object? sender, EventArgs e) => Dispose();
        public void DisposeEvents()
        {
            if (CollectionChangedNotifier != null)
                CollectionChangedNotifier.CollectionChanged -= OnCollectionChanged;
            if (PART_NextButton != null)
                PART_NextButton.Click -= OnNextButtonClicked;
            if (PART_PreviousButton != null)
                PART_PreviousButton.Click -= OnPreviousButtonClicked;
            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }
        #endregion

        public void Dispose()
        {
            DisposeEvents();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Represents an ItemsControl that is used int the <see cref="Walkthrough"/> control to display a series of pages or steps.
    /// </summary>
    public class Pages : ItemsControl
    {
        /// <summary>
        /// Initializes static members of the <see cref="Pages"/> class.
        /// Overrides the default style key property metadata for the <see cref="Pages"/> class.
        /// </summary>
        static Pages() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Pages), new FrameworkPropertyMetadata(typeof(Pages)));
    }
}