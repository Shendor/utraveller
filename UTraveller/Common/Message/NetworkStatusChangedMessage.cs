using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class NetworkStatusChangedMessage : ObjectMessage<bool>
    {
        public NetworkStatusChangedMessage(bool isConnected)
            : base(isConnected)
        {
        }
    }
}
