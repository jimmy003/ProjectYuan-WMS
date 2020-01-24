using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class CustomerEndpoint : ICustomerEndpoint
    {

        private IApiAppSetting _apiAppSetting;
        private IAPIHelper _apiHelper;
        public CustomerEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }
        public async Task<List<Customer>> GetList()
        {
            return await _apiHelper.GetList<Customer>(_apiAppSetting.Customer);
        }
        public async Task<Customer> Save(Customer customer)
        {
            return await _apiHelper.Save<Customer>(_apiAppSetting.Customer, customer);
        }
        public async Task Update(Customer customer)
        {
            await _apiHelper.Update<Customer>(_apiAppSetting.Customer, customer);
        }
        public async Task Remove(long id)
        {
            await _apiHelper.Remove(_apiAppSetting.Customer + $"?id={id}");
        }
        public async Task<List<Payment>> GetCustomerPayment(long id)
        {
            return await _apiHelper.GetList<Payment>(_apiAppSetting.CustomerPayment + $"?id={id}");
        }

        public async Task<List<Product>> GetCustomerPriceList(long id, int supplier)
        {
            return await _apiHelper.GetList<Product>(_apiAppSetting.CustomerPricelist + $"?id={id}&supplier={supplier}");
        }

        public async Task<List<Product>> GetCustomerProduct(long id)
        {
            return await _apiHelper.GetList<Product>(_apiAppSetting.CustomerProduct + $"?id={id}");
        }

        public async Task<List<ShippingAddress>> GetCustomerShipTo(long id)
        {
            return await _apiHelper.GetList<ShippingAddress>(_apiAppSetting.CustomerShipTo + $"?id={id}");
        }

        public async Task<CustomerPayment> SaveCustomerPayment(CustomerPayment customerPayment)
        {
            return await _apiHelper.Save<CustomerPayment>(_apiAppSetting.CustomerPayment, customerPayment);
        }


        public async Task SaveCustomerPricelist(PriceListProduct value)
        {
            await _apiHelper.Save<PriceListProduct>(_apiAppSetting.CustomerPricelist, value);
        }

        public async Task<CustomerShipTo> SaveCustomerShipTo(CustomerShipTo customerShipTo)
        {
            return await _apiHelper.Save<CustomerShipTo>(_apiAppSetting.CustomerPayment, customerShipTo);
        }

        public async Task<CustomerProduct> SaveCustomerProduct(CustomerProduct customerProduct)
        {
            return await _apiHelper.Save<CustomerProduct>(_apiAppSetting.CustomerPayment, customerProduct);
       }

        public async Task<IEnumerable<Farm>> GetFarms()
        {
            return await _apiHelper.GetList<Farm>(_apiAppSetting.Customer+"/Farm");
        }

        public async Task<List<Payment>> GetPayments()
        {
            return await _apiHelper.GetList<Payment>(_apiAppSetting.CustomerPayment);
        }
    }
}
