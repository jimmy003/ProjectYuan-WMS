using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.Email;
using Project.FC2J.Models.Purchase;
using Project.FC2J.Models.Sale;

namespace Project.FC2J.DataStore.DataAccess
{
    public class PurchaseRepository : IPurchaseRepository
    {

        private readonly string _spInsertPurchaseHeader = "InsertPurchaseHeader";
        private readonly string _spInsertPurchaseDetail = "InsertPurchaseDetail";
        private readonly string _spAcknowledgedPO = "AcknowledgedPO";
        private readonly string _spDeliveredPO = "DeliveredPO";
        private readonly string _spDeliveredPOButNotAll = "DeliveredPOButNotAll";
        private readonly string _spUpdatePurchaseDetail = "UpdatePurchaseDetail";
        private readonly string _spCreateSaleInventory = "CreateSaleInventory";
        private readonly string _spGetPOHeaderByPONo = "GetPOHeaderByPONo";
        private readonly string _spGetPODetailsById = "GetPODetailsById";
        private readonly string _spUpdatePOStatusBasedOnItems = "UpdatePOStatusBasedOnItems";
        private readonly string _spInsertSendEmail = "InsertSendEmail";
        private readonly string _spUpdateSendEmail = "UpdateSendEmail";
        private readonly string _spInsertPayment_FC2J = "InsertPayment_FC2J";
        private readonly string _spGetPayments_FC2J = "GetPayments_FC2J";
        private readonly string _spDeletePayment_FC2J = "DeletePayment_FC2J";
        private readonly string _spInsertInvoiceDetail = "InsertInvoiceDetail";
        private readonly string _spGetPurchases = "GetPurchases";
        private List<SqlParameter> _sqlParameters;

        public Task<List<PurchaseOrder>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<List<PoHeader>> GetPurchases(string userName)
        {
            var value = await _spGetPurchases.GetList<PoHeader>(new SqlParameter("@UserName", userName));
            return value;
        }

        public Task<PurchaseOrder> Save(PurchaseOrder value)
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseOrder> GetPurchaseOrder(string poNo)
        {
            var po = new PurchaseOrder
            {
                PoHeader = await _spGetPOHeaderByPONo.GetRecord<PoHeader>(new SqlParameter("@PONo", poNo))
            };
            po.PoDetails = await _spGetPODetailsById.GetList<PoDetail>(new SqlParameter("@POHeaderId", po.PoHeader.Id));
            return po;
        }

        public async Task InsertInvoiceDetail(long poHeaderId, long productId, string invoiceNo)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@POHeaderId", poHeaderId),
                new SqlParameter("@ProductId", productId),
                new SqlParameter("@InvoiceNo", invoiceNo)
            };
            await _spInsertInvoiceDetail.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }

        public async Task DeletePayment(long id, string deletedBy)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@DeletedBy", deletedBy)
            };
            await _spDeletePayment_FC2J.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }

        public async Task<List<POPayment>> GetPayments(long id)
        {
            return await _spGetPayments_FC2J.GetList<POPayment>(new SqlParameter("@OrderHeaderId", id));
        }

       
        public async Task<POPayment> InsertPayment(POPayment value)
        {
            foreach (var item in value.items)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@POHeaderId", value.OrderHeaderId),
                    new SqlParameter("@ProductId", item.Id),
                    new SqlParameter("@InvoiceNo", value.InvoiceNo)
                };
                await _spInsertInvoiceDetail.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }

            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@OrderHeaderId", value.OrderHeaderId),
                new SqlParameter("@InvoiceNo", value.InvoiceNo),
                new SqlParameter("@InvoiceDate", value.InvoiceDate),
                new SqlParameter("@Amount", value.Amount),
                new SqlParameter("@UserName", value.UserName)
            };
            var result = await _spInsertPayment_FC2J.GetRecord<Placeholder>(_sqlParameters.ToArray());
            value.Id = result.Id;
            return value;
        }
        public async Task<PurchaseOrder> Save(PurchaseOrder value, bool isSendEmail)
        {
            #region Po Header
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@PONo", value.PoHeader.PONo),
                new SqlParameter("@PurchaseDate", value.PoHeader.PurchaseDate),
                new SqlParameter("@DeliveryDate", value.PoHeader.DeliveryDate),
                new SqlParameter("@PickUpDiscount", value.PoHeader.PickUpDiscount),
                new SqlParameter("@Outright", value.PoHeader.Outright),
                new SqlParameter("@CashDiscount", value.PoHeader.CashDiscount),
                new SqlParameter("@PromoDiscount", value.PoHeader.PromoDiscount),
                new SqlParameter("@OtherDeduction", value.PoHeader.OtherDeduction),
                new SqlParameter("@TotalQuantity", value.PoHeader.TotalQuantity),
                new SqlParameter("@TotalQuantityUOMComputed", value.PoHeader.TotalQuantityUOMComputed),
                new SqlParameter("@SubTotal", value.PoHeader.SubTotal),
                new SqlParameter("@TaxPrice", value.PoHeader.TaxPrice),
                new SqlParameter("@Total", value.PoHeader.Total),
                new SqlParameter("@UserName", value.PoHeader.UserName),
                new SqlParameter("@PriceListId", value.PoHeader.PriceListId),
                new SqlParameter("@SupplierName", value.PoHeader.SupplierName),
                new SqlParameter("@SupplierEmail", value.PoHeader.SupplierEmail),
                new SqlParameter("@IsVatable", value.PoHeader.IsVatable),
                new SqlParameter("@IsResubmit", value.PoHeader.AcknowledgedUser)

            };

            var result = await _spInsertPurchaseHeader.GetRecord<PoHeader>(_sqlParameters.ToArray());
            value.PoHeader.Id = result.Id;

            #endregion

            if (value.PoHeader.Id == -1) return value;

            #region Po Details
            foreach (var detail in value.PoDetails)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@POHeaderId", value.PoHeader.Id),
                    new SqlParameter("@LineNo", detail.LineNo),
                    new SqlParameter("@ProductId", detail.ProductId),
                    new SqlParameter("@Quantity", detail.Quantity),
                    new SqlParameter("@Name", detail.Name),
                    new SqlParameter("@Category", detail.Category),
                    new SqlParameter("@UnitOfMeasure", detail.UnitOfMeasure),
                    new SqlParameter("@SalePrice", detail.SalePrice),
                    new SqlParameter("@UnitDiscount", detail.UnitDiscount),
                    new SqlParameter("@SFAUnitOfMeasure", detail.SFAUnitOfMeasure),
                    new SqlParameter("@SFAReferenceNo", detail.SFAReferenceNo),
                    new SqlParameter("@SubTotal", detail.SubTotal),
                    new SqlParameter("@IsTaxable", detail.IsTaxable),
                    new SqlParameter("@TaxRate", detail.TaxRate),
                    new SqlParameter("@TaxPrice", detail.TaxPrice)

                };
                await _spInsertPurchaseDetail.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
            #endregion

            if (isSendEmail && string.IsNullOrWhiteSpace(value.PoHeader.SupplierEmail) == false)
            {
                //process Subject and Body
                var subject = OnCreateSubject(value.PoHeader);
                var body = OnCreateBody(value.PoHeader, value.PoDetails);
                //
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@To", value.PoHeader.SupplierEmail),
                    new SqlParameter("@Subject", subject),
                    new SqlParameter("@Body", body),
                    new SqlParameter("@UserName", value.PoHeader.UserName)
                };

                Placeholder emailPlaceholder = new Placeholder();
                try
                {
                    emailPlaceholder = await _spInsertSendEmail.GetRecord<Placeholder>(_sqlParameters.ToArray());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                }

                //Send email here 
                var emailResult = "Sent";
                try
                {
                    var emailPayload = new EmailPayload
                    {
                        To = value.PoHeader.SupplierEmail,
                        Subject = subject,
                        Body = body
                    };
                    await emailPayload.Send();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    emailResult = $"[Message={e.Message}] [StackTrace={e.StackTrace}]";
                }
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", emailPlaceholder.Id),
                    new SqlParameter("@Result", emailResult.Replace("'", "''"))
                };
                await _spUpdateSendEmail.ExecuteNonQueryAsync(_sqlParameters.ToArray());

            }

            return value;
        }

        private string OnCreateBody(PoHeader header, IEnumerable<PoDetail> valuePoDetails)
        {
            var body = new StringBuilder();
            body.AppendLine("<H2>Total Qty = "+header.TotalQuantity+ "</H2>");
            body.AppendLine("<H2>Total Tax = " + header.TaxPrice.ToString("N2") + "</H2>");
            body.AppendLine("<H2>Total Amount = " + header.Total.ToString("N2") + "</H2>");
            body.AppendLine("<br><br>");

            body.Append("<table border=\"1\">");
            body.Append("<tr bgcolor=\"blue\">");

            body.Append("<th><font color=\"#fff\">Material Code</font></th>");
            body.Append("<th><font color=\"#fff\">Description</font></th>");
            body.Append("<th><font color=\"#fff\">UOM</font></th>");
            body.Append("<th><font color=\"#fff\">Served Qty</font></th>");

            body.Append("<th><font color=\"#fff\">Net Weight</font></th>");
            body.Append("<th><font color=\"#fff\">Unit Selling Price</font></th>");
            body.Append("<th><font color=\"#fff\">Unit Discounts</font></th>");
            body.Append("<th><font color=\"#fff\">Amount</font></th>");

            body.Append("<th><font color=\"#fff\">Tax</font></th>");
            body.Append("<th><font color=\"#fff\">Transaction Type</font></th>");
            body.Append("</tr>");
            //<font color="#fff">Item Id</font>						
            foreach (var detail in valuePoDetails)
            {
                body.AppendLine("<tr>");

                body.Append("<td>"+detail.SFAReferenceNo+"</td>");
                body.Append("<td>"+detail.Name+"</td>");
                body.Append("<td>"+detail.SFAUnitOfMeasure+"</td>");
                body.Append("<td>" + detail.Quantity+"</td>");

                body.Append("<td>"+detail.NetWeight+"</td>");
                body.Append("<td>"+detail.SalePrice.ToString("N2")+"</td>");
                body.Append("<td>"+detail.UnitDiscount.ToString("N2") + "</td>");
                body.Append("<td>" + detail.SubTotal.ToString("N2") + "</td>");

                body.Append("<td>"+detail.TaxPrice.ToString("N2") + "</td>");
                body.Append("<td>"+detail.TaxType + "</td>");

                body.Append("</tr>");

            }
            body.AppendLine("</table>");
            return body.ToString();
        }

        private string OnCreateSubject(PoHeader valuePoHeader)
        {
            return $"FC2J Corporation - PO [{valuePoHeader.PONo}] - {DateTime.Now.ToString("ddMMMyyyy")}";
        }

        public async Task Update(PurchaseOrder value)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", value.PoHeader.Id),
                new SqlParameter("@UserName", value.PoHeader.UserName)
            };
            await _spAcknowledgedPO.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public async Task Deliver(PurchaseOrder value)
        {

            if (value.IsWithReturns == false)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", value.PoHeader.Id),
                    new SqlParameter("@UserName", value.PoHeader.UserName)
                };
                await _spDeliveredPO.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
            else
            {
                //Delivered but Not all 
                #region Po Header 
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", value.PoHeader.Id),
                    new SqlParameter("@UserName", value.PoHeader.UserName),

                    new SqlParameter("@PickUpDiscount", value.PoHeader.PickUpDiscount),
                    new SqlParameter("@Outright", value.PoHeader.Outright),
                    new SqlParameter("@CashDiscount", value.PoHeader.CashDiscount),
                    new SqlParameter("@PromoDiscount", value.PoHeader.PromoDiscount),
                    new SqlParameter("@OtherDeduction", value.PoHeader.OtherDeduction),

                    new SqlParameter("@TotalQuantity", value.PoHeader.TotalQuantity),
                    new SqlParameter("@TotalQuantityUOMComputed", value.PoHeader.TotalQuantityUOMComputed),
                    new SqlParameter("@SubTotal", value.PoHeader.SubTotal),
                    new SqlParameter("@TaxPrice", value.PoHeader.TaxPrice),
                    new SqlParameter("@Total", value.PoHeader.Total)
                };
                await _spDeliveredPOButNotAll.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                #endregion
            }

            #region Po Details

            //63  CORON
            //64  LUBANG
            //68  SAN ILDEFONSO
            var supplierId = 0;
            switch (value.PoHeader.PriceListId)
            {
                case 63:
                    supplierId = 0;
                    break;
                case 64:
                    supplierId = 1;
                    break;
                case 68:
                    supplierId = 2;
                    break;
            }

            foreach (var detail in value.PoDetails)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@Id", detail.Id),
                    new SqlParameter("@ProductId", detail.ProductId),
                    new SqlParameter("@IsDelivered", detail.IsDelivered),
                    new SqlParameter("@DeliveredUser", value.PoHeader.UserName),
                    new SqlParameter("@Quantity", detail.Quantity), //this will update StockQuantity of Product if the Delivered
                    new SqlParameter("@SupplierId", supplierId)
                };
                await _spUpdatePurchaseDetail.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                _sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProductId", detail.ProductId),
                    new SqlParameter("@CustomerId", "Z"),
                    new SqlParameter("@Quantity", detail.Quantity),
                    new SqlParameter("@PONo", value.PoHeader.PONo),
                    new SqlParameter("@SupplierId", supplierId)
                };
                await _spCreateSaleInventory.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
            #endregion

            //call on if with Returns
            if (value.IsWithReturns)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@POHeaderId", value.PoHeader.Id), // this shall be used to udpate status if all were delivered
                    new SqlParameter("@DeliveredUser", value.PoHeader.UserName)

                };
                await _spUpdatePOStatusBasedOnItems.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
        
        }


    }
}
