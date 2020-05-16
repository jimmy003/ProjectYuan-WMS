using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.Models.Product;

namespace Project.FC2J.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;

        public ProductsController(IProductRepository productRepository)
        {
            _repo = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts(long id)
        {
            var list = await _repo.GetProducts(id);
            return Ok(list);
        }

        [HttpGet,Route("GetInternalCategories")]
        public async Task<IActionResult> GetInternalCategories()
        {
            return Ok(await _repo.GetInternalCategories());
        }

        [HttpGet,Route("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _repo.GetCategories());
        }

        [HttpPost]
        public async Task<IActionResult> Post(Product product)
        {
            var result = await _repo.SaveProduct(product);
            return Ok(result);
        }

        //Task<IEnumerable<InventoryAdjustment>> GetForApprovalInventoryAdjustment()
        [Route("GetForApprovalInventoryAdjustment")]
        public async Task<IActionResult> GetForApprovalInventoryAdjustment()
        {
            var list = await _repo.GetForApprovalInventoryAdjustment();
            return Ok(list);
        }

        [HttpPut, Route("ApproveInventoryAdjustment")]
        public async Task<IActionResult> ApproveInventoryAdjustment(InventoryAdjustment payload)
        {
            await _repo.ApproveInventoryAdjustment(payload);
            return Ok();
        }

        [HttpPut, Route("UpdateProductInventory")]
        public async Task<IActionResult> UpdateProductInventory(InventoryAdjustment payload)
        {
            await _repo.UpdateProductInventory(payload);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(Product product)
        {
            await _repo.UpdateProduct(product);
            return Ok();
        }

        [HttpPut,Route("ProductPrice")]
        public async Task<IActionResult> UpdateProductPrice(ProductPrice productPrice)
        {
            await _repo.UpdateProductPrice(productPrice);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(long id)
        {
            await _repo.RemoveProduct(id);
            return Ok();
        }

    }
}
