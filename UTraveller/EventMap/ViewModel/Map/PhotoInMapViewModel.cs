using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.EventDetails.ViewModel;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel.Map
{
    public class PhotoInMapViewModel : BasePushpinItemInMapViewModel<IPushpinItem>
    {

        public PhotoInMapViewModel(Photo photo)
            : base(photo)
        {
        }


        protected override void ShowOnMap()
        {
            MessengerInstance.Send<FindPushpinItemInPushpinMessage>(new FindPushpinItemInPushpinMessage(DateItem));
        }


        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<DeletedEventPhotoMessage>(new DeletedEventPhotoMessage((Photo)DateItem));
        }


        protected override void DeleteFromMap()
        {
            MessengerInstance.Send<PhotoDeletedFromMapMessage>(new PhotoDeletedFromMapMessage((Photo)DateItem));
        }
    }
}
