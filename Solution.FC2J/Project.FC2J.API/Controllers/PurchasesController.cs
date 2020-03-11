using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.Models.Purchase;

namespace Project.FC2J.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PurchasesController : ControllerBase
    {

        private IPurchaseRepository _repo;
        private IConfiguration _config;

        public PurchasesController(IPurchaseRepository purchaseRepository, IConfiguration config)
        {
            _repo = purchaseRepository;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrder(string poNo)
        {
            var result = await _repo.GetPurchaseOrder(poNo);
            return Ok(result);
        }

        [HttpGet, Route("GetList")]
        public async Task<IActionResult> GetPurchases(string userName)
        {
            var result = await _repo.GetPurchases(userName);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(PurchaseOrder purchaseOrder)
        {
            var result = await _repo.Save(purchaseOrder, Convert.ToBoolean(_config.GetSection("AppSettings:IsSendEmail").Value));
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PurchaseOrder purchaseOrder)
        {
            await _repo.Update(purchaseOrder);
            return Ok();
        }

        [HttpPut, Route("Deliver")]
        public async Task<IActionResult> Deliver(PurchaseOrder purchaseOrder)
        {
            await _repo.Deliver(purchaseOrder);
            return Ok();
        }

        [HttpPost, Route("Payment")]
        public async Task<IActionResult> InsertPayment(POPayment value)
        {
            return Ok(await _repo.InsertPayment(value));
        }

        [HttpPost, Route("InsertInvoiceDetail")]
        public async Task<IActionResult> InsertInvoiceDetail(long poHeaderId, long productId, string invoiceNo)
        {
            await _repo.InsertInvoiceDetail(poHeaderId, productId, invoiceNo);
            return Ok();
        }


        [HttpGet, Route("Payment")]
        public async Task<IActionResult> GetPayments(long id)
        {
            return Ok(await _repo.GetPayments(id));
        }

        [HttpDelete, Route("Payment")]
        public async Task<IActionResult> DeletePayment(long id, string deletedBy, string invoiceNo)
        {
            await _repo.DeletePayment(id, deletedBy, invoiceNo);
            return Ok();
        }

    }
}
