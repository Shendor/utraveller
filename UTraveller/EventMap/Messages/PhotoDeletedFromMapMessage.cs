using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.ViewModel.Map;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class PhotoDeletedFromMapMessage : ObjectMessage<Photo>
    {
        public PhotoDeletedFromMapMessage(Photo photo)
            : base(photo)
        {
        }
    }
}
