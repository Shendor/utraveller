using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Exceptions
{
    public class FacebookNotAuthException : Exception
    {
        public FacebookNotAuthException()
            : base("User was not authorized through Facebook")
        {
        }
    }
}
