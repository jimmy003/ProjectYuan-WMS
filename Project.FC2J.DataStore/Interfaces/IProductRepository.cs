using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<IEnumerable<Product>> GetProducts(long id);
        Task<IEnumerable<Product>> GetProductsByCustomer(long customerId);
        Task<Product> SaveProduct(Product product);
        Task UpdateProduct(Product product);
        Task RemoveProduct(long id);
        Task UpdateProductPrice(ProductPrice productPrice);
        Task<IEnumerable<ProductInternalCategory>> GetInternalCategories();
        Task<IEnumerable<ProductSFACategory>> GetCategories();
        Task UpdateProductInventory(InventoryAdjustment payload);
        Task<IEnumerable<InventoryAdjustment>> GetForApprovalInventoryAdjustment();
        Task ApproveInventoryAdjustment(InventoryAdjustment payload);
    }
}