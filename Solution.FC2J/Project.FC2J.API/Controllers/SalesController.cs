using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Project.FC2J.Models.Sale;

namespace Project.F2CJ.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private ISaleRepository _repo;
        private readonly IConfiguration _config;

        public SalesController(ISaleRepository saleRepository, IConfiguration config)
        {
            _repo = saleRepository;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post(SaleHeader sale)
        {
            var result = await _repo.PostSale(sale);
            return Ok(result);
        }

        [HttpGet, Route("SONo")]
        public async Task<IActionResult> GetSONo()
        {
            var soNo = await _repo.GetSONo();
            return Ok(new SalesInvoice { SONo = soNo });
        }

        [HttpGet, Route("InvoiceNo")]
        public async Task<IActionResult> GetInvoiceNo()
        {
            var value = await _repo.GetSalesInvoice();
            return Ok(value);
        }


        [HttpGet, Route("Invoices")]
        public async Task<IActionResult> GetInvoices(long id, long customerId)
        {
            if (customerId > 0)
            {
                var value = await _repo.GetSaleHeader(customerId, id);
                return Ok(value);
            }
            else
            {
                var value = await _repo.GetInvoices();
                return Ok(value);
            }
        }

        [HttpPost, Route("Invoices")]
        public async Task<IActionResult> ReceivedInvoice(ReceiveInvoice receiveInvoice)
        {
             await _repo.ReceivedInvoice(receiveInvoice);
             return Ok();
        }

        [HttpPut, Route("Invoices")]
        public async Task<IActionResult> PayInvoice(SalePayment salePayment)
        {
            await _repo.PayInvoice(salePayment);
            return Ok();
        }
        [HttpPut, Route("RetrieveInvoice")]
        public async Task<IActionResult> RetrievePaidBadSale(SalePayment salePayment)
        {
            await _repo.RetrievePaidBadSale(salePayment);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSales(long id, string userName, long customerId)
        {
            if (id > 0)
            {
                var list = await _repo.GetSaleDetails(id, customerId);
                return Ok(list);
            }
            else
            {
                var list = await _repo.GetSales(userName);
                return Ok(list);
            }
        }

        [HttpGet, Route("Collections")]
        public async Task<IActionResult> GetCollections(string userName)
        {
            var nearDue = Convert.ToInt32(_config.GetSection("AppSettings:NearDue").Value);
            var list = await _repo.GetCollections(userName, nearDue);
            return Ok(list);
        }

        [HttpGet, Route("Collected")]
        public async Task<IActionResult> GetCollected(string userName, int isPaid)
        {
            var list = await _repo.GetCollected(userName, isPaid);
            return Ok(list);
        }


    }
}
