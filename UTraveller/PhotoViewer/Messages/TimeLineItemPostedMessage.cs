using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.PhotoViewer.Messages
{
    public class TimeLineItemPostedMessage : ObjectMessage<bool>
    {
        public TimeLineItemPostedMessage(bool isPosted)
            : base(isPosted)
        {
        }
    }
}
