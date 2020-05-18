using System;
using System.Collections.Generic;

namespace Project.FC2J.Models.Sale
{
    public class SaleHeader
    {
        public long Id { get; set; }
        public string SONo { get; set; }
        public string PONo { get; set; }
        public string InvoiceNo { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate{ get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DueDate { get; set; }
        public float TotalOrderQuantity { get; set; }
        public float TotalOrderQuantityUOMComputed { get; set; }
        public decimal TotalProductSalePrice { get; set; }
        public decimal TotalProductTaxPrice { get; set; }
        public decimal TotalDeductionPrice { get; set; }
        public decimal Outright { get; set; }
        public decimal CashDiscount { get; set; }
        public decimal PickUpDiscount { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal LessPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Total { get; set; } //Gross Price
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string MobileNo { get; set; }
        public string TelNo { get; set; }
        public string TIN { get; set; }
        public string SFAReferenceNo { get; set; }
        public long OrderStatusId { get; set; }
        public long OldOrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public bool Revalidate { get; set; }
        public long SelectedPaymentTypeId { get; set; }
        public string SelectedPaymentType { get; set; }
        public string OverrideUser { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime ValidatedDate { get; set; }
        public DateTime CancelledDate { get; set; }
        public bool Deleted { get; set; }
        public bool IsVatable { get; set; }

        public long PriceListId { get; set; }

        public DateTime CollectedDate { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string ReceivedUser { get; set; }
        public string CollectedUser { get; set; }
        public decimal PaidAmount { get; set; }

        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
        public List<long> Deductions { get; set; } = new List<long>();
    }
}
