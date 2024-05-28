namespace FrontEnd.Reports
{
    /// <summary>
    /// This interface defines a set of Properties that a <see cref="ReportPage"/> must implement.
    /// </summary>
    public interface IReportPage
    {
        public double PageWidth { get;  }
        public double PageHeight { get; }

        public int PageNumber { get; set; }

        public bool ContentOverflown { get; }
    }
}
