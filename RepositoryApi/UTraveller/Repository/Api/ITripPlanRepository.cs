using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface ITripPlanRepository : IBaseRepository<TripPlanEntity, long>, IDeleteFromEventRepository,
        IMarkDeleteRepository<TripPlanEntity>
    {
        TripPlanEntity GetTripPlanOfEvent(long eventId);

        TripPlanEntity GetUnSyncTripPlanOfEvent(long eventId);
    }
}
