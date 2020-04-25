using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Purchase;
using Project.FC2J.Models.Sale;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers.Purchase
{
    public class PurchaseEndpoint : IPurchaseEndpoint
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;

        public PurchaseEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }
        public Task<List<PurchaseOrder>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrder> Save(PurchaseOrder value)
        {
            return await _apiHelper.GetRecord<PurchaseOrder>(_apiAppSetting.Purchase, value);
        }

        public async Task Update(PurchaseOrder value)
        {
            await _apiHelper.Update(_apiAppSetting.Purchase, value);
        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public async Task Delivered(PurchaseOrder value)
        {
            await _apiHelper.Update(_apiAppSetting.Purchase + "/Deliver", value);
        }

        public async Task<PurchaseOrder> GetPurchaseOrder(string poNo)
        {
            return await _apiHelper.GetRecord<PurchaseOrder>(_apiAppSetting.Purchase + $"?poNo={poNo}");
        }

        public async Task<POPayment> InsertPayment(POPayment value)
        {
            try
            {
                return await _apiHelper.Save(_apiAppSetting.Purchase + "/Payment", value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<List<POPayment>> GetPayments(long id)
        {
            return await _apiHelper.GetList<POPayment>(_apiAppSetting.Purchase + $"/Payment?id={id}");
        }

        public async Task DeletePayment(string invoiceNo, long id, string deletedBy)
        {
            await _apiHelper.Remove(_apiAppSetting.Purchase +  $"/Payment?id={id}&deletedBy={deletedBy}&invoiceNo={invoiceNo}");
        }

        public async Task InsertInvoiceDetail(long poHeaderId, long productId, string invoiceNo)
        {
            await _apiHelper.Update(_apiAppSetting.Purchase + $"/InsertInvoiceDetail?poHeaderId={poHeaderId}&productId={productId}&invoiceNo={invoiceNo}");
        }

        public async Task<List<PoHeader>> GetPurchasesOrder(string userName)
        {
            return await _apiHelper.GetList<PoHeader>(_apiAppSetting.Purchase + $"/GetList?userName={userName}");
        }
    }
}
