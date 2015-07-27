using System;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.Converter
{
    public class MoneyConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal)
                return ((decimal) value).ToString("N");
            if (value is double)
                return ((double) value).ToString("N");
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string str = value as string;
            if (str != null)
                return str.Trim(',');
            return "0";
        }
    }
}
