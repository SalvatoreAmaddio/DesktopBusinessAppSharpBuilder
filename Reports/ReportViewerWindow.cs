using Backend.Utils;
using System.Windows;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class wraps a <see cref="ReportViewer"/> in a <see cref="Window"/>.
    /// It provides functionalities to manage report pages and interact with the <see cref="ReportViewer"/>.
    /// </summary>
    public class ReportViewerWindow : Window
    {
        /// <summary>
        /// A list of <see cref="ReportPage"/> objects that are displayed in the <see cref="ReportViewer"/>.
        /// </summary>
        private readonly List<ReportPage> _pages = [];

        /// <summary>
        /// The <see cref="ReportViewer"/> instance that is wrapped by this window.
        /// </summary>
        private readonly ReportViewer _reportViewer = new();

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="EmailSender"/> used by the <see cref="ReportViewer"/>.
        /// </summary>
        public EmailSender? EmailSender
        {
            set => _reportViewer.EmailSender = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ReportViewer"/> can open the file once it gets printed.
        /// </summary>
        public bool OpenFile
        {
            get => _reportViewer.OpenFile;
            set => _reportViewer.OpenFile = value;
        }

        /// <summary>
        /// Gets or sets the selected <see cref="ReportPage"/> in the <see cref="ReportViewer"/>.
        /// </summary>
        public ReportPage SelectedPage
        {
            get => _reportViewer.SelectedPage;
            set => _reportViewer.SelectedPage = value;
        }

        /// <summary>
        /// Gets or sets the file name of the report being viewed.
        /// </summary>
        public string FileName
        {
            get => _reportViewer.FileName;
            set => _reportViewer.FileName = value;
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewerWindow"/> class.
        /// </summary>
        public ReportViewerWindow() 
        {
            Content = _reportViewer;
            Title = "Report Viewer";
            Closed += OnWindowClosed;
        }

        /// <summary>
        /// Handles the <see cref="Window.Closed"/> event.
        /// Disposes the <see cref="ReportViewer"/> and unsubscribes from the event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void OnWindowClosed(object? sender, EventArgs e)
        {
            _reportViewer.Dispose();
            Closed -= OnWindowClosed;
        }

        /// <summary>
        /// Adds a <see cref="ReportPage"/> to the <see cref="ReportViewer"/>.
        /// </summary>
        /// <param name="page">The report page to add.</param>
        public void AddPage(ReportPage page) 
        {
            _pages.Add(page);
            _reportViewer.ItemsSource = _pages;
        }

        /// <summary>
        /// Removes a <see cref="ReportPage"/> from the <see cref="ReportViewer"/>.
        /// </summary>
        /// <param name="page">The report page to remove.</param>
        public void RemovePage(ReportPage page) 
        {
            _pages.Remove(page);
            _reportViewer.ItemsSource = _pages;
        }

        /// <summary>
        /// Gets the <see cref="ReportPage"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the page to get.</param>
        /// <returns>The <see cref="ReportPage"/> at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is out of range.</exception>
        public ReportPage this[int index] => (index < 0 || index >= _pages.Count) ? throw new IndexOutOfRangeException() : _pages[index];
    }
}