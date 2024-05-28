using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontEnd.Reports
{
    public interface IClonablePage
    {
        public ReportPage CloneMe();
    }
}
