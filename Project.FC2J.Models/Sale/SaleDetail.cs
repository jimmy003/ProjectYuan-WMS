using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.Models.Sale
{
    public class SaleDetail
    {
        public long Id { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }
        public float OrderQuantity { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public decimal ProductSalePrice { get; set; }
        public decimal TaxRate{ get; set; }
        public decimal SubTotalProductSalePrice { get; set; }
        public decimal SubTotalProductTaxPrice { get; set; }
        public decimal DeductionFixPrice { get; set; }
        public decimal DeductionOutright { get; set; }
        public double Discount { get; set; }
        public decimal DeductionCashDiscount { get; set; }
        public decimal DeductionPromoDiscount { get; set; }
        public string ProductUnitOfMeasure { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public bool IsTaxable { get; set; }

    }
}
