using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IEventService
    {
        int GetEventsQuantity(User user);

        IEnumerable<Event> GetEvents(User user);

        bool IsEventNameExist(string eventName, User user);

        void AddEvent(Event e, User user);

        bool DeleteEvent(Event e);

        void UpdateEvent(Event e);

        void ChangeCurrentEvent(Event e);

        Event GetCurrentEvent(User user);
    }
}