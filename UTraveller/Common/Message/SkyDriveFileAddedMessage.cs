using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Message
{
    public class SkyDriveFileAddedMessage : ObjectMessage<Object>
    {
        public SkyDriveFileAddedMessage(Object skyDriveFile)
            : base(skyDriveFile)
        {
        }
    }
}
