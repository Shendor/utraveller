using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class UserRegisteredMessage: ObjectMessage<bool>
    {
        public UserRegisteredMessage(bool isRegistered)
            : base(isRegistered)
        {
        }
    }
}
