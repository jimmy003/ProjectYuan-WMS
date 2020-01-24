using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Product;
using Project.FC2J.Models.Report;

namespace Project.FC2J.UI.Helpers.Products
{
    public interface IProductEndpoint : IEndpoint<FC2J.Models.Product.Product>
    {
        Task<List<FC2J.Models.Product.Product>> GetList(long id);
        Task UpdateProductPrice(ProductPrice productPrice);
        Task<IEnumerable<ProductInternalCategory>> GetInternalCategories();
        Task<IEnumerable<ProductSFACategory>> GetCategories();
        Task UpdateProductInventory(InventoryAdjustment payload);
        Task<IEnumerable<InventoryAdjustment>> GetForApprovalInventoryAdjustment();
        Task ApproveInventoryAdjustment(InventoryAdjustment payload);
    }
}