using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BinLaw.Account.Model.Base
{
    public sealed class BaseRequestItem
    {
        /// <summary>
        /// 渠道
        /// </summary>
        [JsonProperty(PropertyName = "srcChannel")]
        public string srcChannel { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [JsonProperty(PropertyName = "submitTimestamp")]
        public string submitTimestamp { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [JsonProperty(PropertyName = "RequestParameter")]
        public object requestParameter { get; set; }

    }
}
