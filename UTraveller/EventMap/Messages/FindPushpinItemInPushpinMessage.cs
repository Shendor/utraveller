using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.EventMap.ViewModel.Map;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class FindPushpinItemInPushpinMessage : ObjectMessage<IPushpinItem>
    {
        public FindPushpinItemInPushpinMessage(IPushpinItem photo)
            : base(photo)
        {
        }
    }
}
