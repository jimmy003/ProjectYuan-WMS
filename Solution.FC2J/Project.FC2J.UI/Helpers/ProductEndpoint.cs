using Project.FC2J.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Project.FC2J.Models.Customer;

namespace Project.FC2J.UI.Helpers
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

        public async Task<Product> Save(Product product)
        {
            return await _apiHelper.Save<Product>(_apiAppSetting.Product, product);
        }

        public async Task Update(Product product)
        {
            await _apiHelper.Update<Product>(_apiAppSetting.Product, product);
        }

    }
}
