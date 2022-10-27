using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.ImageChooser.Messages
{
    public class ExcludeMessagesFromListMessage : ObjectsCollectionMessage<long>
    {
        public ExcludeMessagesFromListMessage(ICollection<long> messagesId)
            : base(messagesId)
        {
        }
    }
}
