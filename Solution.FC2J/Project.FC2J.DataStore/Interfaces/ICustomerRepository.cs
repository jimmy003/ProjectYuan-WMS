using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<IEnumerable<Customer>> GetCustomersByPartialName(string partialName);
        Task<IEnumerable<Payment>> GetCustomerPayment(long id);
        Task<IEnumerable<Payment>> GetPayments();
        Task<IEnumerable<Product>> GetCustomerPriceList(long id, int supplier);
        Task<IEnumerable<Product>> GetCustomerProduct(long id);
        Task<IEnumerable<ShippingAddress>> GetCustomerShipTo(long id);
        Task<Customer> SaveCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task RemoveCustomer(long id);

        Task SaveCustomerPayment(CustomerPayment customerPayment);

        Task SaveCustomerProduct(CustomerProduct customerProduct);

        Task SaveCustomerPriceList(PriceListProduct value);

        Task SaveCustomerShippingAddress(CustomerShipTo customerShipTo);

        Task UpdateCustomerPayment(CustomerPayment customerPayment);

        Task UpdateCustomerPricelist(CustomerPricelist customerPricelist);

        Task UpdateCustomerProduct(CustomerProduct customerProduct);

        Task UpdateCustomerShippingAddress(CustomerShipTo customerShipTo);

        Task RemoveCustomerDetails(Int64 id);

        Task RemoveCustomerPayment(Int64 id);

        Task RemoveCustomerPricelist(Int64 id);

        Task RemoveCustomerProduct(Int64 id);

        Task RemoveCustomerShippingAddress(Int64 id);

        Task<IEnumerable<Farm>> GetFarms();


    }
}
