using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api.Remote
{
    public class WebServiceException : Exception
    {
        public WebServiceException(string message)
            : base(message)
        {
        }
    }
}
