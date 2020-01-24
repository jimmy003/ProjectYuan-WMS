using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Report
{
    public class ProjectReportParameter
    {
        public string Address2 { get; set; }
        public bool IsFeeds { get; set; }
        public string InternalCategory { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
