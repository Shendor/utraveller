using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IRouteRepository : IBaseRepository<RouteEntity, long>, IDeleteFromEventRepository,
        IMarkDeleteRepository<RouteEntity>
    {
        IEnumerable<RouteEntity> GetRoutesOfEvent(long eventId);

        IEnumerable<RouteEntity> GetUnSyncRoutesOfEvent(long eventId);

        int GetRoutesQuantity(long eventId);
    }
}
