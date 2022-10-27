using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class EventRemoteModelMapper : IModelMapper<Event, EventRemoteModel>
    {
        public Event MapEntity(EventRemoteModel remoteEvent)
        {
            var e = new Event();
            e.RemoteId = remoteEvent.Id;
            e.Name = remoteEvent.Name;
            e.Image = remoteEvent.Image;
            e.Date = remoteEvent.StartDate;
            e.EndDate = remoteEvent.EndDate;
            e.ChangeDate = remoteEvent.ChangeDate;

            return e;
        }


        public EventRemoteModel MapModel(Event e)
        {
            var eventEntity = new EventRemoteModel();
            eventEntity.Id = e.RemoteId;
            eventEntity.Name = e.Name;
            eventEntity.Image = e.Image;
            eventEntity.StartDate = new DateTime(e.Date.Ticks, DateTimeKind.Utc);
            if (e.EndDate != null)
            {
                eventEntity.EndDate = new DateTime(e.EndDate.Value.Ticks, DateTimeKind.Utc);
            }
            eventEntity.ChangeDate = e.ChangeDate;

            return eventEntity;
        }
    }
}
