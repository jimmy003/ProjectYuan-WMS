using Project.FC2J.Models;
using Project.FC2J.Models.Sale;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Project.FC2J.UI.Helpers
{
    public interface ISaleEndpoint
    {
        Task<SaleHeader> PostSale(SaleHeader sale);
        Task<string> GetSONo();
        Task<SalesInvoice> GetInvoiceNo();
        Task<List<OrderHeader>> GetSales(string userName);
        Task<List<OrderHeader>> GetSalesForPrint(string userName);
        Task<List<OrderHeader>> GetCollection(string userName);
        Task<List<OrderHeader>> GetCollected(string userName, int isPaid);
        Task<List<SaleDetail>> GetSaleDetails(long valueId, long customerId);
        Task<List<Invoice>> GetInvoices();
        Task ReceiveInvoice(ReceiveInvoice receiveInvoice);
        Task<SaleHeader> GetSaleHeader(long customerId, long id);
        Task PayInvoice(SalePayment salePayment);
        Task RetrievePaidBadSale(SalePayment salePayment);
        Task UpdatePONumber(string customerId, string _poNo, string _newPoNO, long salesId);
    }
}
