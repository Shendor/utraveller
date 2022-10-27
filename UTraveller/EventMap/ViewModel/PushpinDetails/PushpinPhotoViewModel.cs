using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class PushpinPhotoViewModel : BaseTimeLineItemViewModel<Photo>
    {

        public PushpinPhotoViewModel(Photo photo)
            : base(photo)
        {
        }

        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<DeletedPushpinPhotoMessage>(new DeletedPushpinPhotoMessage(DateItem));
        }
    }
}
