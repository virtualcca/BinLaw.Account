using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using BinLaw.Account.Helper;

namespace BinLaw.Account.Converter
{
    public class BillCategoryIconPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return new BitmapImage(new Uri(@"ms-appx:///Assets/Account/Undefined.png", UriKind.Absolute));
            if (value is int)
            {
                var categoryId = (int)value;
                var category = BillHelper.CategoryConverter(categoryId);
                var uri = new Uri(@"ms-appx:///Assets/Account/" + category.Icon, UriKind.Absolute);
                

                BitmapImage bmp = new BitmapImage(uri);
                return bmp;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BillCategoryIconBackgroudColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if(value is int)
            {
                var category = BillHelper.CateGoryItems.Result.FirstOrDefault(a => a.CategoryId == (int) value);
                return category.BackGroundBrush;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BillCategoryIdToNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is int)
            {
                var category = (int) value;
                return BillHelper.CategoryNameConverter(category);
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BillTypeToNameConverter : IValueConverter
    {
        static readonly ResourceLoader loader = new ResourceLoader();
        
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is int)
            {
                var type = (int) value;
                return type == 1 ? loader.GetString("Expanse") :
                loader.GetString("Income");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
