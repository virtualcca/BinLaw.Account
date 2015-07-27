using System;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.Converter
{
    public class DateTimeToOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var date = (DateTime)value;
                return new DateTimeOffset(date);
            }
            catch 
            {
                return DateTimeOffset.MinValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var dto = (DateTimeOffset)value;
                return dto.DateTime;
            }
            catch 
            {
                return DateTime.MinValue;
            }
        }
    }
}
