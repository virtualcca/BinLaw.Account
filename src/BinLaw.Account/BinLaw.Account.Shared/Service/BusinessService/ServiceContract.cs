using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Helper;

namespace BinLaw.Account.Service.BusinessService
{
    internal class ServiceContract
    {
        internal string Name { get; set; }
        internal string EndPointUrl { get; set; }
        internal string EndPointService { get; set; }
        internal string RequestHeader { get; set; }
        internal string RequestParamerter { get; set; }
        internal string ReponseHeader { get; set; }
        internal string DeserializeType { get; set; }
        internal bool DeserializeToList { get; set; }
        internal string MethodType { get; set; }  //Get or Post
        internal string RequestContentType { get; set; }  //application/json or FormUrlEncoded
        internal string ResponseMessageType { get; set; } //Standard or Extended

        internal string SampleMessageDirectory { get; set; }

        internal string SampleMessageFileType { get; set; }

        internal bool SyncCache { get; set; }
    }

    internal class ServiceContractList
    {
        private static IDictionary<string, ServiceContract> appContract;

        /// <summary>
        /// Load contract used for communicating with DataPower from XML
        /// </summary>
        private static async void LoadContract()
        {
            if (appContract == null)
                appContract = new Dictionary<string, ServiceContract>();

            //StreamResourceInfo contractXml = Application.GetResourceStream(new Uri(Constants.CONTRACT_FILE, UriKind.Relative));
            //XElement contractData = XElement.Load(contractXml.Stream);
            //var contracts = from item in contractData.Elements("Contract")
            //                select item;
            //foreach (XElement contract in contracts)

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(await ResHelper.ReadResourceAsync(Constants.CONTRACT_FILE));
            foreach (IXmlNode xmlNode in xmlDoc.DocumentElement.SelectNodes("Contract"))
            {
                ServiceContract _serviceContract = new ServiceContract();
                _serviceContract.Name = xmlNode.Attributes.GetNamedItem("Name").InnerText;
                _serviceContract.RequestHeader = xmlNode.Attributes.GetNamedItem("RequestHeader").InnerText;
                _serviceContract.ReponseHeader = xmlNode.Attributes.GetNamedItem("ReponseHeader").InnerText;
                _serviceContract.RequestParamerter = xmlNode.Attributes.GetNamedItem("RequestParameter").InnerText;
                _serviceContract.EndPointUrl = xmlNode.Attributes.GetNamedItem("EndPointURL").InnerText;
                _serviceContract.EndPointService = xmlNode.Attributes.GetNamedItem("EndPointService").InnerText;
                _serviceContract.DeserializeType = xmlNode.Attributes.GetNamedItem("DeserializeType").InnerText;
                _serviceContract.DeserializeToList =
                    bool.Parse(xmlNode.Attributes.GetNamedItem("DeserializeToList").InnerText);
                _serviceContract.MethodType = xmlNode.Attributes.GetNamedItem("MethodType").InnerText;
                _serviceContract.RequestContentType = xmlNode.Attributes.GetNamedItem("RequestContentType").InnerText;
                _serviceContract.ResponseMessageType = xmlNode.Attributes.GetNamedItem("ResponseMessageType").InnerText;
                _serviceContract.SampleMessageDirectory =
                    xmlNode.Attributes.GetNamedItem("SampleMessageDirectory").InnerText;
                _serviceContract.SampleMessageFileType =
                    xmlNode.Attributes.GetNamedItem("SampleMessageFileType").InnerText;
                _serviceContract.SyncCache = bool.Parse(xmlNode.Attributes.GetNamedItem("SyncCache").InnerText);

                if (!appContract.Keys.Contains(_serviceContract.Name))
                    appContract.Add(_serviceContract.Name, _serviceContract);
            }
        }

        internal static ServiceContract GetContract(string contractType)
        {
            if (appContract == null)
                LoadContract();

            return appContract[contractType];
        }

        internal static KeyValuePair<string, ServiceContract> GetContractByModelName(string modelFullName)
        {
            if (appContract == null)
                LoadContract();

            return appContract.FirstOrDefault(o => o.Value.DeserializeType == modelFullName);
        }
    }
}
