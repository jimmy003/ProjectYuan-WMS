using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Purchase;

namespace Project.FC2J.UI.Helpers.Purchase
{
    public interface IPurchaseEndpoint : IEndpoint<PurchaseOrder>
    {
        Task Delivered(PurchaseOrder value);
        Task<PurchaseOrder> GetPurchaseOrder(string poNo);
        Task<POPayment> InsertPayment(POPayment value);
        Task<List<POPayment>> GetPayments(long id);
        Task DeletePayment(string invoiceNo, long id, string deletedBy);
        Task InsertInvoiceDetail(long poHeaderId, long productId, string invoiceNo);
        Task<List<PoHeader>> GetPurchasesOrder(string userName);
    }
}
