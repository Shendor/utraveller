using LocalRepositoryImpl.UTraveller.LocalRepository;
using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class RoutePushpinRepository : BaseRepository<RoutePushpinEntity>, IRoutePushpinRepository
    {
        public RoutePushpinRepository(LocalDatabase database)
            : base(database)
        {
        }

        public override void Insert(RoutePushpinEntity entity)
        {
            InsertEntityInTable(database.RoutePushpins, entity);
        }

        public override void Delete(RoutePushpinEntity entity)
        {
            DeleteEntityFromTable(database.RoutePushpins, entity);
        }

        public IEnumerable<RoutePushpinEntity> GetPushpinsOfRoute(long routeId)
        {
            return from RoutePushpinEntity routePushpin in database.RoutePushpins
                   where routePushpin.RouteId.Equals(routeId)
                   select routePushpin;
        }

        public RoutePushpinEntity GetById(long id)
        {
            return GetById(database.RoutePushpins, id);
        }
    }
}
