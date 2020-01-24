using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Purchase;

namespace Project.FC2J.DataStore.Interfaces
{
    public interface IPurchaseRepository : IRepository<PurchaseOrder>
    {
        Task Deliver(PurchaseOrder value);
        Task<PurchaseOrder> GetPurchaseOrder(string poNo);
        Task<PurchaseOrder> Save(PurchaseOrder value, bool isSendEmail);
        Task<POPayment> InsertPayment(POPayment value);
        Task<List<POPayment>> GetPayments(long id);
        Task DeletePayment(long id, string deletedBy);
        Task InsertInvoiceDetail(long poHeaderId, long productId, string invoiceNo);
        Task<List<PoHeader>> GetPurchases(string userName);
    }
}
