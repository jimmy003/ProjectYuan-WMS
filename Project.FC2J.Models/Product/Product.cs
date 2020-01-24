using System;

namespace Project.FC2J.Models.Product
{
    public class Product
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal SalePrice_CORON { get; set; }
        public decimal SalePrice_LUBANG { get; set; }
        public decimal SalePrice_SANILDEFONSO { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public decimal UnitDiscount { get; set; }
        public string SFAUnitOfMeasure { get; set; }
        public string SFAReferenceNo { get; set; }
        /// <summary>
        /// Resolved from the Product table
        /// </summary>
        public double StockQuantity { get; set; } 
        public double Stock_CORON { get; set; } 
        public double Stock_LUBANG { get; set; } 
        public double Stock_SANILDEFONSO { get; set; } 
        public bool IsTaxable { get; set; }
        public bool Deleted { get; set; }
        public bool IsDelivered { get; set; }
        public string ProductType { get; set; }
        public bool CanBeSold { get; set; }
        public bool CanBePurchased { get; set; }
        public string Barcode { get; set; }
        public string InternalCategory { get; set; }
        public string PurchaseUnitOfMeasure { get; set; }
        public string ControlPurchaseBills { get; set; }
        public decimal DeductionFixPrice { get; set; }
        public decimal DeductionOutright { get; set; }
        public double Discount { get; set; }
        public decimal DeductionCashDiscount { get; set; }
        public decimal DeductionPromoDiscount { get; set; }
        public string Image { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsEditable { get; set; }
        public bool IsFeeds { get; set; }
        public bool IsConvertibleToBag { get; set; }
        public decimal KiloPerUnit { get; set; }

    }
}
