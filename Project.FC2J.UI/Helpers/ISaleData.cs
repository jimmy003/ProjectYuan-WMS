
using Project.FC2J.Models.Sale;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Helpers
{
    public interface ISaleData
    {
        bool IsFromSalesList { get; set; }
        SaleHeader Value { get; set; }
        
        Task Authenticate(string username, string password);
    }
}
