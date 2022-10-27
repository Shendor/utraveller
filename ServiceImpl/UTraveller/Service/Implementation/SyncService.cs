using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class SyncService : ISyncService
    {
        private IDictionary<Type, IUnSyncEntityRetreiver<BaseModel>> unSyncRetreivers;

        public SyncService()
        {
            unSyncRetreivers = new Dictionary<Type, IUnSyncEntityRetreiver<BaseModel>>();
        }

        public void AddUnSyncEntityRetreiver<T>(IUnSyncEntityRetreiver<T> unSyncRetreiver) where T : BaseModel
        {
            unSyncRetreivers.Add(unSyncRetreiver.GetEntityType(), unSyncRetreiver);
        }


        public ICollection<BaseModel> GetUnSyncEntitiesForEvent(Event e)
        {
            var unSyncEntities = new List<BaseModel>();

            foreach (var retreiver in unSyncRetreivers.Values)
            {
                unSyncEntities.AddRange(retreiver.GetUnSyncEntitiesOfEvent(e));
            }

            return unSyncEntities;
        }


        public async Task SyncEntitiesForEvent(Type entityType, Event e)
        {
            if (unSyncRetreivers.ContainsKey(entityType))
            {
                await unSyncRetreivers[entityType].SyncEntitiesOfEvent(e);
            }
        }

    }
}
