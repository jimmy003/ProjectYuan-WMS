using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    public interface ICustomerEndpoint : IEndpoint<Customer>
    {
        Task<List<Payment>> GetCustomerPayment(long id);
        Task<List<Payment>> GetPayments();
        Task<List<Product>> GetCustomerPriceList(long id, int supplier);
        Task<List<Product>> GetCustomerProduct(long id);
        Task<List<ShippingAddress>> GetCustomerShipTo(long id);
        Task<CustomerPayment> SaveCustomerPayment(CustomerPayment customerPayment);
        Task SaveCustomerPricelist(PriceListProduct value);
        Task<CustomerShipTo> SaveCustomerShipTo(CustomerShipTo customerShipTo);
        Task<CustomerProduct> SaveCustomerProduct(CustomerProduct customerProduct);
        Task<IEnumerable<Farm>> GetFarms();
    }
}
