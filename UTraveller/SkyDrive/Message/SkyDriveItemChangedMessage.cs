using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.SkyDrive.ViewModel;

namespace UTraveller.SkyDrive.Message
{
    public class SkyDriveItemChangedMessage : ObjectMessage<BaseSkyDriveItemViewModel>
    {
        public SkyDriveItemChangedMessage(BaseSkyDriveItemViewModel obj)
            : base(obj)
        {
        }
    }
}
