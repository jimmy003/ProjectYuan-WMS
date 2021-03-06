﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;

namespace Project.FC2J.DataStore.Interfaces.Codesets
{
    public interface IPriceListRepository : IRepository<PriceList>
    {
        Task<PriceListCustomer> SavePriceListCustomers(PriceListCustomer value);
        Task<List<TargetCustomer>> GetTargetCustomers(long priceListId);
        Task<PriceList> GetRecord(long priceListId);
        Task<List<PriceList>> GetList(int isForSalesOrder);
        Task<List<Product>> GetPriceList(long id);
        Task UpdatePOPricelist(PriceList value);
        Task UpdatePricelistTemplateDetails(long pricelistTemplateId, Product value);
        Task RemovePriceListCustomer(long customerId);
        Task UpdatePricelistName(PriceList value);
    }
}
