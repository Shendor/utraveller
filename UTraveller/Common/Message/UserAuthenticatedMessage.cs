using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class UserAuthenticatedMessage : ObjectMessage<bool>
    {
        public UserAuthenticatedMessage(bool isAuthenticated)
            : base(isAuthenticated)
        {
        }
    }
}
