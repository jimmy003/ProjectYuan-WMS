using System;
using System.Globalization;
using System.Windows;

namespace Project.FC2J.UI.ValueConverters
{
    public class BoolToStocksAction : BaseValueConverter<BoolToStocksAction>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            return (bValue) ? "Decrement Stocks" : "Increment Stocks";
        }


        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
