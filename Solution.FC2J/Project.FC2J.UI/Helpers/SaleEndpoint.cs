using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Sale;
using System.Net.Http;
using Project.FC2J.Models;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class SaleEndpoint : ISaleEndpoint
    {

        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;

        public SaleEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }

        public async Task<SalesInvoice> GetInvoiceNo()
        {
            var obj = await _apiHelper.GetRecord<SalesInvoice>(_apiAppSetting.InvoiceNo);
            return obj;
        }

        public async Task<List<OrderHeader>> GetSales(string userName)
        {
            return await _apiHelper.GetList<OrderHeader>(_apiAppSetting.Sale + $"?userName={userName}");
        }

        public async Task<List<OrderHeader>> GetCollection(string userName)
        {
            return await _apiHelper.GetList<OrderHeader>(_apiAppSetting.Sale + $"/Collections?userName={userName}");
        }

        public async Task<List<OrderHeader>> GetCollected(string userName, int isPaid)
        {
            return await _apiHelper.GetList<OrderHeader>(_apiAppSetting.Sale + $"/Collected?userName={userName}&isPaid={isPaid}");
        }

        public async Task<List<SaleDetail>> GetSaleDetails(long valueId, long customerId)
        {
            return await _apiHelper.GetList<SaleDetail>(_apiAppSetting.Sale + $"?id={valueId}&customerId={customerId}");
        }

        public async Task<List<Invoice>> GetInvoices()
        {
            return await _apiHelper.GetList<Invoice>(_apiAppSetting.Sale + @"/Invoices");
        }

        public async Task ReceiveInvoice(ReceiveInvoice receiveInvoice)
        {
            await _apiHelper.Save(_apiAppSetting.Sale + @"/Invoices", receiveInvoice);
        }

        public async Task<SaleHeader> GetSaleHeader(long customerId, long id)
        {
            return await _apiHelper.GetRecord<SaleHeader>(_apiAppSetting.Sale + $"/Invoices?customerId={customerId}&id={id}");
        }

        public async Task PayInvoice(SalePayment salePayment)
        {
            await _apiHelper.Update(_apiAppSetting.Sale + @"/Invoices", salePayment);
        }

        public async Task RetrievePaidBadSale(SalePayment salePayment)
        {
            await _apiHelper.Update(_apiAppSetting.Sale + @"/RetrieveInvoice", salePayment);
        }

        public async Task<string> GetSONo()
        {
            var obj = await _apiHelper.GetRecord<SalesInvoice>(_apiAppSetting.SaleSONo);
            return obj.SONo;
        }

        public async Task<SaleHeader> PostSale(SaleHeader sale)
        {
            return await _apiHelper.GetRecord<SaleHeader>(_apiAppSetting.Sale, sale);
        }

        public async Task<List<OrderHeader>> GetSalesForPrint(string userName)
        {
            return await _apiHelper.GetList<OrderHeader>(_apiAppSetting.Sale + @"/ForPrint" + $"?userName={userName}");
        }

        public async Task UpdatePONumber(string customerId, string _poNo, string _newPoNO, long salesId)
        {
            await _apiHelper.Update(_apiAppSetting.Sale + @"/UpdatePONumber" + $"?customerId={customerId}&poNo={_poNo}&newPoNO={_newPoNO}&salesId={salesId}");
        }
    }
}
