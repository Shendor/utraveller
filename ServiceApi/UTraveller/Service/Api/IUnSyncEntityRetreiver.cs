using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IUnSyncEntityRetreiver<out T> where T : BaseModel
    {
        Task SyncEntitiesOfEvent(Event e);

        IEnumerable<T> GetUnSyncEntitiesOfEvent(Event e);

        Type GetEntityType();
    }
}
