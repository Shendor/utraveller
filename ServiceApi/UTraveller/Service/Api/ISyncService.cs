using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface ISyncService
    {
        Task SyncEntitiesForEvent(Type entityType, Event e);

        ICollection<BaseModel> GetUnSyncEntitiesForEvent(Event e);
    }
}
