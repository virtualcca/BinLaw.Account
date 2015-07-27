using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace BinLaw.Account.FE.Foundation.Extension
{
    public static class ExtensionMethod
    {
        public static ObservableCollection<T> ToObserableCollection<T>(this IEnumerable<T> source)
        {
            return source == null ? null : new ObservableCollection<T>(source);
        }

        
    }
}
