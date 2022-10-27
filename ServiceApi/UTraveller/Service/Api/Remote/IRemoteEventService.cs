using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface IEventRemoteService
    {
        Task<Event> GetEvent(User user, long id);

        Task<IEnumerable<Event>> GetEvents(User user);

        Task<bool> AddEvent(Event e, User user);

        Task<bool> DeleteEvent(Event e);

        Task<bool> UpdateEvent(Event e);
    }
}
