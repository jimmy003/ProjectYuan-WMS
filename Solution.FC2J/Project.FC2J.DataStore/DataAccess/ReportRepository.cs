using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.Models.Sale;

namespace Project.FC2J.DataStore.DataAccess
{
    public class ReportRepository : IReportRepository
    {
        private readonly string _spGetMonthToDateSalesReport = "GetMonthToDateSalesReport";
        private readonly string _spGetDailyInventory = "GetDailyInventory";
        private readonly string _spGetCategoryArrangement = "GetCategoryArrangement";
        private readonly string _spGetCustomerAddress2 = "GetCustomerAddress2";
        private readonly string _spGetPersonnel = "GetPersonnel";
        private readonly string _spGetDailyInventoryCustomers = "GetDailyInventoryCustomers";
        private readonly string _spGetInventoryProducts = "GetInventoryProducts";
        private readonly string _spGetSalesReport = "GetSalesReport";
        private readonly string _spGetMTDSalesReportConverted = "GetMTDSalesReportConverted";
        private readonly string _spGetPurchaseReportMTD = "GetPurchaseReportMTD";
        private readonly string _spGetPurchaseReportMTDConverted = "GetPurchaseReportMTDConverted";
        private readonly string _spGetSalesReportSMAHC = "GetSalesReportSMAHC"; //MTD AND MONTHLY
        private readonly string _spGetPurchasesReportSMAHC = "GetPurchasesReportSMAHC"; //MTD AND MONTHLY
        private readonly string _spGetSalesReportMonthlyBIR = "GetSalesReportMonthlyBIR"; 
        private readonly string _spGetSalesReportMonthlyBIRVatable = "GetSalesReportMonthlyBIRVatable";

        private readonly string _spGetPurchasesReportMonthlyVatExempt = "GetPurchasesReportMonthlyVatExempt"; 
        private readonly string _spGetPurchasesReportMonthlyVatable = "GetPurchasesReportMonthlyVatable";
        private readonly string _spGetCustomerAccountSummary = "GetCustomerAccountSummary";
        private readonly string _spGetBmegReport = "GetBmegReport";
        private List<SqlParameter> _sqlParameters;

        public async Task<DataTable> GetCustomerAccountSummary(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetCustomerAccountSummary.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetBMEGReport(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@FROM", reportParameter.DateFrom.ToString("yyyy-MM-dd")),
                new SqlParameter("@TO", reportParameter.DateTo.ToString("yyyy-MM-d"))
            };
            return await _spGetBmegReport.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetPurchasesReportMonthlyVatable(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetPurchasesReportMonthlyVatable.GetDataTable(_sqlParameters.ToArray());
        }
        public async Task<DataTable> GetPurchasesReportMonthlyVatExempt(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetPurchasesReportMonthlyVatExempt.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<List<OrderHeader>> GetSalesReport()
        {
            return await _spGetMonthToDateSalesReport.GetList<OrderHeader>();
        }

        public async Task<DataTable> GetSalesReportMonthlyBIRVatable(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetSalesReportMonthlyBIRVatable.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetSalesReportMonthlyBIR(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@IsVatable", reportParameter.IsFeeds),
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetSalesReportMonthlyBIR.GetDataTable(_sqlParameters.ToArray());
        }
        public async Task<DataTable> GetPurchasesReportSMAHC(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetPurchasesReportSMAHC.GetDataTable(_sqlParameters.ToArray());
        }


        public async Task<DataTable> GetSalesReportSMAHC(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetSalesReportSMAHC.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetMonthToDateSalesReport(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@address2", reportParameter.Address2),
                new SqlParameter("@InternalCategory", reportParameter.InternalCategory),
                new SqlParameter("@IsFeeds", reportParameter.IsFeeds),
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetSalesReport.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetPurchaseReportMTD(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@address2", reportParameter.Address2),
                new SqlParameter("@InternalCategory", reportParameter.InternalCategory),
                new SqlParameter("@IsFeeds", reportParameter.IsFeeds),
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetPurchaseReportMTD.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetPurchaseReportMTDConverted(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@address2", reportParameter.Address2),
                new SqlParameter("@InternalCategory", reportParameter.InternalCategory),
                new SqlParameter("@IsFeeds", reportParameter.IsFeeds),
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetPurchaseReportMTDConverted.GetDataTable(_sqlParameters.ToArray());
        }

        public async Task<DataTable> GetMTDSalesReportConverted(ProjectReportParameter reportParameter)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@address2", reportParameter.Address2),
                new SqlParameter("@InternalCategory", reportParameter.InternalCategory),
                new SqlParameter("@IsFeeds", reportParameter.IsFeeds),
                new SqlParameter("@DateFrom", reportParameter.DateFrom),
                new SqlParameter("@DateTo", reportParameter.DateTo)
            };
            return await _spGetMTDSalesReportConverted.GetDataTable(_sqlParameters.ToArray());
        }
        public async Task<List<ProjectCustomerAddress2>> GetCustomerAddress2()
        {
            return await _spGetCustomerAddress2.GetList<ProjectCustomerAddress2>();
        }

        public async Task<List<Personnel>> GetPersonnel()
        {
            return await _spGetPersonnel.GetList<Personnel>();
        }

        public async Task<List<ProductInternalCategory>> GetCategoryArrangement()
        {
            return await _spGetCategoryArrangement.GetList<ProductInternalCategory>();
        }

        public async Task<List<DailyInventory>> GetDailyInventory(string inventoryDate, int sourceId)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@InventoryDate", Convert.ToDateTime(inventoryDate)),
                new SqlParameter("@SourceId", sourceId)
            };
            return await _spGetDailyInventory.GetList<DailyInventory>(_sqlParameters.ToArray());
        }

        public async Task<List<DailyInventoryCustomer>> GetDailyInventoryCustomers(string inventoryDate, int sourceId)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@InventoryDate", Convert.ToDateTime(inventoryDate)),
                new SqlParameter("@SourceId", sourceId)
            };
            return await _spGetDailyInventoryCustomers.GetList<DailyInventoryCustomer>(_sqlParameters.ToArray());
        }

        public async Task<List<InventoryProduct>> GetInventoryProducts()
        {
            return await _spGetInventoryProducts.GetList<InventoryProduct>();
        }

    }
}
