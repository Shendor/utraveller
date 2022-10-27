using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MessagePost.Messages
{
    public class MessagePostedMessage : ObjectMessage<string>
    {
        public MessagePostedMessage(string message)
            : base(message)
        {
        }
    }
}
