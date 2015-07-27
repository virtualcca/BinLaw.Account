using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinLaw.Account.FE.Model.Common
{
    public class KeyValueData: ObservableObject
    {
        private string _key;
        private string _value;

        public string Key
        {
            get { return _key; }
            set { Set(()=>Key,ref _key,value); }
        }

        public string Value
        {
            get { return _value; }
            set { Set(()=>Value,ref _value,value); }
        }
    }

    public class KeyValueData<TKey,TValue> : ObservableObject
    {
        private TKey _key;
        private TValue _value;

        public TKey Key
        {
            get { return _key; }
            set { Set(() => Key, ref _key, value); }
        }

        public TValue Value
        {
            get { return _value; }
            set { Set(() => Value, ref _value, value); }
        }

        public KeyValueData()
        {
            
        }

        public KeyValueData(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}
