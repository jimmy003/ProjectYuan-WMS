using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;

namespace Project.FC2J.UI.Helpers
{
    public interface IProductEndpoint : IEndpoint<Product>
    {
        Task<List<Product>> GetList(long id);
    }
}