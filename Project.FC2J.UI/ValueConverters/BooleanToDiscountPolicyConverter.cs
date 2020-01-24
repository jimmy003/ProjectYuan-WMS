using System;
using System.Globalization;

namespace Project.FC2J.UI
{
    public class BooleanToDiscountPolicyConverter : BaseValueConverter<BooleanToDiscountPolicyConverter>
    {

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var discountPolicy = "Discount included in the price";
            
            if (value != null && ((bool)value) )
            {
                discountPolicy = "Show public price &amp; discount to the customer";
            }
            return discountPolicy;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
