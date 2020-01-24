using System;

namespace Project.FC2J.UI.Models
{
    public class CartItemDisplayModel : BaseDisplayModel
    {

        public ProductDisplayModel Product { get; set;}

        private float _cartQuantity;

        public float CartQuantity
        {
            get { return _cartQuantity; }
            set
            {
                _cartQuantity = value;
                CallPropertyChanged(nameof(CartQuantity));
                CallPropertyChanged(nameof(DisplayText));
                CallPropertyChanged(nameof(Description));
                CallPropertyChanged(nameof(SalePrice));
                CallPropertyChanged(nameof(CostPrice));
                CallPropertyChanged(nameof(SubTotal));
                CallPropertyChanged(nameof(SubTotalCostPrice));

                CallPropertyChanged(nameof(FixPrice));
                CallPropertyChanged(nameof(Outright));
                CallPropertyChanged(nameof(Promo));
                CallPropertyChanged(nameof(Discount));
                CallPropertyChanged(nameof(CashDiscount));
                CallPropertyChanged(nameof(NetWeight));
                CallPropertyChanged(nameof(UnitDiscount));
                CallPropertyChanged(nameof(PoTax));
                CallPropertyChanged(nameof(TaxType));
                CallPropertyChanged(nameof(SFAReferenceNo));
                
            }
        }

        private decimal _poTaxRage;

        public decimal PoTaxRate
        {
            get { return _poTaxRage; }
            set
            {
                _poTaxRage = value;
                CallPropertyChanged(nameof(PoTaxRate));
            }
        }


        //public bool IsDelivered => Product.IsDelivered;
        private bool _isDelivered;
        public bool IsDelivered
        {
            get { return _isDelivered; }
            set
            {
                _isDelivered = value;
                CallPropertyChanged(nameof(IsDelivered));
            }
        }



        public decimal PoTax => Product.IsTaxable ? SubTotalCostPrice * PoTaxRate : 0;
        public string TaxType => Product.IsTaxable ? "VATABLE" : "VAT EXEMPT";
        public string DisplayText => $"{Product.Name} ({CartQuantity})";
        public string Description => $"{Product.Name}";
        public string SFAReferenceNo => $"{Product.SFAReferenceNo}";
        
        public string UOM => $"{Product.SFAUnitOfMeasure}";

        public string NetWeight
        {
            get
            {
                var data = Product.UnitOfMeasure.ToLower().Trim();
                var number = string.Empty;
                var uom = string.Empty;

                if (data.Contains("kg"))
                {
                    number = data.Replace("kg", string.Empty);
                    number = GetNumber(number);
                    uom = "KG";
                }
                if (data.Contains("unit(s)"))
                {
                    number = data.Replace("unit(s)", string.Empty);
                    number = GetNumber(number);
                    uom = "Unit(s)";
                }
                if (data.Contains("liter(s)"))
                {
                    number = data.Replace("liter(s)", string.Empty);
                    number = GetNumber(number);
                    uom = "Liter(s)";
                }

                try
                {
                    var value = (decimal)CartQuantity * Convert.ToDecimal(number);
                    return $"{value.ToString("C").Substring(1)} {uom}";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return string.Empty;
            }
        }

        private string GetNumber(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "1" : value;
        }
        

        public decimal SalePrice => Product.SalePrice;
        public decimal CostPrice => Product.CostPrice;
        public decimal UnitDiscount => Product.UnitDiscount;

        public decimal FixPrice => Product.DeductionFixPrice; //.ToString("N2")

        public decimal Outright => Product.DeductionOutright; //.ToString("N2")

        public decimal Promo => Product.DeductionPromoDiscount; //.ToString("N2")

        public string DiscountDisplay =>
            $"{Product.Discount.ToString("N2")} - {CashDiscount.ToString("N2")}"; //.ToString("N2")
        public double Discount => Product.Discount; //.ToString("N2")

        public decimal CashDiscount => CalculateDiscount; //.ToString("N2")

        private decimal CalculateDiscount =>
            Product.DeductionFixPrice > 0
                ? 0
                : (decimal) Product.Discount;

        private decimal CalculatePromo => Product.DeductionPromoDiscount;

        private decimal CalculateOutright => Product.DeductionOutright;

        public decimal SubTotal
        {
            get
            {
                decimal subTotal = 0;
                subTotal = (Product.DeductionFixPrice > 0 ? Product.DeductionFixPrice : Product.SalePrice) * (decimal)CartQuantity;
                return subTotal;
            }
        }

        public decimal SubTotalCostPrice
        {
            get
            {
                decimal subTotal = 0;
                subTotal = (SalePrice - UnitDiscount)  * (decimal)CartQuantity;
                return subTotal;
            }
        }

        public string InvoiceNo { get; set; }
        public string Supplier { get; set; }
    }
}
