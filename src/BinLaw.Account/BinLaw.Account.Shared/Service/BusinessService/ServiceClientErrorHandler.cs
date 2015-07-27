using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinLaw.Account.Model;

namespace BinLaw.Account.Service.BusinessService
{
    public static class ServiceClientErrorHandler
    {
        public static Action<Exception, ServiceResponseMessage> BasicServiceHandler(string title = "错误")
        {
            return async (e, c) =>
            {
                if (e != null)
                {
                    //await MessageDialogHelper.ShowAsyncQueue(e, title);
                }
                if (c != null && c.ErrorCode != "0")
                {
                    //await MessageDialogHelper.ShowAsyncQueue(c, title);
                }
            };
        }
    }
}
