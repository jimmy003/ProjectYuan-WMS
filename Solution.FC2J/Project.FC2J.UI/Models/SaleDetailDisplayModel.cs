using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.UI.Helpers;

namespace Project.FC2J.UI.Models
{
    public class SaleDetailDisplayModel : BaseDisplayModel
    {
        public long Id { get; set; }
        public int LineNo { get; set; }
        public long ProductId { get; set; }

        private float _orderQuantity;
        public float OrderQuantity
        {
            get { return _orderQuantity; }
            set { _orderQuantity = value; CallPropertyChanged(nameof(OrderQuantity)); }
        }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCategory { get; set; }
        public decimal TaxRate { get; set; }

        private decimal _subTotalProductSalePrice;
        public decimal SubTotalProductSalePrice
        {
            get { return _subTotalProductSalePrice; }
            set { _subTotalProductSalePrice = value; CallPropertyChanged(nameof(SubTotalProductSalePrice)); }
        }

        public decimal SubTotalProductTaxPrice { get; set; }
        public decimal DeductionOutright { get; set; }
        public double Discount { get; set; }
        public decimal DeductionCashDiscount { get; set; }
        public decimal DeductionPromoDiscount { get; set; }
        public string ProductUnitOfMeasure { get; set; }
        public bool IsTaxable { get; set; }

        private decimal _deductionFixPrice;
        public decimal DeductionFixPrice
        {
            get { return _deductionFixPrice; }
            set
            {
                _deductionFixPrice = value;
                CallPropertyChanged(nameof(DeductionFixPrice));
                CallPropertyChanged(nameof(Price));
            }
        }

        private decimal _productSalePrice;
        public decimal ProductSalePrice
        {
            get { return _productSalePrice; }
            set
            {
                _productSalePrice = value;
                CallPropertyChanged(nameof(ProductSalePrice));
                CallPropertyChanged(nameof(Price));
            }
        }


        private int _supplierId;
        public int SupplierId
        {
            get { return _supplierId; }
            set
            {
                _supplierId = value;
                CallPropertyChanged(nameof(SupplierId));
            }
        }

        private string _supplier;
        public string Supplier
        {
            get { return _supplier; }
            set
            {
                _supplier = value;
                CallPropertyChanged(nameof(Supplier));
            }
        }

        public string Price => (DeductionFixPrice > 0 ? DeductionFixPrice : ProductSalePrice).ToString("C").Substring(1);

    }
}
