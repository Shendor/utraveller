using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class SkyDriveSessionChangedMessage : ObjectMessage<LiveConnectSession>
    {
        public SkyDriveSessionChangedMessage(LiveConnectSession session)
            : base(session)
        {
        }
    }
}
