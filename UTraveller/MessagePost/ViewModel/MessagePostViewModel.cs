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
using UTraveller.MessagePost.Messages;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MessagePost.ViewModel
{
    public class MessagePostViewModel : BaseViewModel
    {
        private Event currentEvent;

        public MessagePostViewModel()
        {
            PostMessageCommand = new ActionCommand(PostMessage);

            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
            MessengerInstance.Register<MessageChangedMessage>(this, OnMessageChanged);
        }


        public ICommand PostMessageCommand
        {
            get;
            private set;
        }


        public object Token
        {
            get;
            set;
        }


        public string Message
        {
            get;
            set;
        }


        private void PostMessage()
        {
            if (currentEvent != null)
            {
                MessengerInstance.Send<MessagePostedMessage>(new MessagePostedMessage(Message), Token);
                Message = null;
            }
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }


        private void OnMessageChanged(MessageChangedMessage message)
        {
            Message = message.Object;
            RaisePropertyChanged("Message");
        }

    }
}
