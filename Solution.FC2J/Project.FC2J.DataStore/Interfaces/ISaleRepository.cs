using System.Collections.Generic;
using System.Threading.Tasks;
using Project.FC2J.Models.Sale;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface ISaleRepository
    {
        Task<SaleHeader> PostSale(SaleHeader sale);
        Task<string> GetSONo();
        Task<SalesInvoice> GetSalesInvoice();
        Task<List<Invoice>> GetInvoices();
        Task<List<OrderHeader>> GetSales(string userName);
        Task<List<OrderHeader>> GetSalesForPrint(string userName);        
        Task<List<OrderHeader>> GetCollections(string userName, int nearDueDays);
        Task<List<OrderHeader>> GetCollected(string userName, int isPaid);

        Task<List<SaleDetail>> GetSaleDetails(long id, long customerId);
        Task ReceivedInvoice(ReceiveInvoice receiveInvoice);
        Task<SaleHeader> GetSaleHeader(long customerId, long id);
        Task PayInvoice(SalePayment salePayment);
        Task RetrievePaidBadSale(SalePayment salePayment);
        Task UpdatePONumber(string customerId, string poNo, string newPoNO, long salesId);
        
    }
}
