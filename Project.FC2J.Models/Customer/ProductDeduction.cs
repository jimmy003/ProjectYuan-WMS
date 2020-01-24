using System;

namespace Project.FC2J.Models.Customer
{
    public class ProductDeduction
    {
        public Int64 Id { get; set; }

        public decimal DeductionFixPrice { get; set; }

        public decimal DeductionOutright { get; set; }

        public double Discount { get; set; }

        public decimal DeductionCashDiscount { get; set; }

        public decimal DeductionPromoDiscount { get; set; }

    }
}
