using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;

namespace Project.FC2J.UI.Helpers
{
    public interface IPriceListEndpoint :  IEndpoint<PriceList>
    {
        Task SavePriceListCustomers(PriceListCustomer value);
        Task<List<TargetCustomer>> GetTargetCustomers(long priceListId);
        Task<PriceList> GetRecord(long priceListId);
        Task<List<PriceList>> GetList(int isForSalesOrder);
        Task<List<Product>> GetPriceList(long id);
        Task UpdatePOPricelist(PriceList priceList);
        Task UpdatePricelistTemplateDetails(long pricelistTemplateId, Product value);

    }
}
