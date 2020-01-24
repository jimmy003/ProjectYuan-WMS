using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.FC2J.UI.Models
{
    public class PricelistProducts : BaseDisplayModel
    {

        public Int64 Id { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; CallPropertyChanged(nameof(Name)); }
        }
        private string _internalCategory;
        public string InternalCategory
        {
            get { return _internalCategory; }
            set { _internalCategory = value; CallPropertyChanged(nameof(InternalCategory)); }
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

    }
}
