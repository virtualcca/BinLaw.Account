using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BinLaw.Account.FE.Common;
using BinLaw.Account.Model;
using BinLaw.Account.Model.Base;

namespace BinLaw.Account.Service.BusinessService
{
    /// <summary>
    /// 这是接口逻辑的基类
    /// </summary>
    public class BaseService
    {
        private ServiceClient _sc;

        public delegate void SessionTimeoutHandler(ServiceResponseMessage message, Exception exception);
        /// <summary>
        /// 会话超时事件
        /// </summary>
        public static event SessionTimeoutHandler OnSessionTimeouted;

        /// <summary>
        /// 服务连接客户端实例
        /// </summary>
        internal ServiceClient ServiceClientInstance
        {
            get
            {
                return _sc;
            }
        }

        public BaseService()
        {
            _sc = new ServiceClient();
            _sc.OnUpdateCacheCompleted += _sc_OnUpdateCacheCompleted;
        }

        /// <summary>
        /// 实现对缓存数据的更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _sc_OnUpdateCacheCompleted(object sender, SessionEventArgs e)
        {
            //SessionService.Instance.Update(e.SyncCache);
        }

        /// <summary>
        /// 获取服务返回数据
        /// </summary>
        /// <param name="serviceType">接口名称，可从文档获取</param>
        /// <param name="requestObject">传入参数对象</param>
        /// <param name="httpMethod">网络请求的操作谓词</param>
        /// <returns></returns>
        public async Task<ServiceResponse> GetServiceResponse(string serviceType, object requestObject, HttpMethod httpMethod)
        {
            var req = new BaseRequestItem
            {
                requestParameter = requestObject,
                srcChannel = Constants.APP_SRCCHANNEL,
                submitTimestamp = DateTime.Now.ToString("yyyyMMddHHmmss")
            };
            return await ServiceClientInstance.HttpRequest(serviceType, requestObject, httpMethod);
        }

        /// <summary>
        /// 获取服务返回数据
        /// </summary>
        /// <param name="serviceType">接口名称，可从文档获取</param>
        /// <param name="requestString">传入参数</param>
        /// <returns></returns>
        public async Task<ServiceResponse> GetServiceResponse(string serviceType, string requestString)
        {
            return await ServiceClientInstance.Request(serviceType, requestString);
        }

        /// <summary>
        /// 会话超时事件处理
        /// </summary>
        /// <param name="serviceResponseMessage"></param>
        protected void SessionTimeoutErrorHandler(ServiceResponseMessage serviceResponseMessage, Exception exception)
        {
            if (OnSessionTimeouted != null)
            {
                OnSessionTimeouted(serviceResponseMessage, exception);
                //CanvelPendingRequests();
            }
        }

        /// <summary>
        /// 取消ServiceClient的所有的请求
        /// </summary>
        public void CancelPendingRequests()
        {
            _sc.CancelPendingRequests();
        }
    }
}
