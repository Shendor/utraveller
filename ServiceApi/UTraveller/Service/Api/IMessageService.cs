using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IMessageService
    {
        IEnumerable<Message> GetMessagesOfEvent(Event e);

        Task AddMessageToEvent(Message message, Event e);

        void UpdateMessage(Message message, Event e);

        void UpdateMessages(List<Message> messages, Event e);

        void DeleteMessage(Message message, Event e);
    }
}
