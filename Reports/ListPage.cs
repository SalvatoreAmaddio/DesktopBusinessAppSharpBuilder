using FrontEnd.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class extends <see cref="ListBox"/> and it is meant to deal with <see cref="ReportPage"/> objects.
    /// It is used in <see cref="ReportViewer"/> to display Pages.
    /// </summary>
    public partial class ListPage : ListBox
    {
        private readonly ResourceDictionary resourceDict = Helper.GetDictionary(nameof(ListPage));
        public ListPage() 
        {
            Background = Brushes.Gray;
            BorderThickness = new(0);
            SelectionChanged += OnPageSelected;
            ItemContainerStyle = (Style)resourceDict["ListBoxItemContainerStyle"];
        }

        private void OnPageSelected(object sender, SelectionChangedEventArgs e) => ScrollIntoView(SelectedItem);
    }
}
