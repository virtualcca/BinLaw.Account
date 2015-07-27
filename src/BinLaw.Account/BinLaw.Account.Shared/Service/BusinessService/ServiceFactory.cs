using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BinLaw.Account.FE.Common;
using BinLaw.Account.Model;
using BinLaw.Account.Model.Base;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace BinLaw.Account.Service.BusinessService
{
    /// <summary>
    /// 服务调用统一入口
    /// </summary>
    public class ServiceFactory : BaseService
    {
       
        /// <summary>
        /// 获取数据集合
        /// </summary>
        /// <param name="requestObject">传入对象</param>
        /// <param name="errHandle">错误处理</param>
        /// <returns>数据集合</returns>
        public async Task<List<T>> GetModelCollection<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle)
        {
            string typeName = typeof(T).FullName;
            var _scType = ServiceContractList.GetContractByModelName(typeName).Key;
            try
            {
                var result = await GetServiceResponse(_scType, requestObject,HttpMethod.Get);
                if (result.Exception != null
                   || !result.Message.ErrorCode.Equals("0"))
                {
                    return null;
                }
                return (List<T>)result.Message.Body;
            }
            catch (Exception ex)
            {
                errHandle(ex, null);
                return null;
            }
        }


        #region 标准的CRUD操作
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="requestObject">请求对象</param>
        /// <param name="errHandle">错误处理</param>
        /// <returns>返回实体</returns>
        public async Task<T> Get<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle = null)
        {
            return await RequestModel<T>(requestObject, errHandle, HttpMethod.Get);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="requestObject">请求对象</param>
        /// <param name="errHandle">错误处理</param>
        /// <returns>返回实体</returns>
        public async Task<T> Post<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle = null)
        {
            return await RequestModel<T>(requestObject, errHandle, HttpMethod.Post);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="requestObject">请求对象</param>
        /// <param name="errHandle">错误处理</param>
        /// <returns>返回实体</returns>
        public async Task<T> Put<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle = null)
        {
            return await RequestModel<T>(requestObject, errHandle, HttpMethod.Put);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="requestObject">请求对象</param>
        /// <param name="errHandle">错误处理</param>
        /// <returns>返回实体</returns>
        public async Task<T> Delete<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle = null)
        {
            return await RequestModel<T>(requestObject, errHandle, HttpMethod.Delete);
        }

        private async Task<T> RequestModel<T>(object requestObject, Action<Exception, ServiceResponseMessage> errHandle,
            HttpMethod httpMethod)
        {
            string typeName = typeof(T).FullName;
            var _scType = ServiceContractList.GetContractByModelName(typeName).Key;

            if (errHandle == null)
                errHandle = ServiceClientErrorHandler.BasicServiceHandler();
            try
            {
                var result = await GetServiceResponse(_scType, requestObject, httpMethod);
                if (result.Exception != null
                   )
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(result.Message.Body.ToString());
            }
            catch (Exception ex)
            {
                errHandle(ex, null);
                return default(T);
            }
        } 
        #endregion

        /// <summary>
        /// 获取标准格式数据，返回ServiceResponse
        /// </summary>
        /// <param name="requestObject">请求对象</param>
        /// <returns>返回ServiceResponse</returns>
        public async Task<ServiceResponse> GetData<T>(object requestObject)
        {
            string typeName = typeof(T).FullName;
            var _scType = ServiceContractList.GetContractByModelName(typeName).Key;

            return await GetServiceResponse(_scType, requestObject,HttpMethod.Get);
        }

        /// <summary>
        /// 处理请求参数
        /// 判断是否从基类继承，从基类继承添加必要属性(EMP_SID、srcChannel和submitTimestamp)
        /// 不从继承基类，直接返回
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        private object RequestObjectHandler(object requestObject)
        {
            return RequestObjectHandler(requestObject, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        }

        /// <summary>
        /// 处理请求参数(对象),传入请求时间戳
        /// 判断是否从基类继承，从基类继承添加必要属性(EMP_SID、srcChannel和submitTimestamp)
        /// 不从继承基类，直接返回
        /// </summary>
        /// <param name="requestObject"></param>
        /// <param name="submitTimestamp"></param>
        /// <returns></returns>
        private object RequestObjectHandler(object requestObject, string submitTimestamp)
        {
            if (requestObject is BaseRequestItem)
            {
                var requestItem = requestObject as BaseRequestItem;
                requestItem.srcChannel = Constants.APP_SRCCHANNEL;
                requestItem.submitTimestamp = submitTimestamp;
            }
            return requestObject;
        }

        /// <summary>
        /// 处理请求参数（键值对）
        /// 判断是否有必要键值对(EMP_SID、srcChannel和submitTimestamp)，若没有添加，若有直接返回
        /// </summary>
        /// <param name="requestString"></param>
        /// <param name="submitTimestamp"></param>
        /// <returns></returns>
        private string RequestKeyValueHandler(string requestString, string submitTimestamp)
        {
            if (string.IsNullOrEmpty(requestString)) requestString = string.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(requestString);

            if (!requestString.Contains("srcChannel"))
            {
                sb.Append(string.Format("&srcChannel={0}", Constants.APP_SRCCHANNEL));
            }
            if (!requestString.Contains("submitTimestamp"))
            {
                sb.Append(string.Format("&submitTimestamp={0}", submitTimestamp));
            }

            return sb.ToString();
        }
    }
}
