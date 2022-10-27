using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.ImageChooser.Messages;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.ViewModel
{
    public class MessagesChooserViewModel : BaseViewModel
    {
        private ICollection<long> excludedMessagesId;
        private IMessageService messageService;
        private Event currentEvent;

        public MessagesChooserViewModel(IMessageService messageService)
        {
            this.messageService = messageService;

            ChooseMessagesCommand = new ActionCommand(ChooseMessages);

            MessageList = new ObservableCollection<MessageViewModel>();
            MessengerInstance.Register<ExcludeMessagesFromListMessage>(this, OnExcludeMessagesFromList);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
        }


        public ICommand ChooseMessagesCommand
        {
            get;
            private set;
        }


        public override void Cleanup()
        {
            MessageList.Clear();
            excludedMessagesId = null;
        }


        public void Initialize()
        {
            var messages = messageService.GetMessagesOfEvent(currentEvent);
            foreach (var message in messages)
            {
                if (excludedMessagesId == null || !excludedMessagesId.Contains(message.Id))
                {
                    MessageList.Add(new MessageViewModel(message));
                }
            }
        }


        public ICollection<MessageViewModel> MessageList
        {
            get;
            private set;
        }


        private void ChooseMessages()
        {
            var messages = new List<Message>();
            foreach (var messageViewModel in MessageList)
            {
                if (messageViewModel.IsChecked)
                {
                    messages.Add(messageViewModel.Message);
                }
            }
            MessengerInstance.Send<MessagesChosenMessage>(new MessagesChosenMessage(messages));
        }


        private void OnExcludeMessagesFromList(ExcludeMessagesFromListMessage message)
        {
            excludedMessagesId = message.Objects;
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }

    }

    public class MessageViewModel : BaseViewModel
    {
        public MessageViewModel(Message message)
        {
            Message = message;
        }


        public Message Message
        {
            get;
            private set;
        }


        public bool IsChecked
        {
            get;
            set;
        }
    }
}
