using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class DeleteMessageFromMapMessage : ObjectMessage<Message>
    {
        public DeleteMessageFromMapMessage(Message message)
            : base(message)
        {
        }
    }
}
