using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel.Map
{
    public class MessageInMapViewModel : BasePushpinItemInMapViewModel<IPushpinItem>
    {

        public MessageInMapViewModel(Message message)
            : base(message)
        {
        }


        protected override void ShowOnMap()
        {
            MessengerInstance.Send<FindPushpinItemInPushpinMessage>(new FindPushpinItemInPushpinMessage(DateItem));
        }


        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<MessageDeletedMessage>(new MessageDeletedMessage((Message)DateItem));
        }


        protected override void DeleteFromMap()
        {
            MessengerInstance.Send<DeleteMessageFromMapMessage>(new DeleteMessageFromMapMessage((Message)DateItem));
        }
    }
}
