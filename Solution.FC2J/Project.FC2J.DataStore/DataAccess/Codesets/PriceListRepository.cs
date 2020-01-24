using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;

namespace Project.FC2J.DataStore.DataAccess.Codesets
{
    public class PriceListRepository : IPriceListRepository
    {
        private readonly string _spGetPriceLists = "GetPriceLists";
        private readonly string _spInsertPricelist = "InsertPricelist";

        private readonly string _spRemovePriceListCustomer = "RemovePriceListCustomer";

        private readonly string _spInsertCustomerPricelist = "InsertCustomerPricelist";

        private readonly string _spGetTargetCustomers = "GetTargetCustomers";

        private readonly string _spUpdateCustomerPricelist = "UpdateCustomerPricelist";

        private readonly string _spGetPriceListById = "GetPriceListById";
        private readonly string _spGetPriceList = "GetPriceList";
        private readonly string _spUpdatePOPricelist = "UpdatePOPricelist";
        private readonly string _spUpdatePricelistTemplateDetails = "UpdatePricelistTemplateDetails";

        public async Task UpdatePricelistTemplateDetails(long pricelistTemplateId, Product value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@PricelistTemplateId", pricelistTemplateId),
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@DeductionFixPrice", value.DeductionFixPrice),
                new SqlParameter("@DeductionOutright", value.DeductionOutright),
                new SqlParameter("@Discount", value.Discount),
                new SqlParameter("@DeductionPromoDiscount", value.DeductionPromoDiscount),
                new SqlParameter("@SalePrice_CORON", value.SalePrice_CORON),
                new SqlParameter("@SalePrice_LUBANG", value.SalePrice_LUBANG),
                new SqlParameter("@SalePrice_SANILDEFONSO", value.SalePrice_SANILDEFONSO)
            };
            await _spUpdatePricelistTemplateDetails.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }
        public async Task<List<PriceList>> GetList(int isForSalesOrder)
        {
            var value = await _spGetPriceLists.GetList<PriceList>(new SqlParameter("@IsForSalesOrder", isForSalesOrder));
            return value;
        }

        public async Task<List<Product>> GetPriceList(long id)
        {
            var value = await _spGetPriceList.GetList<Product>(new SqlParameter("@Id", id));
            return value;
        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public async Task SavePriceListCustomers(PriceListCustomer value)
        {

            value.CustomerIds.ForEach(async id =>
            {
                var sqlParameters1 = new List<SqlParameter>()
                {
                    new SqlParameter("@CustomerId", id)
                };
                await _spRemovePriceListCustomer.ExecuteNonQueryAsync(sqlParameters1.ToArray());
            });


            value.CustomerIds.ForEach(async id =>
            {
                var sqlParameters2 = new List<SqlParameter>()
                {
                    new SqlParameter("@CustomerId", id),
                    new SqlParameter("@PriceListId", value.PriceListId)
                };
                await _spInsertCustomerPricelist.ExecuteNonQueryAsync(sqlParameters2.ToArray());
            });

        }

        public async Task<List<TargetCustomer>> GetTargetCustomers(long priceListId)
        {
            var value = await _spGetTargetCustomers.GetList<TargetCustomer>(new SqlParameter("@PriceListId", priceListId));
            return value;
        }

        public async Task<PriceList> GetRecord(long priceListId)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", priceListId)
            };
            var result = await _spGetPriceListById.GetRecord<PriceList>(sqlParameters.ToArray());
            return result;
        }

        public Task<List<PriceList>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<PriceList> Save(PriceList value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Name", value.Name),
                new SqlParameter("@DiscountPolicy", value.DiscountPolicy),
                new SqlParameter("@Subscribed", value.Subscribed),
                new SqlParameter("@IsForSalesOrder", value.IsForSalesOrder)
            };
            value = await _spInsertPricelist.GetRecord<PriceList>(sqlParameters.ToArray());

            return value;
        }

        public async Task Update(PriceList value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@Name", value.Name),
                new SqlParameter("@DiscountPolicy", value.DiscountPolicy),
                new SqlParameter("@Subscribed", value.Subscribed)
            };
            await _spUpdateCustomerPricelist.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task UpdatePOPricelist(PriceList value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@Name", value.Name),
                new SqlParameter("@Email", value.Email)
            };
            await _spUpdatePOPricelist.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }
       
    }
}
