using System;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Mobile.Service.Tables;
using Newtonsoft.Json;

namespace BinLaw.Account.MobileService.DataObjects
{
    public class BillModel : ITableData
    {

        [DataMember(Name = "Name")]
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "DateTime")]
        [JsonProperty(PropertyName = "DateTime")]
        public DateTime DateTime { get; set; }

        [DataMember(Name = "RecordTime")]
        [JsonProperty(PropertyName = "RecordTime")]
        public DateTime RecordTime { get; set; }

        [DataMember(Name = "UpdateTime")]
        [JsonProperty(PropertyName = "UpdateTime")]
        public DateTime? UpdateTime { get; set; }

        [DataMember(Name = "Longitude")]
        [JsonProperty(PropertyName = "Longitude")]
        public double? Longitude { get; set; }

        [DataMember(Name = "Latitude")]
        [JsonProperty(PropertyName = "Latitude")]
        public double? Latitude { get; set; }

        [DataMember(Name = "Money")]
        [JsonProperty(PropertyName = "Money")]
        public double Money { get; set; }

        [DataMember(Name = "Note")]
        [JsonProperty(PropertyName = "Note")]
        public string Notes { get; set; }

        [DataMember(Name = "Category")]
        [JsonProperty(PropertyName = "Category")]
        public int Category { get; set; }

        [DataMember(Name = "BillType")]
        [JsonProperty(PropertyName = "BillType")]
        public int BillType { get; set; }

        [DataMember(Name = "IsSync")]
        [JsonProperty(PropertyName = "IsSync")]
        public bool IsSync { get; set; }

        [DataMember(Name = "IsRegular")]
        [JsonProperty(PropertyName = "IsRegular")]
        public bool IsRegular { get; set; }

        public string UserId { get; set; }

        [DataMember(Name = "DeviceId")]
        [JsonProperty(PropertyName = "DeviceId")]
        public string DeviceId { get; set; }

        public DateTimeOffset? CreatedAt
        {
            get;
            set;
        }

        public bool Deleted
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public DateTimeOffset? UpdatedAt
        {
            get;
            set;
        }

        public byte[] Version
        {
            get;
            set;
        }
    }
}