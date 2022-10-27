using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.EventMap.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class PhotoPushpinDeletedMessage : ObjectMessage<TimeLineItemPushpinViewModel>
    {
        public PhotoPushpinDeletedMessage(TimeLineItemPushpinViewModel photoPushpin)
            : base(photoPushpin)
        {
        }
    }
}
