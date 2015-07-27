using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BinLaw.Account.FE.Common;
using BinLaw.Account.FE.Foundation.Common;
using BinLaw.Account.FE.Foundation.Helper;
using BinLaw.Account.Model;
using BinLaw.Account.Model.Base;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace BinLaw.Account.Service.BusinessService
{
    internal class ServiceClient
    {
        private static HttpClient httpClient;
        internal delegate void SessionUpdateCacheCompleted(object sender, SessionEventArgs e);
        internal event SessionUpdateCacheCompleted OnUpdateCacheCompleted;

        internal ServiceClient()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient
                {
                    BaseAddress = new Uri(AppConfig.BACKEND_URL),
                    Timeout = new TimeSpan(0, 0, 0, 0, AppConfig.CONNECTION_TIMEOUT),
                };
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-ZUMO-APPLICATION", AppConfig.APPLICATION_KEY);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-ZUMO-MASTER ", AppConfig.MASTER_KEY);
            }
        }


        internal async Task<ServiceResponse> HttpRequest(string txType, object requestObject, HttpMethod httpMethod)
        {
            string url, uriPara = string.Empty;
            HttpContent body = null;
            ServiceContract txContract;

            txContract = ServiceContractList.GetContract(txType.ToString());

            if (!DeviceHelper.IsNetworkAvailable)
                return new ServiceResponse(null, new ServiceException(ServiceExceptionCode.Error_NoConnection));

            //Body
            if (requestObject != null)
            {
                if (httpMethod == HttpMethod.Get)
                {
                    Dictionary<string, string> props = (from x in requestObject.GetType().GetRuntimeProperties()
                                                        select x).ToDictionary(
                            GetPropDataMemberAttrName,
                            x =>
                                (x.GetValue(requestObject) == null ? string.Empty : x.GetValue(requestObject).ToString()));
                    body = new FormUrlEncodedContent(props);

                    if (string.IsNullOrEmpty(txContract.RequestParamerter))
                        uriPara = string.Format("?{0}", body.ReadAsStringAsync().Result);
                    else
                        uriPara = string.Format("&{0}", body.ReadAsStringAsync().Result);
                }
                else
                {
                    body = new StringContent(EnvelopeMessage(txContract, JsonHelper.Serialize(requestObject)));
                }
                body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            if (App.MobileService.CurrentUser != null && App.MobileService.CurrentUser.MobileServiceAuthenticationToken != null)
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-ZUMO-AUTH", App.MobileService.CurrentUser.MobileServiceAuthenticationToken);

            //URL
            url = httpMethod == HttpMethod.Get ? string.Format("{0}{1}{2}{3}", txContract.EndPointUrl, txContract.EndPointService, txContract.RequestParamerter, uriPara) : string.Format("{0}{1}{2}", txContract.EndPointUrl, txContract.EndPointService, txContract.RequestParamerter);
            Debug.WriteLine(url);

            HttpResponseMessage respMsg;
            try
            {
                if (httpMethod == HttpMethod.Post)
                {
                    respMsg = await httpClient.PostAsync(url, body);
                }
                else if (httpMethod == HttpMethod.Get)
                {
                    respMsg = await httpClient.GetAsync(url);
                }
                else if (httpMethod == HttpMethod.Put)
                {
                    respMsg = await httpClient.PutAsync(url, body);
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    respMsg = await httpClient.DeleteAsync(url);
                }
                else
                {
                    respMsg = await httpClient.GetAsync(url);
                }

                respMsg.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(null, ex);
            }

            ServiceResponse response = GetResponse(txContract, await respMsg.Content.ReadAsStringAsync());

            return response;
        }

        #region Request Post
        internal async Task<ServiceResponse> Request(string txType, object requestObject)
        {
            string url, uriPara = string.Empty;
            HttpContent body = null;
            ServiceContract txContract;

            txContract = ServiceContractList.GetContract(txType.ToString());

            if (!DeviceHelper.IsNetworkAvailable)
                return new ServiceResponse(null, new ServiceException(ServiceExceptionCode.Error_NoConnection));

            //Body
            if (requestObject != null)
            {
                if (txContract.MethodType.Equals("Post"))
                {
                    if (txContract.RequestContentType.Equals("Json"))
                    {
                        body = new StringContent(EnvelopeMessage(txContract, JsonHelper.Serialize(requestObject)));
                        body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    }
                    else
                    {
                        Dictionary<string, string> props = (from x in requestObject.GetType().GetRuntimeProperties()
                                                            select x).ToDictionary(
                                                               GetPropDataMemberAttrName,
                                                               x => (x.GetValue(requestObject) == null ? string.Empty : x.GetValue(requestObject).ToString()));
                        body = new FormUrlEncodedContent(props);
                    }
                }
                else
                {
                    Dictionary<string, string> props = (from x in requestObject.GetType().GetRuntimeProperties()
                                                        select x).ToDictionary(
                                                           GetPropDataMemberAttrName,
                                                           x => (x.GetValue(requestObject) == null ? string.Empty : x.GetValue(requestObject).ToString()));
                    body = new FormUrlEncodedContent(props);

                    if (string.IsNullOrEmpty(txContract.RequestParamerter))
                        uriPara = string.Format("?{0}", body.ReadAsStringAsync().Result);
                    else
                        uriPara = string.Format("&{0}", body.ReadAsStringAsync().Result);
                }
            }

            //URL
            url = string.Format("{0}{1}{2}{3}", txContract.EndPointUrl, txContract.EndPointService, txContract.RequestParamerter, uriPara);
            Debug.WriteLine(url);

            HttpResponseMessage respMsg;
            try
            {
                if (txContract.MethodType.Equals("Post"))
                {
                    respMsg = await httpClient.PostAsync(url, body);
                }
                else
                    respMsg = await httpClient.GetAsync(url);

                respMsg.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(null, ex);
            }

            ServiceResponse response = GetResponse(txContract, await respMsg.Content.ReadAsStringAsync());

            return response;
        }

        internal async Task<ServiceResponse> Request(string txType, string requestString)
        {
            string url, uriPara = string.Empty;
            HttpContent body = null;
            ServiceContract txContract;

            txContract = ServiceContractList.GetContract(txType.ToString());

            if (!DeviceHelper.IsNetworkAvailable)
                return new ServiceResponse(null, new ServiceException(ServiceExceptionCode.Error_NoConnection));

            //Body
            if (!String.IsNullOrEmpty(requestString))
            {
                if (txContract.MethodType.Equals("Post"))
                {
                    if (txContract.ResponseMessageType.Equals("Json"))
                        body = new StringContent(EnvelopeMessage(txContract, JsonHelper.Serialize(requestString)));
                    else
                    {
                        body = new StringContent(requestString);
                    }
                }
                else
                {
                    uriPara = string.Format("?{0}", requestString);
                }
            }

            //URL
            url = string.Format("{0}{1}{2}{3}", txContract.EndPointUrl, txContract.EndPointService, txContract.RequestParamerter, uriPara);

            HttpResponseMessage respMsg;


            try
            {
                if (txContract.MethodType.Equals("Post"))
                {
                    if (txContract.ResponseMessageType.Equals("Json"))
                        respMsg = await httpClient.PostAsync(url, body);
                    else
                        respMsg = await httpClient.PostAsync(url, body);
                }
                else
                    respMsg = await httpClient.GetAsync(url);

                respMsg.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(null, ex);
            }

            ServiceResponse response = GetResponse(txContract, await respMsg.Content.ReadAsStringAsync());

            return response;
        }
        #endregion

        #region Get DataMember attribute name of property
        internal static string GetPropDataMemberAttrName(PropertyInfo prop)
        {
            string attrName = string.Empty;

            var attr = prop.GetCustomAttributes(true).FirstOrDefault(p => p is DataMemberAttribute);
            if (attr != null)
                attrName = (attr as DataMemberAttribute).Name;

            return attrName.Equals(string.Empty) ? prop.Name : attrName;
        }
        #endregion

        #region Envelope Message
        private static string EnvelopeMessage(ServiceContract contractSpec, string body)
        {
            if (!string.IsNullOrEmpty(contractSpec.RequestHeader))
            {
                string defaultFormat = "{0}";
                string[] headers = contractSpec.RequestHeader.Split(',');

                for (int i = headers.Length - 1; i >= 0; i--)
                {
                    if (!string.IsNullOrEmpty(headers[i].Trim()))
                        defaultFormat = string.Format("{{{{\"{0}\":{1}}}}}", headers[i], defaultFormat);
                }

                return string.Format(defaultFormat, body);
            }
            else
                return body;
        }
        #endregion

        #region UnEnvelope Message
        /// <summary>
        /// Remove headers from JSON string
        /// </summary>
        /// <param name="jsonStr">JSON string</param>
        /// <param name="headers">header array, e.g. {"Body","checkDevice",...}</param>
        /// <returns></returns>
        internal static string UnEnvelopeMessage(ServiceContract txContract, string jsonStr)
        {
            if (!string.IsNullOrEmpty(txContract.ReponseHeader))
            {
                string[] headers = txContract.ReponseHeader.Split(',');

                Match match;

                string body = jsonStr;

                foreach (string header in headers)
                {
                    if (header.Trim().Length > 0)
                    {
                        // (?<=\s*{\s*"Body"\s*:).*(?=\}\s*\z)
                        match = Regex.Match(body, string.Format("(?<=\\s*{{\\s*\"{0}\"\\s*:).*(?=\\}}\\s*\\z)", header.Trim()));

                        if (match.Groups.Count == 1)
                        {
                            body = match.Groups[0].ToString();
                        }
                        else
                            throw new Exception("JSON string format is not correct.");
                    }
                }

                return body;
            }
            else
                return jsonStr;
        }
        #endregion

        #region Get return message to async caller

        private ServiceResponse GetResponse(ServiceContract txContract, string responseJson)
        {
            try
            {
                string strongClassName;
                if (txContract.DeserializeType.IndexOf(",") >= 0)
                    strongClassName = txContract.DeserializeType;
                else
                    strongClassName = txContract.DeserializeType + "," + Constants.MODEL_ASSEMBLY;

                Type bodyType = Type.GetType(strongClassName);
                //Type headerType = txContract.ResponseMessageType.Equals("Standard") ? typeof(ServiceResponseMessage) : typeof(ServiceResponseMessageEx);
                Type headerType = typeof(ServiceResponseMessage);

                dynamic msg;
                if (txContract.DeserializeToList)
                {
                    System.Type objTypeGeneric = typeof(List<>).MakeGenericType(new System.Type[] { bodyType });
                    msg = JsonHelper.DeserializeMessage(headerType, objTypeGeneric, UnEnvelopeMessage(txContract, responseJson));
                }
                else
                    msg = JsonHelper.DeserializeMessage(headerType, bodyType, UnEnvelopeMessage(txContract, responseJson));

                return new ServiceResponse(msg, null, false);
            }
            catch (Exception ex)
            {
                return new ServiceResponse(null, ex);
            }
        }


        #endregion

        #region CancelPendingRequests
        /// <summary>
        /// 取消当前HttpClient所有请求
        /// </summary>
        internal void CancelPendingRequests()
        {
            httpClient.CancelPendingRequests();
        }
        #endregion
    }

    public class SessionEventArgs : EventArgs
    {
        public bool SyncCache { get; set; }
    }
}
