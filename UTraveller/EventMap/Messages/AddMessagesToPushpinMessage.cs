using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.EventMap.ViewModel;

namespace UTraveller.EventMap.Messages
{
    public class AddMessagesToPushpinMessage : ObjectMessage<TimeLineItemPushpinViewModel>
    {
        public AddMessagesToPushpinMessage(TimeLineItemPushpinViewModel pushpin)
            : base(pushpin)
        {
        }
    }
}
