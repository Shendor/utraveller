using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class RoutePushpinSelectionChangedMessage
    {
        public RoutePushpinSelectionChangedMessage(RoutePushpin pushpin)
        {
            Pushpin = pushpin;
        }


        public RoutePushpin Pushpin
        {
            get;
            private set;
        }
    }
}
