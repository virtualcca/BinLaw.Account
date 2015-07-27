using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace BinLaw.Account.Converter
{
    public class MoneyColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is int)) return null;
            return (int)value == 1
                ? new SolidColorBrush(Color.FromArgb(255, 196, 1, 16))
                : new SolidColorBrush(Color.FromArgb(255, 74, 141, 56));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
