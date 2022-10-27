using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation.Internal
{
    public class LocalAndRemoteEntityMergeService
    {
        public static IEnumerable<M> UnionData<P, M, E>(P parent, IEnumerable<E> entities, IEnumerable<M> remoteEntities,
            AddEntityLocallyDelegate<M, P> addEntityLocallyDelegate,
            DeleteEntityLocallyDelegate<E> deleteEntityLocallyDelegate,
            UpdateEntityLocallyDelegate<M> updateEntityLocallyDelegate,
            UpdateEntityLocallyDelegate<E> updateLocalEntityLocallyDelegate,
            GetEntitiesLocallyDelegate<M> getEntitiesLocallyDelegate)
            where M : BaseModel
            where E : IRemotableEntity<long>
        {
            var deletedItems = new HashSet<long>();
            var localEntitiesWithRemoteId = new Dictionary<long, E>();
            var remoteEntitiesQuantity = remoteEntities == null ? -1 : remoteEntities.Count();

            foreach (var entity in entities)
            {
                var foundRemoteEntity = remoteEntities != null ? remoteEntities.FirstOrDefault((re) =>
                {
                    return re.RemoteId == entity.RemoteId;
                }) : null;
                if (!entity.IsDeleted && remoteEntitiesQuantity >= 0 && entity.RemoteId > 0)
                {
                    if (foundRemoteEntity == null)
                    {
                        entity.IsSync = false;
                        entity.RemoteId = 0;
                        updateLocalEntityLocallyDelegate(entity);
                        // deleteEntityLocallyDelegate(entity);
                    }
                    else
                    {
                        localEntitiesWithRemoteId.Add(entity.RemoteId, entity);
                    }
                }
                else if (entity.RemoteId > 0)
                {
                    if (remoteEntitiesQuantity >= 0 && foundRemoteEntity == null)
                    {
                        deleteEntityLocallyDelegate(entity);
                    }
                    else
                    {
                        deletedItems.Add(entity.RemoteId);
                    }
                }
            }

            if (remoteEntities != null)
            {
                foreach (var remoteEntity in remoteEntities)
                {
                    if (!deletedItems.Contains(remoteEntity.RemoteId) && !localEntitiesWithRemoteId.ContainsKey(remoteEntity.RemoteId))
                    {
                        remoteEntity.IsSync = true;
                        addEntityLocallyDelegate(remoteEntity, parent);
                    }
                    else if (updateEntityLocallyDelegate != null && remoteEntity.ChangeDate != null
                        && localEntitiesWithRemoteId.ContainsKey(remoteEntity.RemoteId))
                    {
                        var localEntity = localEntitiesWithRemoteId[remoteEntity.RemoteId];
                        if (localEntity.ChangeDate != null && localEntity.IsSync &&
                            localEntity.ChangeDate.Value.CompareTo(remoteEntity.ChangeDate.Value) < 0)
                        {
                            remoteEntity.Id = localEntity.Id;
                            remoteEntity.IsSync = true;
                            updateEntityLocallyDelegate(remoteEntity);
                        }
                    }

                }
            }
            return getEntitiesLocallyDelegate();
        }

    }

    public delegate void UpdateEntityLocallyDelegate<M>(M model);

    public delegate void AddEntityLocallyDelegate<M, P>(M model, P parent);

    public delegate void DeleteEntityLocallyDelegate<E>(E entity);

    public delegate ICollection<M> GetEntitiesLocallyDelegate<M>() where M : BaseModel;
}
