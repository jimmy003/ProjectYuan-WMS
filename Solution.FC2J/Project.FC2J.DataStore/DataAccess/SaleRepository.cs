using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.Models.Order;
using Project.FC2J.Models.Sale;

namespace Project.FC2J.DataStore.DataAccess
{
    public enum CancelledOrReturnedEnum
    {
        None =0,
        Cancelled =1,
        Returned = 2
    }

    public class SaleRepository : ISaleRepository
    {
        private readonly string _spGetSONo = "GetSONo";
        private readonly string _spGetInvoiceNo = "GetInvoiceNo";
        private readonly string _spInsertSaleHeader = "InsertSaleHeader";
        private readonly string _spInsertSaleDetail = "InsertSaleDetail";
        private readonly string _spClearSaleInventoryByPONo = "ClearSaleInventoryByPONo";
        private readonly string _spCreateSaleInventory = "CreateSaleInventory";
        private readonly string _spUpdateSaleHeader = "UpdateSaleHeader";
        private readonly string _spValidateSaleHeader = "ValidateSaleHeader";
        private readonly string _spGetSales = "GetSales";
        private readonly string _spUpdatePONo = "UpdatePONo";
        private readonly string _spGetSalesForPrint = "GetSalesPrint";
        private readonly string _spGetCollections = "GetCollections";
        private readonly string _spGetCollected = "GetCollected";
        private readonly string _spGetSaleDetails = "GetSaleDetails";
        private readonly string _spGetSaleHeader = "GetSaleHeader";
        private readonly string _spCancelSaleOrder = "CancelSaleOrder";
        private readonly string _spManageSaleDeduction = "ManageSaleDeduction";

        private readonly string _spManageInvoice = "ManageInvoice";
        private readonly string _spUpdateSaleDetailWithReturns = "UpdateSaleDetailWithReturns";
        private readonly string _spUpdateSaleHeaderWithReturns = "UpdateSaleHeaderWithReturns";

        private readonly string _spInsertSaleHeaderPayment = "InsertSaleHeaderPayment";
        private readonly string _spRetrievePaidBadSale = "RetrievePaidBadSale";

        private List<SqlParameter> _sqlParameters;

        public async Task<SalesInvoice> GetSalesInvoice()
        {
            var value = await _spGetInvoiceNo.GetRecord<SalesInvoice>();
            return value;
        }

        public async Task<List<Invoice>> GetInvoices()
        {
            var value = await _spManageInvoice.GetList<Invoice>();           
            return value;
        }

        public async Task ReceivedInvoice(ReceiveInvoice receiveInvoice)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@t", 1),
                new SqlParameter("@Id", receiveInvoice.Invoice.Id),
                new SqlParameter("@PONo", receiveInvoice.Invoice.PONo),
                new SqlParameter("@CustomerId", receiveInvoice.Invoice.CustomerId),
                new SqlParameter("@WithReturns", receiveInvoice.Invoice.WithReturns)
            };

            await _spManageInvoice.ExecuteNonQueryAsync(_sqlParameters.ToArray());

            foreach (var _return in receiveInvoice.Returns)
            {
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@CustomerId", receiveInvoice.Invoice.CustomerId),
                    new SqlParameter("@OrderDetailId", _return.Id),
                    new SqlParameter("@ProductId", _return.ProductId),
                    new SqlParameter("@ReturnQuantity", _return.OrderQuantity)
                };
                await _spUpdateSaleDetailWithReturns.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                //Must Create Returned Inventory to negate the made transaction validated/delivered
                foreach (var item in receiveInvoice.Returns)
                {
                    _sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@ProductId", item.ProductId),
                        new SqlParameter("@CustomerId", receiveInvoice.Invoice.CustomerId),
                        new SqlParameter("@Quantity", item.OrderQuantity),
                        new SqlParameter("@PONo", receiveInvoice.Invoice.PONo),
                        new SqlParameter("@SupplierId", item.SupplierId),
                        new SqlParameter("@CancelledOrReturned", CancelledOrReturnedEnum.Returned)
                    };
                    await _spCreateSaleInventory.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                }

            }

            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", receiveInvoice.Invoice.CustomerId),
                new SqlParameter("@Id", receiveInvoice.Invoice.Id),
                new SqlParameter("@TotalOrderQuantity", receiveInvoice.SaleHeader.TotalOrderQuantity),
                new SqlParameter("@TotalOrderQuantityUOMComputed", receiveInvoice.SaleHeader.TotalOrderQuantityUOMComputed),

                new SqlParameter("@TotalProductSalePrice", receiveInvoice.SaleHeader.TotalProductSalePrice),
                new SqlParameter("@TotalProductTaxPrice", receiveInvoice.SaleHeader.TotalProductTaxPrice),

                new SqlParameter("@PickUpDiscount", receiveInvoice.SaleHeader.PickUpDiscount),

                new SqlParameter("@TotalDeductionPrice", receiveInvoice.SaleHeader.TotalDeductionPrice),
                new SqlParameter("@Outright", receiveInvoice.SaleHeader.Outright),
                new SqlParameter("@CashDiscount", receiveInvoice.SaleHeader.CashDiscount),
                new SqlParameter("@PromoDiscount", receiveInvoice.SaleHeader.PromoDiscount),

                new SqlParameter("@LessPrice", receiveInvoice.SaleHeader.LessPrice),
                new SqlParameter("@TotalPrice", receiveInvoice.SaleHeader.TotalPrice),
                new SqlParameter("@Total", receiveInvoice.SaleHeader.Total),
                new SqlParameter("@ReceivedUser", receiveInvoice.SaleHeader.UserName),

                new SqlParameter("@OrderStatusId", receiveInvoice.SaleHeader.OrderStatusId)

            };

            await _spUpdateSaleHeaderWithReturns.ExecuteNonQueryAsync(_sqlParameters.ToArray());


            //re-set the used deductions in reference to PONo, if all items are returned 
            if (receiveInvoice.SaleHeader.OrderStatusId == (long)OrderStatusEnum.RETURNEDALL )
            {
                _sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@t", 9),
                    new SqlParameter("@PONo", receiveInvoice.Invoice.PONo),
                    new SqlParameter("@CustomerId", receiveInvoice.Invoice.CustomerId)
                };
                await _spManageSaleDeduction.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }

        }

        public async Task PayInvoice(SalePayment salePayment)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", salePayment.CustomerId),
                new SqlParameter("@Id", salePayment.Id),
                new SqlParameter("@UserName", salePayment.UserName),
                new SqlParameter("@OrderStatusId", salePayment.OrderStatusId),

                new SqlParameter("@PONo", salePayment.PONo),
                new SqlParameter("@PaidAmount", salePayment.PaidAmount),

                new SqlParameter("@IsCash", salePayment.IsCash),
                new SqlParameter("@CashRemarks", salePayment.CashRemarks),

                new SqlParameter("@CheckBank", salePayment.CheckBank),
                new SqlParameter("@CheckNumber", salePayment.CheckNumber),
                new SqlParameter("@CheckDate", salePayment.CheckDate)
            };

            await _spInsertSaleHeaderPayment.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }

        public async Task RetrievePaidBadSale(SalePayment salePayment)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", salePayment.CustomerId),
                new SqlParameter("@Id", salePayment.Id),
                new SqlParameter("@UserName", salePayment.UserName)
            };

            await _spRetrievePaidBadSale.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }

        public async Task<List<OrderHeader>> GetSales(string userName)
        {
            var value = await _spGetSales.GetList<OrderHeader>(new SqlParameter("@UserName", userName));
            return value;
        }

        public async Task<List<OrderHeader>> GetCollections(string userName, int nearDueDays)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@NearDueDays", nearDueDays)
            };
            var value = await _spGetCollections.GetList<OrderHeader>(_sqlParameters.ToArray());
            return value;
        }

        public async Task<List<OrderHeader>> GetCollected(string userName, int isPaid)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@UserName", userName),
                new SqlParameter("@IsPaid", isPaid)
            };
            var value = await _spGetCollected.GetList<OrderHeader>(_sqlParameters.ToArray());
            return value;
        }

        public async Task<SaleHeader> GetSaleHeader(long customerId, long id)
        {
            _sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@id", id)
            };
            var value = await _spGetSaleHeader.GetRecord<SaleHeader>(_sqlParameters.ToArray());
            return value;
        }
        
        public async Task<List<SaleDetail>> GetSaleDetails(long id, long customerId)
        {
            _sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@CustomerId", customerId),
            };

            var value = await _spGetSaleDetails.GetList<SaleDetail>(_sqlParameters.ToArray());
            return value;
        }

        public async Task<string> GetSONo()
        {
            string soNo = string.Empty;

            using (var connection = await DbHelper.GetOpenConnectionAsync())
            {
                using (var reader = await connection.ExecuteReaderAsync(_spGetSONo))
                {
                    while (reader.Read())
                    {
                        soNo = reader.GetAsync<string>("SONo").Result;
                        break;
                    }
                }
            }

            return soNo;
        }

        public async Task<SaleHeader> PostSale(SaleHeader sale)
        {
            var result = new SaleHeader { Id = sale.Id};


            if (sale.Id > 0)
            {
                switch (sale.OrderStatusId)
                {
                    case 1:

                        #region parameters for UpdateSaleHeader
                        _sqlParameters = new List<SqlParameter>()
                        {
                            new SqlParameter("@Id", sale.Id),
                            new SqlParameter("@UserName", sale.UserName),

                            new SqlParameter("@PaymentTypeId", sale.SelectedPaymentTypeId),
                            new SqlParameter("@DeliveryDate", sale.DeliveryDate),
                            new SqlParameter("@DueDate", sale.DueDate),

                            new SqlParameter("@TotalOrderQuantity", sale.TotalOrderQuantity),
                            new SqlParameter("@TotalOrderQuantityUOMComputed", sale.TotalOrderQuantityUOMComputed),

                            new SqlParameter("@TotalProductSalePrice", sale.TotalProductSalePrice),
                            new SqlParameter("@TotalProductTaxPrice", sale.TotalProductTaxPrice),
                            new SqlParameter("@TotalDeductionPrice", sale.TotalDeductionPrice),

                            new SqlParameter("@PickupDiscount", sale.PickUpDiscount),
                            new SqlParameter("@Outright", sale.Outright),
                            new SqlParameter("@CashDiscount", sale.CashDiscount),
                            new SqlParameter("@PromoDiscount", sale.PromoDiscount),

                            new SqlParameter("@LessPrice", sale.LessPrice),
                            new SqlParameter("@TotalPrice", sale.TotalPrice),
                            new SqlParameter("@Total", sale.Total),

                            new SqlParameter("@CustomerId", sale.CustomerId)
                        };
                        await _spUpdateSaleHeader.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                        await ProcessSaleDetails(sale.CustomerId, sale, sale.Id.ToString());

                        //-- reset the deductions using Id only with PONo
                        _sqlParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@t", 9),
                            new SqlParameter("@PONo", sale.PONo),
                            new SqlParameter("@CustomerId", sale.CustomerId)
                        };
                        await _spManageSaleDeduction.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                        if (sale.Deductions.Count > 0)
                        {
                            foreach (var deduction in sale.Deductions)
                            {
                                _sqlParameters = new List<SqlParameter>
                                {
                                    new SqlParameter("@t", 7),
                                    new SqlParameter("@PONo", sale.PONo),
                                    new SqlParameter("@Id", deduction),
                                    new SqlParameter("@CustomerId", sale.CustomerId)
                                };
                                await _spManageSaleDeduction.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                            }
                        }
                        #endregion
                        break;

                    case 2:
                    case 4:
                    {
                        _sqlParameters = new List<SqlParameter>()
                        {
                            new SqlParameter("@Id", sale.Id),
                            new SqlParameter("@Revalidate", sale.Revalidate),
                            new SqlParameter("@OverrideUser", sale.OverrideUser),
                            new SqlParameter("@PONo", sale.PONo),

                            new SqlParameter("@PaymentTypeId", sale.SelectedPaymentTypeId),
                            new SqlParameter("@DeliveryDate", sale.DeliveryDate),
                            new SqlParameter("@DueDate", sale.DueDate),

                            new SqlParameter("@TotalOrderQuantity", sale.TotalOrderQuantity),
                            new SqlParameter("@TotalOrderQuantityUOMComputed", sale.TotalOrderQuantityUOMComputed),

                            new SqlParameter("@TotalProductSalePrice", sale.TotalProductSalePrice),
                            new SqlParameter("@TotalProductTaxPrice", sale.TotalProductTaxPrice),

                            new SqlParameter("@TotalDeductionPrice", sale.TotalDeductionPrice),

                            new SqlParameter("@PickupDiscount", sale.PickUpDiscount),
                            new SqlParameter("@Outright", sale.Outright),
                            new SqlParameter("@CashDiscount", sale.CashDiscount),
                            new SqlParameter("@PromoDiscount", sale.PromoDiscount),

                            new SqlParameter("@LessPrice", sale.LessPrice),
                            new SqlParameter("@TotalPrice", sale.TotalPrice),
                            new SqlParameter("@Total", sale.Total),
                            new SqlParameter("@OrderStatusId", sale.OrderStatusId),
                            new SqlParameter("@CustomerId", sale.CustomerId),
                            new SqlParameter("@IsVatable", sale.IsVatable)
                        };
                        
                        await _spValidateSaleHeader.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                        if (sale.Revalidate)
                        {
                            await ProcessSaleDetails(sale.CustomerId, sale, sale.Id.ToString());

                            if (sale.Deductions.Count > 0)
                            {
                                foreach (var deduction in sale.Deductions)
                                {
                                    _sqlParameters = new List<SqlParameter>
                                    {
                                        new SqlParameter("@t", 7),
                                        new SqlParameter("@PONo", sale.PONo),
                                        new SqlParameter("@Id", deduction),
                                        new SqlParameter("@CustomerId", sale.CustomerId)
                                    };
                                    await _spManageSaleDeduction.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                                }
                            }
                        }
                        break;

                    }
                    case 3:

                        _sqlParameters = new List<SqlParameter>()
                        {
                            new SqlParameter("@Id", sale.Id),
                            new SqlParameter("@CustomerId", sale.CustomerId),
                            new SqlParameter("@PONo", sale.PONo),
                            new SqlParameter("@OverrideUser", sale.OverrideUser)
                        };
                        await _spCancelSaleOrder.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                        //Must Create Cancelled Inventory to negate the 
                        foreach (var item in sale.SaleDetails)
                        {
                            _sqlParameters = new List<SqlParameter>
                            {
                                new SqlParameter("@ProductId", item.ProductId),
                                new SqlParameter("@CustomerId", sale.CustomerId),
                                new SqlParameter("@Quantity", item.OrderQuantity),
                                new SqlParameter("@PONo", sale.PONo),
                                new SqlParameter("@SupplierId", item.SupplierId),
                                new SqlParameter("@CancelledOrReturned", CancelledOrReturnedEnum.Cancelled)
                            };
                            await _spCreateSaleInventory.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                        }

                        //_sqlParameters = new List<SqlParameter>
                        //{
                        //    new SqlParameter("@PONo", sale.PONo),
                        //    new SqlParameter("@CustomerId", sale.CustomerId),
                        //    new SqlParameter("@SupplierId", sale.SaleDetails[0].SupplierId)
                        //};
                        //await _spClearSaleInventoryByPONo.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                        //-- reset the deductions using Id only with PONo
                        _sqlParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@t", 9),
                            new SqlParameter("@PONo", sale.PONo),
                            new SqlParameter("@CustomerId", sale.CustomerId)
                        };
                        await _spManageSaleDeduction.ExecuteNonQueryAsync(_sqlParameters.ToArray());


                        break;
                }
            }
            else
            {

                #region parameters for InsertHeaderr
                _sqlParameters = new List<SqlParameter>()
                {
                    new SqlParameter("@SONo", sale.SONo),
                    new SqlParameter("@UserName", sale.UserName),
                    new SqlParameter("@InvoiceNo", sale.InvoiceNo),
                    new SqlParameter("@PONo", sale.PONo),
                    new SqlParameter("@OrderDate", sale.OrderDate),
                    new SqlParameter("@DeliveryDate", sale.DeliveryDate),
                    new SqlParameter("@DueDate", sale.DueDate),

                    new SqlParameter("@TotalOrderQuantity", sale.TotalOrderQuantity),
                    new SqlParameter("@TotalOrderQuantityUOMComputed", sale.TotalOrderQuantityUOMComputed),

                    new SqlParameter("@TotalProductSalePrice", sale.TotalProductSalePrice),
                    new SqlParameter("@TotalProductTaxPrice", sale.TotalProductTaxPrice),
                    new SqlParameter("@TotalDeductionPrice", sale.TotalDeductionPrice),

                    new SqlParameter("@PickupDiscount", sale.PickUpDiscount),
                    new SqlParameter("@Outright", sale.Outright),
                    new SqlParameter("@CashDiscount", sale.CashDiscount),
                    new SqlParameter("@PromoDiscount", sale.PromoDiscount),

                    new SqlParameter("@LessPrice", sale.LessPrice),
                    new SqlParameter("@TotalPrice", sale.TotalPrice),
                    new SqlParameter("@Total", sale.Total),

                    new SqlParameter("@CustomerId", sale.CustomerId),
                    new SqlParameter("@PaymentTypeId", sale.SelectedPaymentTypeId),
                    new SqlParameter("@OrderStatusId", sale.OrderStatusId),
                    new SqlParameter("@IsVatable", sale.IsVatable)
                };

                result = await _spInsertSaleHeader.GetRecord<SaleHeader>(_sqlParameters.ToArray());

                await ProcessSaleDetails(sale.CustomerId, sale, result.Id.ToString());

                

                #endregion

            }

            return result;

        }

        private async Task ProcessSaleDetails(long customerId, SaleHeader sale, string id)
        {

            if (sale.OrderStatusId == 2 || sale.OrderStatusId == 4)
            {
                _sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PONo", sale.PONo),
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@SupplierId", sale.SaleDetails[0].SupplierId)
                };
                await _spClearSaleInventoryByPONo.ExecuteNonQueryAsync(_sqlParameters.ToArray());
            }
            foreach (var item in sale.SaleDetails)
            {
                _sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerId),
                    new SqlParameter("@OrderHeaderId", Convert.ToInt64(id)),
                    new SqlParameter("@LineNo", item.LineNo),
                    new SqlParameter("@ProductId", item.ProductId),
                    new SqlParameter("@OrderQuantity", item.OrderQuantity),
                    new SqlParameter("@TaxRate", item.TaxRate),
                    new SqlParameter("@SubTotalProductSalePrice", item.SubTotalProductSalePrice),
                    new SqlParameter("@SubTotalProductTaxPrice", item.SubTotalProductTaxPrice),
                    new SqlParameter("@PriceListId", sale.PriceListId),
                    new SqlParameter("@SupplierId", item.SupplierId),
                    new SqlParameter("@Supplier", item.Supplier),
                    new SqlParameter("@OrderStatusId", sale.OrderStatusId)
                };
                await _spInsertSaleDetail.ExecuteNonQueryAsync(_sqlParameters.ToArray());

                if (sale.OrderStatusId == 2 || sale.OrderStatusId == 4)
                {
                    _sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@ProductId", item.ProductId),
                        new SqlParameter("@CustomerId", customerId),
                        new SqlParameter("@Quantity", item.OrderQuantity),
                        new SqlParameter("@PONo", sale.PONo),
                        new SqlParameter("@SupplierId", item.SupplierId)
                    };
                    await _spCreateSaleInventory.ExecuteNonQueryAsync(_sqlParameters.ToArray());
                }
            }
        }

        public async Task<List<OrderHeader>> GetSalesForPrint(string userName)
        {
            var value = await _spGetSalesForPrint.GetList<OrderHeader>(new SqlParameter("@UserName", userName));
            return value;
        }

        public async Task UpdatePONumber(string customerId, string poNo, string newPoNO, long salesId)
        {

            _sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@CustomerId", customerId),
                        new SqlParameter("@OldPONo", poNo),
                        new SqlParameter("@NewPONo", newPoNO),
                        new SqlParameter("@Id", salesId)
                    };
            await _spUpdatePONo.ExecuteNonQueryAsync(_sqlParameters.ToArray());
        }
    }
}
