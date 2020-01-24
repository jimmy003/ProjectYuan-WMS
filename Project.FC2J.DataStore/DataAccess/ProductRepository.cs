using Project.FC2J.Models.Product;
using Project.FC2J.DataStore.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.Models.Purchase;
using Project.FC2J.Models.Report;

namespace Project.FC2J.DataStore.Interfaces
{
    public class ProductRepository : IProductRepository
    {

        private const string _spGetProducts = "GetProducts";
        private const string _spGetProductsByCustomer = "GetProductsByCustomer";
        private const string _updateProduct = "UpdateProduct";
        private const string _removeProduct = "RemoveProduct";
        private const string _insertProduct = "InsertProduct";
        private const string _spUpdateProductPrice = "UpdateProductPrice";
        private const string _spGetInternalCategories = "GetInternalCategories";
        private const string _spGetCategories = "GetSFACategory";
        private const string _spUpdateProductInventory = "UpdateProductInventory";
        private const string _spGetForApprovalInventoryAdjustment = "GetForApprovalInventoryAdjustment";
        private const string _spApproveInventoryAdjustment = "ApproveInventoryAdjustment";
        
        private Product _product;

        public async Task ApproveInventoryAdjustment(InventoryAdjustment payload)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", payload.Id),
                new SqlParameter("@Action", payload.Action),
                new SqlParameter("@Supplier", payload.Supplier.Replace("'","''")),
                new SqlParameter("@Quantity", payload.Quantity),
                new SqlParameter("@ProductId", payload.ProductId),
                new SqlParameter("@IsApproved", payload.IsApproved),
                new SqlParameter("@ApprovedBy", payload.ApprovedBy.Replace("'","''"))
            };
            await _spApproveInventoryAdjustment.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task UpdateProductInventory(InventoryAdjustment payload)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@ProductId", payload.ProductId),
                new SqlParameter("@ProductName", payload.ProductName.Replace("'","''")),
                new SqlParameter("@OriginalQuantity", payload.OriginalQuantity),
                new SqlParameter("@Quantity", payload.Quantity),
                new SqlParameter("@Action", payload.Action),
                new SqlParameter("@Remarks", payload.Remarks.Replace("'","''")),
                new SqlParameter("@Supplier", payload.Supplier.Replace("'","''")),
                new SqlParameter("@RequestBy", payload.RequestBy.Replace("'","''"))
            };
            await _spUpdateProductInventory.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task<IEnumerable<InventoryAdjustment>> GetForApprovalInventoryAdjustment()
        {
            var list = await _spGetForApprovalInventoryAdjustment.GetList<InventoryAdjustment>();
            return list;
        }

        public async Task<IEnumerable<Product>> GetProducts(long id)
        {
            var list = await _spGetProducts.GetList<Product>(new SqlParameter("@Id", id));
            return list;
        }

        public async Task<IEnumerable<ProductInternalCategory>> GetInternalCategories()
        {
            return await _spGetInternalCategories.GetList<ProductInternalCategory>();
        }

        public async Task<IEnumerable<ProductSFACategory>> GetCategories()
        {
            return await _spGetCategories.GetList<ProductSFACategory>();
        }

        public async Task<IEnumerable<Product>> GetProductsByCustomer(long customerId)
        {
            var list = await _spGetProductsByCustomer.GetList<Product>(new SqlParameter("@CustomerId", customerId));
            return list;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await GetProducts(0);
        }

        public async Task RemoveProduct(long id)
        {
            var sqlParameters = new[]
            {
                new SqlParameter("@Id", id)
            };
            await _removeProduct.ExecuteNonQueryAsync(sqlParameters);
        }

        public async Task UpdateProductPrice(ProductPrice productPrice)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", productPrice.Id),
                new SqlParameter("@PriceListId", productPrice.PriceListId),
                new SqlParameter("@SalePrice", productPrice.SalePrice),
                new SqlParameter("@UnitDiscount", productPrice.UnitDiscount)
            };
            await _spUpdateProductPrice.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        private List<SqlParameter> GetSqlParameters()
        {

            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@SFAReferenceNo", _product.SFAReferenceNo.Replace("'", "''")),
                new SqlParameter("@Name", _product.Name.Replace("'", "''")),
                new SqlParameter("@Description", _product.Description.Replace("'", "''")),
                new SqlParameter("@Category", _product.Category.Replace("'", "''")),
                new SqlParameter("@InternalCategory", _product.InternalCategory.Replace("'", "''")),
                new SqlParameter("@UnitOfMeasure", _product.UnitOfMeasure.Replace("'", "''")),
                new SqlParameter("@SFAUnitOfMeasure", _product.SFAUnitOfMeasure.Replace("'", "''")),
                new SqlParameter("@SalePriceCORON", _product.SalePrice_CORON),
                new SqlParameter("@SalePriceLUBANG", _product.SalePrice_LUBANG),
                new SqlParameter("@SalePriceSANILDEFONSO", _product.SalePrice_SANILDEFONSO),
                new SqlParameter("@CostPrice", _product.CostPrice),
                new SqlParameter("@IsFeeds", _product.IsFeeds),
                new SqlParameter("@IsTaxable", _product.IsTaxable),
                new SqlParameter("@IsConvertibleToBag", _product.IsConvertibleToBag),
                new SqlParameter("@KiloPerUnit", _product.KiloPerUnit)
            };
            return sqlParameters;
        }

        public async Task<Product> SaveProduct(Product product)
        {
            //decimal id = 0;
            _product = product;
            var result =  await _insertProduct.GetRecord<Product>(GetSqlParameters().ToArray());

            //using (var connection = await DbHelper.GetOpenConnectionAsync())
            //{
            //    using (var reader = await connection.ExecuteReaderAsync(_insertProduct, GetSqlParameters().ToArray()))
            //    {
            //        while (reader.Read())
            //        {
            //            id = reader.GetAsync<decimal>("Id").Result;
            //        }
            //    }
            //}

            try
            {
                product.Id = result.Id; // Convert.ToInt64(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
            return product;
        }

        public async Task UpdateProduct(Product product)
        {
            _product = product;
            var sqlParams = GetSqlParameters();
            sqlParams.Add(new SqlParameter("@Id", _product.Id));
            await _updateProduct.ExecuteNonQueryAsync(sqlParams.ToArray());
        }

    }
}
