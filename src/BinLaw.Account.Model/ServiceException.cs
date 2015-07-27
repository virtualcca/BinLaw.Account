using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinLaw.Account.Model
{
    public class ServiceException : Exception
    {
        public ServiceExceptionCode ErrorCode { get; set; }

        public ServiceException()
        {
        }

        public ServiceException(ServiceExceptionCode errorCode)
            : base(errorCode.ToString())
        {
            ErrorCode = errorCode;
        }

        public ServiceException(ServiceExceptionCode errorCode, Exception innerException)
            : base(errorCode.ToString(), innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public enum ServiceExceptionCode
    {
        /// <summary>
        /// 网络未连接
        /// </summary>
        Error_NoConnection,
    }
}
