using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers.Products
{
    public class ProductEndpoint : IProductEndpoint
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;

        public ProductEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }

        public async Task Remove(long id)
        {
            await _apiHelper.Remove(_apiAppSetting.Product + $"?id={id}");
        }

        public async Task<List<Product>> GetList()
        {
            return await GetList(0);
        }

        public async Task<List<Product>> GetList(long id)
        {
            return await _apiHelper.GetList<Product>(_apiAppSetting.Product + $"?id={id}");
        }

        public async Task UpdateProductPrice(ProductPrice productPrice)
        {
            await _apiHelper.Update<ProductPrice>(_apiAppSetting.Product + "/ProductPrice", productPrice);
        }

        public async Task<IEnumerable<ProductInternalCategory>> GetInternalCategories()
        {
            return await _apiHelper.GetList<ProductInternalCategory>(_apiAppSetting.Product + "/GetInternalCategories");
        }

        public async Task<IEnumerable<ProductSFACategory>> GetCategories()
        {
            return await _apiHelper.GetList<ProductSFACategory>(_apiAppSetting.Product + "/GetCategories");
        }

        public async Task UpdateProductInventory(InventoryAdjustment payload)
        {
            await _apiHelper.Update<InventoryAdjustment>(_apiAppSetting.Product + "/UpdateProductInventory", payload);
        }

        public async Task ApproveInventoryAdjustment(InventoryAdjustment payload)
        {
            await _apiHelper.Update<InventoryAdjustment>(_apiAppSetting.Product + "/ApproveInventoryAdjustment", payload);
        }

        public async Task<IEnumerable<InventoryAdjustment>> GetForApprovalInventoryAdjustment()
        {
            return await _apiHelper.GetList<InventoryAdjustment>(_apiAppSetting.Product + "/GetForApprovalInventoryAdjustment");
        }

        public async Task<Product> Save(Product product)
        {
            return await _apiHelper.Save<Product>(_apiAppSetting.Product, product);
        }

        public async Task Update(Product product)
        {
            await _apiHelper.Update(_apiAppSetting.Product, product);
        }

    }
}
