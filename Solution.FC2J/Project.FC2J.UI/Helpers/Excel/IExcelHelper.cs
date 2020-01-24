using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers.Excel
{
    public interface IExcelHelper
    {
        Task ExportDSToExcel(DataSet ds, string destination, string overAllTotal = "", string dateRange = "", ReportTypeEnum reportTypeEnum = ReportTypeEnum.Inventory, string tin = "");
    }
}
