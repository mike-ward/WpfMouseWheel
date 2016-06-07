using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Logitech.Windows.Data
{
    public class BooleanToWaitCursorConverter : IValueConverter
    {
        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value ? Cursors.Wait : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}