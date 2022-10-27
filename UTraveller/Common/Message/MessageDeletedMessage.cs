using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class MessageDeletedMessage : ObjectMessage<UTravellerModel.UTraveller.Model.Message>
    {
        public MessageDeletedMessage(UTravellerModel.UTraveller.Model.Message message)
            : base(message)
        {
        }
    }
}
