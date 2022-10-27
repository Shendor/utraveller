using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface IMessageRemoteService
    {
        Task<IEnumerable<Message>> GetMessagesOfEvent(Event e);

        Task<bool> AddMessageToEvent(Message message, Event e);

        Task<bool> UpdateMessage(Message message, Event e);

        Task<bool> DeleteMessage(Message message, Event e);
    }
}
