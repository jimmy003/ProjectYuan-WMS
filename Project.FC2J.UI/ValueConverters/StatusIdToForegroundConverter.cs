using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Project.FC2J.UI
{
    public class StatusIdToForegroundConverter : BaseValueConverter<StatusIdToForegroundConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var color = new SolidColorBrush(Colors.Black); 

            switch (System.Convert.ToInt32(value))
            {
                case 2:
                    color = new SolidColorBrush(Colors.Blue);
                    break;
                case 3:
                    color = new SolidColorBrush(Colors.Red);
                    break;
            }

            return color;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
