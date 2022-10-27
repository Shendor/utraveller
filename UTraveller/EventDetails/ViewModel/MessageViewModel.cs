using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class MessageViewModel : BasePushpinItemViewModel<UTravellerModel.UTraveller.Model.Message>, IDescriptionedTimeLineItem
    {
        public MessageViewModel(UTravellerModel.UTraveller.Model.Message message) :base(message)
        {
        }

        public void UpdateDescription()
        {
            RaisePropertyChanged("DateItem.Text");
        }

        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<MessageDeletedMessage>(new MessageDeletedMessage(DateItem));
        }

        protected override void ShowOnMap()
        {
            throw new NotImplementedException();
        }
    }
}
