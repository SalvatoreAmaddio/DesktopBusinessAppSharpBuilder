using Backend.Utils;
using System.Windows;
using System.Windows.Input;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class wraps a <see cref="ReportViewer"/> in Window.
    /// </summary>
    public class ReportViewerWindow : Window 
    {

        private readonly List<ReportPage> Pages = [];

        private readonly ReportViewer ReportViewer = new();

        public EmailSender? EmailSender
        {
            set => ReportViewer.EmailSender = value;
        }

        public bool OpenFile
        {
            get => ReportViewer.OpenFile;
            set => ReportViewer.OpenFile = value;
        }

        public ReportPage SelectedPage
        {
            get => ReportViewer.SelectedPage;
            set => ReportViewer.SelectedPage = value;
        }

        public string FileName
        {
            get => ReportViewer.FileName;
            set => ReportViewer.FileName = value;
        }

        public ReportViewerWindow() 
        {
            Content = ReportViewer;
            Title = "Report Viewer";
        }

        public void AddPage(ReportPage page) 
        {
            Pages.Add(page);
            ReportViewer.ItemsSource = Pages;
        }

        public void RemovePage(ReportPage page) 
        {
            Pages.Remove(page);
            ReportViewer.ItemsSource = Pages;
        }

        public ReportPage this[int index] => (index < 0 || index >= Pages.Count) ? throw new IndexOutOfRangeException() : Pages[index];

    }
}
