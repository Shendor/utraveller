using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.EventMap.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class PushpinFoundMessage : ObjectMessage<TimeLineItemPushpinViewModel>
    {
        public PushpinFoundMessage(TimeLineItemPushpinViewModel photoPushpin)
            : base(photoPushpin)
        {
        }
    }
}
