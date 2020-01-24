using System;
using System.Globalization;
using System.Windows.Media;

namespace Project.FC2J.UI
{
    public class StringStarIconConverter : BaseValueConverter<StringStarIconConverter>
    {

        public object TrueValue { get; set; }
        public object FalseValue { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var star = "StarOutline";
            var max = 10;
            // Setting default values
            // Parsing converter parameter
            if (parameter != null)
            {
                // Parameter format: star count 1, 2, 3, 4, 5
                var parameterString = parameter.ToString();
                if (!string.IsNullOrEmpty(parameterString))
                {
                    var x = 0;

                    if (int.TryParse(parameterString, out x))
                    {
                        // you know that the parsing attempt
                        // was successful
                        max = max * x;
                    }
                    
                }
            }
            // Creating Color Brush
            if (value != null && ((int)value) > 0)
            {
                var _value = (int) value;

                if (_value >= max )
                {
                    star = "Star";
                }
                else 
                {
                    if ((max - _value) > 1 && (max - _value) <= 5)
                    {
                        star = "StarHalf";
                    }
                }
            }
            return star;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
