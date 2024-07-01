using Backend.Exceptions;
using Backend.Utils;
using FrontEnd.Controller;
using FrontEnd.Dialogs;
using FrontEnd.Forms;
using FrontEnd.Properties;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class instantiates a control that allows the viewing and printing of reports.
    /// <para/>
    /// See also: <seealso cref="ReportPage"/>, <seealso cref="ListPage"/>
    /// </summary>
    public class ReportViewer : Control, IAbstractControl
    {
        private Button? PART_SendButton;
        private Button? PART_ChooseDir;

        /// <summary>
        /// Gets the file path of the report to be saved.
        /// </summary>
        private string FilePath => DirName + $"\\{FileName}.pdf";
        public Window? ParentWindow { get; private set; }

        /// <summary>
        /// Sets the <see cref="EmailSender"/> that will be used by the <see cref="OnSendEmailClicked(object?, EventArgs)"/>.
        /// </summary>
        public EmailSender? EmailSender { private get; set; }

        #region OpenFile
        /// <summary>
        /// Sets this property to true to open the file after the printing process has completed.
        /// </summary>
        public bool OpenFile
        {
            get => (bool)GetValue(OpenFileProperty);
            set => SetValue(OpenFileProperty, value);
        }

        /// <summary>
        /// Identifies the OpenFile dependency property.
        /// </summary>
        public static readonly DependencyProperty OpenFileProperty =
        DependencyProperty.Register(nameof(OpenFile), typeof(bool), typeof(ReportViewer), new PropertyMetadata(false));
        #endregion

        #region Message
        /// <summary>
        /// Gets or sets the message to be displayed.
        /// </summary>
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        /// <summary>
        /// Identifies the Message dependency property.
        /// </summary>
        public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(nameof(Message), typeof(string), typeof(ReportViewer), new PropertyMetadata(string.Empty));
        #endregion

        #region IsLoading
        /// <summary>
        /// Gets or sets a value indicating whether a progress bar should be displayed.
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// Identifies the IsLoading dependency property.
        /// </summary>
        public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ReportViewer), new PropertyMetadata(false));
        #endregion

        #region FileName
        /// <summary>
        /// Gets or sets the name of the file to be printed.
        /// </summary>
        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        /// <summary>
        /// Identifies the FileName dependency property.
        /// </summary>
        public static readonly DependencyProperty FileNameProperty =
        DependencyProperty.Register(nameof(FileName), typeof(string), typeof(ReportViewer), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region DirName
        /// <summary>
        /// Gets or sets the directory name where the file will be saved.
        /// </summary>
        public string DirName
        {
            get => (string)GetValue(DirNameProperty);
            set => SetValue(DirNameProperty, value);
        }

        /// <summary>
        /// Identifies the DirName dependency property.
        /// </summary>
        public static readonly DependencyProperty DirNameProperty =
        DependencyProperty.Register(nameof(DirName), typeof(string), typeof(ReportViewer), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDirNamePropertyChanged));
        #endregion

        #region SelectedPage
        /// <summary>
        /// Gets or sets the currently selected page of the report.
        /// </summary>
        public ReportPage SelectedPage
        {
            get => (ReportPage)GetValue(SelectedPageProperty);
            set => SetValue(SelectedPageProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedPage dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedPageProperty =
        DependencyProperty.Register(nameof(SelectedPage), typeof(ReportPage), typeof(ReportViewer), new PropertyMetadata());
        #endregion

        #region ItemsSource
        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{ReportPage}"/> containing one or more <see cref="ReportPage"/> instances.
        /// </summary>
        public IEnumerable<ReportPage> ItemsSource
        {
            get => (IEnumerable<ReportPage>)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ReportPage>), typeof(ReportViewer), new PropertyMetadata());
        #endregion

        #region PrintCommand
        /// <summary>
        /// Gets or sets the command that calls the <see cref="PrintFixDocs"/> method to print the document.
        /// </summary>
        public ICommand PrintCommand
        {
            get => (ICommand)GetValue(PrintCommandProperty);
            set => SetValue(PrintCommandProperty, value);
        }

        /// <summary>
        /// Identifies the PrintCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty PrintCommandProperty =
        DependencyProperty.Register(nameof(PrintCommand), typeof(ICommand), typeof(ReportViewer), new PropertyMetadata());
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="ReportViewer"/> class.
        /// </summary>
        static ReportViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ReportViewer), new FrameworkPropertyMetadata(typeof(ReportViewer)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportViewer"/> class.
        /// </summary>
        public ReportViewer()
        {
            PrintCommand = new CMDAsync(PrintFixDocs);

            if (string.IsNullOrEmpty(FrontEndSettings.Default.ReportDefaultDirectory))
                FrontEndSettings.Default.ReportDefaultDirectory = Sys.Desktop;

            DirName = FrontEndSettings.Default.ReportDefaultDirectory;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            PART_SendButton = (Button?)GetTemplateChild(nameof(PART_SendButton));
            if (PART_SendButton != null)
                PART_SendButton.Click += OnSendEmailClicked;

            PART_ChooseDir = (Button?)GetTemplateChild(nameof(PART_ChooseDir));
            if (PART_ChooseDir != null)
                PART_ChooseDir.Click += OnChooseDirClicked;

            ParentWindow = Window.GetWindow(this);
            if (ParentWindow != null)
                ParentWindow.Closed += OnClosed;
        }

        /// <summary>
        /// Handles changes to the DirName property.
        /// </summary>
        private static void OnDirNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string? str = (string?)e.NewValue;
            ReportViewer reportViewer = (ReportViewer)d;
            reportViewer.DirName = (string.IsNullOrEmpty(str)) ? Sys.Desktop : str;
            FrontEndSettings.Default.ReportDefaultDirectory = reportViewer.DirName;
            FrontEndSettings.Default.Save();
        }

        /// <summary>
        /// Handles the send email button click event.
        /// </summary>
        protected virtual async void OnSendEmailClicked(object? sender, EventArgs e)
        {
            if (EmailSender == null) 
            {
                Failure.Allert("This command has not been implemented");
                return;
            }

            if (!EmailSender.CredentialCheck()) 
            {
                Failure.Allert("I could not find any email settings.");
                return;
            }

            DialogResult result = ConfirmDialog.Ask("Do you want to send this document by email?");
            if (result == DialogResult.No) return;

            bool openFile = OpenFile;
            OpenFile = false;

            Task<bool> printingTask = PrintFixDocs();
            
            Message = "Preparing document...";
            await Task.Delay(100);

            bool hasPrinted = await printingTask;
            if (!hasPrinted) 
            {
                Message = "Email Task Failed.";
                return;
            }

            if (!File.Exists(FilePath))
            {
                Failure.Allert("I could not find the file to attach.");
                Message = "Email Task Failed.";
                return;
            }

            IsLoading = true;
            await Task.Delay(100);

            EmailSender.AddAttachment(FilePath);

            Message = "Sending...";

            try
            {
                await Task.Run(EmailSender.SendAsync);
            }
            catch (Exception)
            {
                Failure.Allert("The system failed to send the email. Possible reasons could be:\n- Wrong email settings,\nPoor internet connection.");
                Message = "Email Task Failed.";
                return;
            }
            finally 
            {
                IsLoading = false;
                OpenFile = openFile;
                string filePath = CopyFilePath();
                await Task.Run(() => File.Delete(filePath));
            }

            Message = "Almost Ready...";
            SuccessDialog.Display("Email Sent");
            Message = "";
        }

        /// <summary>
        /// Handles the choose directory button click event.
        /// </summary>
        private void OnChooseDirClicked(object sender, RoutedEventArgs e)
        {
            FolderDialog folderDialog = new("Select the folder where to save this file.");
            bool result = folderDialog.ShowDialog();
            DirName = (result) ? folderDialog.SelectedPath : Sys.Desktop;
        }

        /// <summary>
        /// Copies the source of the cloned report pages.
        /// </summary>
        private IEnumerable<PageContent> CopySource(IEnumerable<ReportPage> clonedPages)
        {
            foreach (ReportPage page in clonedPages)
                yield return page.AsPageContent();
        }

        /// <summary>
        /// Creates a new string containing the <see cref="_dirName"/> and <see cref="FileName"/> properties.
        /// This is for threading purposes to avoid threading errors.
        /// </summary>
        /// <returns>A string representing the file path.</returns>
        private string CopyFilePath() 
        {
            StringBuilder sb = new();
            sb.Append(DirName);
            sb.Append($"\\{FileName}.pdf");
            return sb.ToString();
        }

        /// <summary>
        /// Starts the printing process.
        /// </summary>
        protected virtual async Task<bool> PrintFixDocs()
        {
            if (!PDFPrinter.IsInstalled())
            {
                Failure.Allert("I could not find a PDF Printer in your computer");
                return false;
            }

            if (string.IsNullOrEmpty(FileName)) 
            {
                Failure.Allert("Please, specify a file name");
                return false;
            }

            if (!Directory.Exists(DirName))
            {
                Failure.Allert("The directory that you have specified does not exist.");
                return false;
            }

            if (File.Exists(FilePath)) 
            {
                DialogResult result = UnsavedDialog.Ask($"A file named {FileName} already exist int {DirName}. Do you want to replace it?");
                if (result.Equals(DialogResult.No)) return false;
                string filePath = CopyFilePath();
                await Task.Run(()=>File.Delete(filePath));
            }

            IsLoading = true;
            await Task.Delay(100);

            IEnumerable<ReportPage> clonedPages = ItemsSource.Cast<IClonablePage>().Select(s=>s.CloneMe());
            IEnumerable<PageContent> copied = await Task.Run(() => CopySource(clonedPages));

            return await Application.Current.Dispatcher.InvokeAsync(async() =>
            {
                return await PrintTask(copied);
            }).Result;
        }

        /// <summary>
        /// Executes the print task.
        /// </summary>
        /// <param name="copied">The pages to be printed.</param>
        /// <returns>True if printing was successful; otherwise, false.</returns>
        protected virtual async Task<bool> PrintTask(IEnumerable<PageContent> copied)
        {
            ReportPage first_page = ItemsSource.First();
            double width = first_page.PageWidth;
            double height = first_page.PageHeight;

            FixedDocument doc = new();
            doc.DocumentPaginator.PageSize = new Size(width, height);

            foreach (var i in copied)
                doc.Pages.Add(i);

            PDFPrinter pdfPrinter = new(FileName, DirName);

            try
            {
                await Task.Run(pdfPrinter.PrinterPortManager.SetPort);
            }
            catch (RunAsAdminException ex) 
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    Failure.Allert(ex.Message,"Ouch.");
                    IsLoading = false;
                });
                return false;
            }

            Message = "Saving...";
            pdfPrinter.Print(doc.DocumentPaginator);

            await Task.Run(pdfPrinter.PrinterPortManager.ResetPort);

            Message = "Printed!";

            if (OpenFile)
            {
                Message = "Opening...";
                string filePath = CopyFilePath();
                await Task.Run(() => Open(filePath));
                Message = string.Empty;
            }
            IsLoading = false;
            return true;
        }

        /// <summary>
        /// Opens the file after it has been printed.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        private static void Open(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open the PDF file. Error: {ex.Message}");
            }
        }

        #region IAbstractControl
        public void OnUnloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        public void OnClosed(object? sender, EventArgs e) => Dispose();
        public void DisposeEvents()
        {
            if (PART_SendButton != null)
                PART_SendButton.Click -= OnSendEmailClicked;

            if (PART_ChooseDir != null)
                PART_ChooseDir.Click -= OnChooseDirClicked;

            if (ParentWindow != null)
                ParentWindow.Closed -= OnClosed;
        }
        #endregion

        public void Dispose()
        {
            DisposeEvents();
            foreach(ReportPage page in ItemsSource) 
                page.Dispose();  
            GC.SuppressFinalize(this);
        }
    }
}