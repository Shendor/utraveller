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
    public class ViewPushpinPhotosMessage : ObjectMessage<TimeLineItemPushpinViewModel>
    {
        public ViewPushpinPhotosMessage(TimeLineItemPushpinViewModel photoPushpin)
            : base(photoPushpin)
        {
        }
    }
}
