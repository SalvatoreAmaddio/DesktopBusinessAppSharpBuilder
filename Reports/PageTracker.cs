using FrontEnd.Forms.FormComponents;
using System.Windows;

namespace FrontEnd.Reports
{
    public class PageTracker : RecordTracker
    {
        private int Index { get; set; } = -1;
        private List<ReportPage> SourceList => ItemsSource.ToList();

        #region SelectedPage
        public static readonly DependencyProperty SelectedPageProperty =
         DependencyProperty.Register(nameof(SelectedPage), typeof(ReportPage), typeof(PageTracker), new PropertyMetadata(OnSelectionChanged));

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageTracker control = (PageTracker)d;
            if (control.ItemsSource == null) return;
            ReportPage page = (ReportPage)e.NewValue;
            control.Index  = control.SourceList.IndexOf(page);
            if (control.Index < 0) throw new Exception("Page not found");
            control.Records = $"Page {control.Index + 1} of {control.ItemsSource.Count()}";
        }

        public ReportPage SelectedPage
        {
            get => (ReportPage)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly DependencyProperty ItemsSourceProperty =
         DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ReportPage>), typeof(PageTracker), new PropertyMetadata(OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PageTracker control = (PageTracker)d;
            IEnumerable<ReportPage> newSource = (IEnumerable<ReportPage>)e.NewValue;
            control.Records = $"Page 1 of {newSource.Count()}";
        }

        public IEnumerable<ReportPage> ItemsSource
        {
            get => (IEnumerable<ReportPage>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        static PageTracker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageTracker), new FrameworkPropertyMetadata(typeof(PageTracker)));
        }

        protected override void OnClicked(int movement)
        {
            Index = SourceList.IndexOf(SelectedPage);
            switch (movement)
            {
                case 1:
                    Index = 0;
                    break;
                case 2:
                    if (Index == 0) break;
                    Index--;
                    break;
                case 3:
                    if (Index == SourceList.Count - 1) break;
                    Index++;
                    break;
                case 4:
                    Index = SourceList.Count - 1;
                    break;
            }

            SelectedPage = SourceList[Index];
        }
    }
}
