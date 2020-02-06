using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;

namespace Project.FC2J.API.Controllers.Codesets
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceListsController : ControllerBase
    {

        private readonly IPriceListRepository _repo;

        public PriceListsController(IPriceListRepository repository)
        {
            _repo = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(long id, int isForSalesOrder=1)
        {
            if (id == 0)
            {
                var list = await _repo.GetList(isForSalesOrder);
                return Ok(list);
            }
            else
            {
                var record = await _repo.GetRecord(id);
                return Ok(record);
            }
        }

        [HttpGet,Route("TargetCustomer")]
        public async Task<IActionResult> GetTargetCustomers(long priceListId)
        {
            var list = await _repo.GetTargetCustomers(priceListId);
            return Ok(list);
        }

        [HttpGet, Route("GetPriceList")]
        public async Task<IActionResult> GetPriceList(long id)
        {
            var list = await _repo.GetPriceList(id);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PriceList priceList)
        {
            var result = await _repo.Save(priceList);
            return Ok(result);
        }
        
        [HttpPost,Route("Customer")]
        public async Task<IActionResult> Post(PriceListCustomer value)
        {
            var output = await _repo.SavePriceListCustomers(value);
            return Ok(output);
        }

        [HttpPut]
        public async Task<IActionResult> Put(PriceList priceList)
        {
            await _repo.Update(priceList);
            return Ok();
        }

        [HttpPut,Route("UpdatePricelistName")]
        public async Task<IActionResult> UpdatePricelistName(PriceList value)
        {
            await _repo.UpdatePricelistName(value);
            return Ok();
        }

        [HttpDelete, Route("RemovePriceListCustomer")]
        public async Task<IActionResult> Delete(long customerId)
        {
            await _repo.RemovePriceListCustomer(customerId);
            return Ok();
        }

        [HttpPut, Route("UpdatePricelistTemplateDetails")]
        public async Task<IActionResult> UpdatePricelistTemplateDetails(long pricelistTemplateId, [FromBody]Product value)
        {
            await _repo.UpdatePricelistTemplateDetails(pricelistTemplateId, value);
            return Ok();
        }

        [HttpPut, Route("PO")]
        public async Task<IActionResult> UpdatePOPricelist(PriceList priceList)
        {
            await _repo.UpdatePOPricelist(priceList);
            return Ok();
        }

    }
}
