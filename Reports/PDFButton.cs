using FrontEnd.Utils;
using System.Windows.Controls;

namespace FrontEnd.Reports
{
    public class PDFButton : Button
    {
        public PDFButton() 
        {
            ToolTip = "Print";
            Content = new Image()
            {
                Source = Helper.LoadFromImages("pdf")
            };

        }
    }
}
