using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class EventRepository : BaseRepository<EventEntity>, IEventRepository
    {
        public EventRepository(LocalDatabase database) : base(database) { }

        public IEnumerable<EventEntity> GetEventsOfUser(long userId)
        {
            return from EventEntity e in database.Events
                   where e.UserId == userId && !e.IsDeleted
                   orderby e.StartDate descending
                   select e;
        }

        public override void Insert(EventEntity entity)
        {
            InsertEntityInTable(database.Events, entity);
        }

        public override void Delete(EventEntity entity)
        {
            DeleteEntityFromTable(database.Events, entity);
        }

        public EventEntity GetById(long id)
        {
            return GetById(database.Events, id);
        }


        public void ChangeCurrentEvent(long eventId, bool isCurrent)
        {
            var eventEntities = from e in database.Events
                                where e.IsCurrent == true
                                select e;
            foreach (var eventEntity in eventEntities)
            {
                eventEntity.IsCurrent = false;
                Update(eventEntity);
            }
            var currentEvent = GetById(eventId);
            if (currentEvent != null)
            {
                currentEvent.IsCurrent = isCurrent;
                Update(currentEvent);
            }
        }


        public EventEntity FindCurrentEvent(long userId)
        {
            return (from e in database.Events
                    where e.IsCurrent == true && e.UserId == userId && !e.IsDeleted
                    select e).FirstOrDefault();
        }

        public int GetEventsQuantity(long userId)
        {
            return (from EventEntity e in database.Events
                    where e.UserId == userId && !e.IsDeleted
                    select e).Count();
        }

        public bool IsEventNameExist(string eventName, long userId)
        {
            return (from e in database.Events
                    where e.Name.Equals(eventName) && e.UserId == userId && !e.IsDeleted
                    select e).Any();
        }
    }
}
