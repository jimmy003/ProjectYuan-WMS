using System;

namespace Project.FC2J.Models.Purchase
{
    public class PoHeader
    {
        public long Id { get; set; }
        public string PONo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public decimal PickUpDiscount { get; set; }
        public decimal Outright { get; set; }
        public decimal CashDiscount { get; set; }
        public decimal PromoDiscount { get; set; }
        public decimal OtherDeduction { get; set; }

        public double TotalQuantity { get; set; }
        public double TotalQuantityUOMComputed { get; set; }

        public decimal SubTotal { get; set; }
        public decimal TaxPrice { get; set; }
        public decimal Total { get; set; }

        public string UserName { get; set; }
        public DateTime SubmittedDate { get; set; }

        public string AcknowledgedUser { get; set; }
        public DateTime AcknowledgedDate { get; set; }

        public string DeliveredUser { get; set; }
        public DateTime DeliveredDate { get; set; }

        public long PriceListId { get; set; }
        public string SupplierName { get; set; }
        
        public int POStatusId { get; set; }
        public string POStatus { get; set; }
        public string SupplierEmail { get; set; }
        public bool IsVatable { get; set; }
    }
}
