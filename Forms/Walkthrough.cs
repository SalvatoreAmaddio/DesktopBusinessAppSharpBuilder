using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace FrontEnd.Forms
{
    /// <summary>
    /// This ContentControl offers a way to display a step guide to help the User to perform a set of actions. 
    /// See also: <seealso cref="Pages"/>
    /// </summary>
    public class Walkthrough : ContentControl, IDisposable
    {
        protected bool _disposed = false;
        int Index { get; set; } = 0;
        Pages? Pages { get; set; }
        INotifyCollectionChanged? CollectionChangedNotifier { get; set; }
        ItemCollection? Collection { get; set; }
        Button? PART_PreviousButton { get; set; }
        Button? PART_NextButton { get; set; }
        static Walkthrough() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Walkthrough), new FrameworkPropertyMetadata(typeof(Walkthrough)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_PreviousButton = (Button)GetTemplateChild(nameof(PART_PreviousButton));
            PART_NextButton = (Button)GetTemplateChild(nameof(PART_NextButton));

            PART_NextButton.Click += OnNextButtonClicked;
            PART_PreviousButton.Click += OnPreviousButtonClicked;
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Collection == null) throw new Exception("");
            if (Index == 0) return;
            Index--;
            Content = Collection[Index];
        }
        private int lastIndex()
        {
            if (Collection == null) throw new Exception();
            return Collection.Count - 1;
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Collection == null) throw new Exception("");
            if (Index == lastIndex()) return;
            Index++;
            Content = Collection[Index];
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (newContent is Pages pages) 
            {
                Pages = pages;
                if (Pages.Items is INotifyCollectionChanged collectionChanged) 
                {
                    Collection = Pages.Items;
                    CollectionChangedNotifier = collectionChanged;
                    CollectionChangedNotifier.CollectionChanged += OnCollectionChanged;
                }
            }
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Unsubscribe from events
                if (CollectionChangedNotifier!=null)
                    CollectionChangedNotifier.CollectionChanged -= OnCollectionChanged;
                if (PART_NextButton != null)
                    PART_NextButton.Click -= OnNextButtonClicked;
                if (PART_PreviousButton != null)
                    PART_PreviousButton.Click -= OnPreviousButtonClicked;
            }

            _disposed = true;
        }

        ~Walkthrough() => Dispose(false);
    }

    /// <summary>
    /// This ItemsControl works together with <see cref="Walkthrough"/>
    /// </summary>
    public class Pages : ItemsControl
    {
        //private void NotifyCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (var newItem in e.NewItems)
        //        {
        //            // Handle the new item added
        //            MessageBox.Show($"Item added: {newItem}");
        //        }
        //    }
        //    else if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (var oldItem in e.OldItems)
        //        {
        //            // Handle the item removed
        //            MessageBox.Show($"Item removed: {oldItem}");
        //        }
        //    }
        //}

        static Pages() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Pages), new FrameworkPropertyMetadata(typeof(Pages)));
    }
}