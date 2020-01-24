using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.FC2J.DataStore.Interfaces;
using System.Threading.Tasks;
using Project.FC2J.Models.Report;

namespace Project.FC2J.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class ReportsController : ControllerBase
    {
        private IReportRepository _repo;
        private IConfiguration _config;

        public ReportsController(IReportRepository reportRepository, IConfiguration config)
        {
            _repo = reportRepository;
            _config = config;
        }

        [HttpPost, Route("GetCustomerAccountSummary")]
        public async Task<IActionResult> GetCustomerAccountSummary(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetCustomerAccountSummary(reportParameter));
        }

        [HttpPost, Route("GetPurchasesReportMonthlyVatExempt")]
        public async Task<IActionResult> GetPurchasesReportMonthlyVatExempt(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetPurchasesReportMonthlyVatExempt(reportParameter));
        }

        [HttpPost, Route("GetPurchasesReportMonthlyVatable")]
        public async Task<IActionResult> GetPurchasesReportMonthlyVatable(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetPurchasesReportMonthlyVatable(reportParameter));
        }


        [HttpPost, Route("GetSalesReportMonthlyBIRVatable")]
        public async Task<IActionResult> GetSalesReportMonthlyBIRVatable(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetSalesReportMonthlyBIRVatable(reportParameter));
        }

        [HttpPost, Route("GetSalesReportMonthlyBIR")]
        public async Task<IActionResult> GetSalesReportMonthlyBIR(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetSalesReportMonthlyBIR(reportParameter));
        }

        [HttpPost, Route("GetPurchasesReportSMAHC")]
        public async Task<IActionResult> GetPurchasesReportSMAHC(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetPurchasesReportSMAHC(reportParameter));
        }

        [HttpPost, Route("GetSalesReportSMAHC")]
        public async Task<IActionResult> GetSalesReportSMAHC(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetSalesReportSMAHC(reportParameter));
        }

        [HttpPost, Route("GetPurchaseReportMTD")]
        public async Task<IActionResult> GetPurchaseReportMTD(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetPurchaseReportMTD(reportParameter));
        }

        [HttpPost, Route("GetPurchaseReportMTDConverted")]
        public async Task<IActionResult> GetPurchaseReportMTDConverted(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetPurchaseReportMTDConverted(reportParameter));
        }

        [HttpPost,Route("GetMonthToDateSalesReport")]
        public async Task<IActionResult> GetMonthToDateSalesReport(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetMonthToDateSalesReport(reportParameter));
        }

        [HttpPost,Route("GetMTDSalesReportConverted")]
        public async Task<IActionResult> GetMTDSalesReportConverted(ProjectReportParameter reportParameter)
        {
            return Ok(await _repo.GetMTDSalesReportConverted(reportParameter));
        }

        [HttpGet, Route("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport(string poNo)
        {
            return Ok(await _repo.GetSalesReport());
        }


        [HttpGet, Route("GetDailyInventory")]
        public async Task<IActionResult> GetDailyInventory(string inventoryDate, int sourceId)
        {
            return Ok(await _repo.GetDailyInventory(inventoryDate, sourceId));
        }

        [HttpGet, Route("GetCategoryArrangement")]
        public async Task<IActionResult> GetCategoryArrangement()
        {
            return Ok(await _repo.GetCategoryArrangement());
        }

        [HttpGet, Route("GetCustomerAddress2")]
        public async Task<IActionResult> GetCustomerAddress2()
        {
            return Ok(await _repo.GetCustomerAddress2());
        }

        [HttpGet, Route("GetDailyInventoryCustomers")]
        public async Task<IActionResult> GetDailyInventoryCustomers(string inventoryDate, int sourceId)
        {
            return Ok(await _repo.GetDailyInventoryCustomers(inventoryDate, sourceId));
        }

        [HttpGet, Route("GetInventoryProducts")]
        public async Task<IActionResult> GetInventoryProducts()
        {
            return Ok(await _repo.GetInventoryProducts());
        }
    }
}
