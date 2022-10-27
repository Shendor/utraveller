using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class EventPhotosChosenMessage : ChoosedPhotosMessage
    {
        public EventPhotosChosenMessage(ICollection<Photo> photos)
            :base(photos)
        {
        }
    }
}
