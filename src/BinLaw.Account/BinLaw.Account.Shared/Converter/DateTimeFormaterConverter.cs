using System;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.Converter
{
    public class DateTimeFormaterConverter : IValueConverter
    {
        static readonly DateTimeFormatter sdatefmt = new DateTimeFormatter("shortdate");
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (!(value is DateTime)) return null;
            var date = (DateTime) value;
            if (parameter == null)
            {
                
                return sdatefmt.Format(date);
            }
                
            return date.ToString(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
