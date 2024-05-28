using Backend.Exceptions;
using Backend.Utils;
using System.Printing;
using System.Windows.Documents;
using System.Windows.Xps;

namespace FrontEnd.Reports
{
    /// <summary>
    /// This class wraps a <see cref="PrintQueue"/> object which will be used to handle a PDF Printer installed in the local computer.
    /// </summary>
    public class PDFPrinter 
    {
        private PrintQueue Queue { get; set; }

        /// <summary>
        /// Gets the PrinterPortManager that manages the PDF Printer's port.
        /// </summary>
        public PrinterPortManager PrinterPortManager { get; } = new();

        /// <summary>
        /// Creates a PDFPrinter object. If the local computer has no PDF Printer installed, 
        /// this constructor will throw an exception. <para/>
        /// You can call <see cref="IsInstalled"/> to check if a PDF Printer is installed.
        /// </summary>
        /// <exception cref="PDFPrinterNotFoundException"></exception>
        public PDFPrinter()
        {
            try 
            {
                Queue = GetPDFPrinter();
            }
            catch 
            {
                throw new PDFPrinterNotFoundException();
            }
        }

        /// <summary>
        /// Checks if a PDF printer is installed in the local computer.
        /// </summary>
        /// <returns>true if a PDF Printer was found in the local computer.</returns>
        public static bool IsInstalled() 
        {
            PrintQueue? pdfPrinter = new LocalPrintServer()
                .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
                .FirstOrDefault(pq => pq.Name.Contains("PDF"));
            return pdfPrinter != null;
        }

        /// <summary>
        /// Creates a PDFPrinter object by providing values to create a new Port.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="dirPath"></param>
        public PDFPrinter(string fileName, string dirPath) : this()
        {
            PrinterPortManager.NewPortName = fileName;
            PrinterPortManager.DirectoryPath = dirPath;
        }
        
        /// <summary>
        /// It prints the document by using a <see cref="XpsDocumentWriter"/>.
        /// </summary>
        /// <param name="documentPaginator"></param>
        public void Print(DocumentPaginator documentPaginator) 
        {
            PrintTicket printTicket = Queue.DefaultPrintTicket;
            XpsDocumentWriter xpsDocumentWriter = PrintQueue.CreateXpsDocumentWriter(Queue);
            xpsDocumentWriter.Write(documentPaginator, printTicket);
            while (Queue.NumberOfJobs > 0)
                Queue.Refresh();
        }

        /// <summary>
        /// Retrieve the local PDF Printer.
        /// </summary>
        /// <returns>A PrintQueue object</returns>
        private static PrintQueue GetPDFPrinter() =>
            new LocalPrintServer()
                .GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections })
                .First(pq => pq.Name.Contains("PDF"));

    }
}
