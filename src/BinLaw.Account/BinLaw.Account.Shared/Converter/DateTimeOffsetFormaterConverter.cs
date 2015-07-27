using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.Converter
{
    public class DateTimeOffsetFormaterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (!(value is DateTimeOffset)) return null;
            DateTimeOffset date = (DateTimeOffset)value;
            if (parameter == null || !(parameter is string)) return date.ToString();
            return date.ToString(parameter as string, CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
