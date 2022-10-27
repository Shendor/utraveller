using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.Messages
{
    public class MessagesChosenMessage : ObjectsCollectionMessage<Message>
    {
        public MessagesChosenMessage(ICollection<Message> messages)
            : base(messages)
        {
        }
    }
}
