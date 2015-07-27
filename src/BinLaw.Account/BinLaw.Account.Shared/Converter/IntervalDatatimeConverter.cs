using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace BinLaw.Account.Converter
{
    public class IntervalDatatimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (value is DateTime)
            {
                var time = (DateTime)value;
                var timeoff = DateTime.Now - time;
                if (timeoff.TotalSeconds < 59)
                    return string.Format("Before {0} Seconds", timeoff.TotalSeconds.ToString("####"));
                if (timeoff.TotalMinutes < 59)
                    return string.Format("Before {0} Minutes", timeoff.TotalMinutes.ToString("####"));
                if (timeoff.TotalHours < 23)
                    return string.Format("Before {0} Hours", timeoff.TotalHours.ToString("####"));
                return time.ToString("d");
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
