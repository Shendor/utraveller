using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class ShowPushpinDetailsMessage
    {
        public ShowPushpinDetailsMessage(PhotoPushpin photoPushpin)
        {
            PhotoPushpin = photoPushpin;
        }

        public PhotoPushpin PhotoPushpin { get; private set; }
    }
}
