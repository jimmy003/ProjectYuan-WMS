using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Models
{
    public class ProductDisplayModel : BaseDisplayModel
    {
        public Int64 Id { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                CallPropertyChanged(nameof(Name));
            }
        }
        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set { _isEditable = value; CallPropertyChanged(nameof(IsEditable)); }
        }
        private bool _isFeeds;
        public bool IsFeeds
        {
            get { return _isFeeds; }
            set { _isFeeds = value; CallPropertyChanged(nameof(IsFeeds)); }
        }
        private bool _isConvertibleToBag;
        public bool IsConvertibleToBag
        {
            get { return _isConvertibleToBag; }
            set { _isConvertibleToBag = value; CallPropertyChanged(nameof(IsConvertibleToBag)); }
        }
        private decimal _kiloPerUnit;
        public decimal KiloPerUnit
        {
            get { return _kiloPerUnit; }
            set { _kiloPerUnit = value; CallPropertyChanged(nameof(KiloPerUnit)); }
        }

        private bool _isDelivered;
        public bool IsDelivered
        {
            get { return _isDelivered; }
            set { _isDelivered = value; CallPropertyChanged(nameof(IsDelivered)); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; CallPropertyChanged(nameof(Description)); }
        }

        private string _category;

        public string Category
        {
            get { return _category; }
            set { _category = value; CallPropertyChanged(nameof(Category)); }
        }

        private string _unitOfMeasure;
        public string UnitOfMeasure
        {
            get { return _unitOfMeasure; }
            set { _unitOfMeasure = value; CallPropertyChanged(nameof(UnitOfMeasure)); }
        }

        private decimal _salePrice;
        public decimal SalePrice
        {
            get { return _salePrice; }
            set { _salePrice = value; CallPropertyChanged(nameof(SalePrice)); }
        }

        private decimal _salePrice_CORON;
        public decimal SalePrice_CORON
        {
            get { return _salePrice_CORON; }
            set { _salePrice_CORON = value; CallPropertyChanged(nameof(SalePrice_CORON)); }
        }

        private decimal _salePrice_LUBANG;
        public decimal SalePrice_LUBANG
        {
            get { return _salePrice_LUBANG; }
            set { _salePrice_LUBANG = value; CallPropertyChanged(nameof(SalePrice_LUBANG)); }
        }

        private decimal _salePrice_SANILDEFONSO;
        public decimal SalePrice_SANILDEFONSO
        {
            get { return _salePrice_SANILDEFONSO; }
            set { _salePrice_SANILDEFONSO = value; CallPropertyChanged(nameof(SalePrice_SANILDEFONSO)); }
        }


        private decimal _costPrice;
        public decimal CostPrice
        {
            get { return _costPrice; }
            set {
                _costPrice = value;
                CallPropertyChanged(nameof(CostPrice));
            }
        }

        private decimal _unitDiscount;

        public decimal UnitDiscount
        {
            get { return _unitDiscount; }
            set
            {
                _unitDiscount = value; 
                CallPropertyChanged(nameof(UnitDiscount));
            }
        }


        private string _sfaUnitOfMeasure;
        public string SFAUnitOfMeasure
        {
            get { return _sfaUnitOfMeasure; }
            set { _sfaUnitOfMeasure = value; CallPropertyChanged(nameof(SFAUnitOfMeasure)); }
        }

        private string _sfaReferenceNo;
        public string SFAReferenceNo
        {
            get { return _sfaReferenceNo; }
            set { _sfaReferenceNo = value; CallPropertyChanged(nameof(SFAReferenceNo)); }
        }

        private double _stockQuantity;
        public double StockQuantity
        {
            get { return _stockQuantity; }
            set
            {
                _stockQuantity = value;
                CallPropertyChanged(nameof(StockQuantity));
            }
        }

        private double _stock_CORON;
        public double Stock_CORON
        {
            get { return _stock_CORON; }
            set
            {
                _stock_CORON = value;
                CallPropertyChanged(nameof(Stock_CORON));
            }
        }

        private double _stock_LUBANG;
        public double Stock_LUBANG
        {
            get { return _stock_LUBANG; }
            set
            {
                _stock_LUBANG = value;
                CallPropertyChanged(nameof(Stock_LUBANG));
            }
        }

        private double _stock_SANILDEFONSO;
        public double Stock_SANILDEFONSO
        {
            get { return _stock_SANILDEFONSO; }
            set
            {
                _stock_SANILDEFONSO = value;
                CallPropertyChanged(nameof(Stock_SANILDEFONSO));
            }
        }

        private bool _isTaxable;

        public bool IsTaxable
        {
            get { return _isTaxable; }
            set { _isTaxable = value; CallPropertyChanged(nameof(IsTaxable)); }
        }

        public bool Deleted { get; set; }
        public string ProductType { get; set; }
        public bool CanBeSold { get; set; }
        public bool CanBePurchased { get; set; }
        public string Barcode { get; set; }
        public string InternalCategory { get; set; }
        public string PurchaseUnitOfMeasure { get; set; }
        public string ControlPurchaseBills { get; set; }
        
        private decimal _deductionFixPrice;
        public decimal DeductionFixPrice
        {
            get { return _deductionFixPrice; }
            set { _deductionFixPrice = value; CallPropertyChanged(nameof(DeductionFixPrice)); }
        }

        private decimal _deductionOutright;
        public decimal DeductionOutright
        {
            get { return _deductionOutright; }
            set { _deductionOutright = value; CallPropertyChanged(nameof(DeductionOutright)); }
        }
        
        private double _discount;
        public double Discount
        {
            get { return _discount; }
            set { _discount = value; CallPropertyChanged(nameof(Discount)); }
        }

        private decimal _deductionCashDiscount;
        public decimal DeductionCashDiscount
        {
            get { return _deductionCashDiscount; }
            set { _deductionCashDiscount = value; CallPropertyChanged(nameof(DeductionCashDiscount)); }
        }

        private decimal _deductionPromoDiscount;
        public decimal DeductionPromoDiscount
        {
            get { return _deductionPromoDiscount; }
            set { _deductionPromoDiscount = value; CallPropertyChanged(nameof(DeductionPromoDiscount)); }
        }

        private string _image;
        public string Image
        {
            get { return _image; }
            set { _image = value; CallPropertyChanged(nameof(Image)); }
        }

        public DateTime UpdatedDate { get; set; }

    }
}
