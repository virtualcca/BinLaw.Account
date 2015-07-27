using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BinLaw.Account.Model
{
    [DataContract]
    public class ServiceResponseMessage
    {
        [DataMember(Name = "ec")]
        public string ErrorCode { get; set; }

        [DataMember(Name = "em")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "cd")]
        public object Body { get; set; }
    }
}
