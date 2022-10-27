using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IEventRepository : IBaseRepository<EventEntity, long>
    {
        int GetEventsQuantity(long userId);

        IEnumerable<EventEntity> GetEventsOfUser(long userId);

        void ChangeCurrentEvent(long eventId, bool isCurrent);

        EventEntity FindCurrentEvent(long userId);

        bool IsEventNameExist(string eventName, long userId);
    }
}