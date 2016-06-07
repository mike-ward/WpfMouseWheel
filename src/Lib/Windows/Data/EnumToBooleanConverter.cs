using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Logitech.Windows.Data
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return DependencyProperty.UnsetValue;

            var checkValue = value.ToString();
            var targetValue = parameter.ToString();
            return checkValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return DependencyProperty.UnsetValue;

            var useValue = (bool) value;
            var targetValue = parameter.ToString();
            return useValue ? Enum.Parse(targetType, targetValue) : DependencyProperty.UnsetValue;
        }
    }
}