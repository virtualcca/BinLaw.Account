using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace BinLaw.Account.Model
{
    public class BillModel
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "DateTime")]
        public DateTime DateTime { get; set; }

        [JsonProperty(PropertyName = "RecordTime")]
        public DateTime RecordTime { get; set; }

        [JsonProperty(PropertyName = "UpdateTime")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty(PropertyName = "Longitude")]
        public double? Longitude { get; set; }

        [JsonProperty(PropertyName = "Latitude")]
        public double? Latitude { get; set; }

        [JsonProperty(PropertyName = "Money")]
        public double Money { get; set; }

        [JsonProperty(PropertyName = "Note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "Category")]
        public int Category { get; set; }

        [JsonProperty(PropertyName = "BillType")]
        public int BillType { get; set; }

        [JsonProperty(PropertyName = "IsSync")]
        public bool IsSync { get; set; }

        [JsonProperty(PropertyName = "IsRegular")]
        public bool IsRegular { get; set; }

        [JsonProperty(PropertyName = "DeviceId")]
        public string DeviceId { get; set; }

        [Version]
        public string Version { get; set; }


        public BillModel() { }
    }
}
