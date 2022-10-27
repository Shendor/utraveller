using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class EventChangedMessage
    {
        public EventChangedMessage(Event e)
        {
            Event = e;
        }

        public Event Event
        {
            get;
            set;
        }
    }
}
