using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.Social.Messages
{
    public class FacebookSignedInMessage : ObjectMessage<bool>
    {
        public FacebookSignedInMessage(bool isSignedIn)
            : base(isSignedIn)
        {
        }
    }
}
