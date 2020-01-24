using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class PriceListEndpoint : IPriceListEndpoint
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;

        public PriceListEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }

        public async Task<List<PriceList>> GetList()
        {
            return await _apiHelper.GetList<PriceList>(_apiAppSetting.Pricelist);
        }
        public async Task<List<PriceList>> GetList(int isForSalesOrder)
        {
            return await _apiHelper.GetList<PriceList>(_apiAppSetting.Pricelist+$"?isForSalesOrder={isForSalesOrder}");
        }

        public async Task<List<Product>> GetPriceList(long id)
        {
            return await _apiHelper.GetList<Product>(_apiAppSetting.Pricelist + $"/GetPriceList?id={id}");
        }

        public async Task UpdatePOPricelist(PriceList priceList)
        {
            await _apiHelper.Update<PriceList>(_apiAppSetting.Pricelist+"/PO", priceList);
        }

        public async Task UpdatePricelistTemplateDetails(long pricelistTemplateId, Product value)
        {
            await _apiHelper.Update<Product>(_apiAppSetting.Pricelist + $"/UpdatePricelistTemplateDetails?pricelistTemplateId={pricelistTemplateId}", value);
        }

        public async Task<PriceList> Save(PriceList value)
        {
            return await _apiHelper.Save<PriceList>(_apiAppSetting.Pricelist, value);
        }

        public async Task Update(PriceList value)
        {
            await _apiHelper.Update<PriceList>(_apiAppSetting.Pricelist, value);
        }

        public async Task Remove(long id)
        {
            await _apiHelper.Remove(_apiAppSetting.Pricelist + $"?id={id}");
        }

        public async Task SavePriceListCustomers(PriceListCustomer value)
        {
            await _apiHelper.Save<PriceListCustomer> (_apiAppSetting.PricelistCustomer, value);
        }

        public async Task<List<TargetCustomer>> GetTargetCustomers(long priceListId)
        {
            return await _apiHelper.GetList<TargetCustomer>(_apiAppSetting.TargetCustomer + $"?priceListId={priceListId}");
        }

        public async Task<PriceList> GetRecord(long priceListId)
        {
            return await _apiHelper.GetRecord<PriceList>(_apiAppSetting.Pricelist + $"?id={priceListId}");
        }
    }
}
