using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApi.UTraveller.Service.Exceptions
{
    public class WebServiceTimeoutException: Exception
    {
        public WebServiceTimeoutException()
            : base("Server was not responded")
        {
        }
    }
}
