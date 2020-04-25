using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.Models.Sale;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers.Reports
{
    public class ReportEndpoint : IReportEndpoint
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;

        public ReportEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }

        public async Task<DataTable> GetPurchasesReportSMAHC(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetPurchasesReportSMAHC", reportParameter);
        }

        public async Task<DataTable> GetSalesReportMonthlyBIR(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetSalesReportMonthlyBIR", reportParameter);
        }

        public async Task<DataTable> GetSalesReportMonthlyBIRVatable(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetSalesReportMonthlyBIRVatable", reportParameter);
        }

        public async Task<DataTable> GetPurchasesReportMonthlyVatExempt(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetPurchasesReportMonthlyVatExempt", reportParameter);
        }

        public async Task<DataTable> GetPurchasesReportMonthlyVatable(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetPurchasesReportMonthlyVatable", reportParameter);
        }

        public async Task<DataTable> GetCustomerAccountSummary(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetCustomerAccountSummary", reportParameter);
        }

        public async Task<DataTable> GetBMEGReport(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetBMEGReport", reportParameter);
        }

        public async Task<DataTable> GetSalesReportSMAHC(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetSalesReportSMAHC", reportParameter);
        }

        public async Task<DataTable> GetMonthToDateSalesReport(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetMonthToDateSalesReport", reportParameter);
        }

        public async Task<DataTable> GetMTDSalesReportConverted(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetMTDSalesReportConverted", reportParameter);
        }

        public async Task<DataTable> GetPurchaseReportMTD(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetPurchaseReportMTD", reportParameter);
        }

        public async Task<DataTable> GetPurchaseReportMTDConverted(ProjectReportParameter reportParameter)
        {
            return await _apiHelper.GetDataTable(_apiAppSetting.Report + "/GetPurchaseReportMTDConverted", reportParameter);
        }

        public async Task<List<OrderHeader>> GetSalesReport()
        {
            return await _apiHelper.GetList<OrderHeader>(_apiAppSetting.Report + $"/GetSalesReport");
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            return _apiHelper.ToDataTable(data);
        }


        public async Task<List<ProductInternalCategory>> GetCategoryArrangement()
        {
            return await _apiHelper.GetList<ProductInternalCategory>(_apiAppSetting.Report + "/GetCategoryArrangement");
        }

        public async Task<List<ProjectCustomerAddress2>> GetCustomerAddress2()
        {
            return await _apiHelper.GetList<ProjectCustomerAddress2>(_apiAppSetting.Report + "/GetCustomerAddress2");
        }

        public async Task<List<Personnel>> GetPersonnel()
        {
            return await _apiHelper.GetList<Personnel>(_apiAppSetting.Report + "/GetPersonnel");
        }

        public async Task<List<DailyInventory>> GetDailyInventory(string inventoryDate, int sourceId)
        {
            return await _apiHelper.GetList<DailyInventory>(_apiAppSetting.Report + $"/GetDailyInventory?inventoryDate={inventoryDate}&sourceId={sourceId}");
        }

        public async Task<List<DailyInventoryCustomer>> GetDailyInventoryCustomers(string inventoryDate, int sourceId)
        {
            return await _apiHelper.GetList<DailyInventoryCustomer>(_apiAppSetting.Report + $"/GetDailyInventoryCustomers?inventoryDate={inventoryDate}&sourceId={sourceId}");
        }

        public async Task<List<InventoryProduct>> GetInventoryProducts()
        {
            return await _apiHelper.GetList<InventoryProduct>(_apiAppSetting.Report + $"/GetInventoryProducts");
        }
    }
}
