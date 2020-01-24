using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.Models.Sale;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface IReportRepository
    {
        Task<List<OrderHeader>> GetSalesReport();
        Task<List<DailyInventory>> GetDailyInventory(string inventoryDate, int sourceId);
        Task<List<ProductInternalCategory>> GetCategoryArrangement();
        Task<List<DailyInventoryCustomer>> GetDailyInventoryCustomers(string inventoryDate, int sourceId);
        Task<List<InventoryProduct>> GetInventoryProducts();

        Task<DataTable> GetMonthToDateSalesReport(ProjectReportParameter reportParameter);
        Task<DataTable> GetPurchaseReportMTD(ProjectReportParameter reportParameter);
        Task<DataTable> GetPurchaseReportMTDConverted(ProjectReportParameter reportParameter);
        Task<DataTable> GetMTDSalesReportConverted(ProjectReportParameter reportParameter);
        Task<List<ProjectCustomerAddress2>> GetCustomerAddress2();
        Task<DataTable> GetSalesReportSMAHC(ProjectReportParameter reportParameter);
        Task<DataTable> GetPurchasesReportSMAHC(ProjectReportParameter reportParameter);
        Task<DataTable> GetSalesReportMonthlyBIR(ProjectReportParameter reportParameter);
        Task<DataTable> GetSalesReportMonthlyBIRVatable(ProjectReportParameter reportParameter);

        Task<DataTable> GetPurchasesReportMonthlyVatExempt(ProjectReportParameter reportParameter);
        Task<DataTable> GetPurchasesReportMonthlyVatable(ProjectReportParameter reportParameter);
        Task<DataTable> GetCustomerAccountSummary(ProjectReportParameter reportParameter);
    }
}
