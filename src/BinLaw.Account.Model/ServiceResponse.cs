using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinLaw.Account.Model
{
    [System.Runtime.Serialization.DataContract]
    public class ServiceResponse
    {
        private ServiceResponseMessage _message;

        public ServiceResponseMessage Message
        {
            get { return _message; }
        }

        private Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
        }

        private bool operationCancelled;

        public bool OperationCancelled
        {
            get { return operationCancelled; }
        }

        public ServiceResponse(ServiceResponseMessage respMessage, Exception ex)
        {
            _message = respMessage;
            _exception = ex;
            operationCancelled = false;
        }

        public ServiceResponse(ServiceResponseMessage respMessage, Exception ex, bool opCancelled)
        {
            _message = respMessage;
            _exception = ex;
            operationCancelled = opCancelled;
        }
    }
}
