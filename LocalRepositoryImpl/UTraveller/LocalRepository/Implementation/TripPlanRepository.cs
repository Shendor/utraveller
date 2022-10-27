using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class TripPlanRepository: BaseRemotableEntityRepository<TripPlanEntity>, ITripPlanRepository
    {
        public TripPlanRepository(LocalDatabase database)
            : base(database)
        {
        }


        public TripPlanEntity GetTripPlanOfEvent(long eventId)
        {
            return (from tripPlan in database.TripPlans
                   where tripPlan.EventId == eventId && !tripPlan.IsDeleted
                   select tripPlan).FirstOrDefault();
        }


        public TripPlanEntity GetById(long id)
        {
            return GetById(database.TripPlans, id);
        }


        public override void Insert(TripPlanEntity entity)
        {
            InsertEntityInTable(database.TripPlans, entity);
        }


        public override void Delete(TripPlanEntity entity)
        {
            DeleteEntityFromTable(database.TripPlans, entity);
        }


        public TripPlanEntity GetUnSyncTripPlanOfEvent(long eventId)
        {
            return (from TripPlanEntity tripPlan in database.TripPlans
                   where tripPlan.EventId == eventId && (tripPlan.IsSync == false || tripPlan.IsDeleted == true)
                   select tripPlan).FirstOrDefault();
        }


        public void DeleteFromEvent(EventEntity eventEntity)
        {
            database.TripPlans.DeleteAllOnSubmit(from TripPlanEntity tripPlan in database.TripPlans
                                              where tripPlan.EventId == eventEntity.Id
                                              select tripPlan);
        }


        public override IEnumerable<TripPlanEntity> GetAllIncludedMarkedAsDeleted(long eventId)
        {
            return from TripPlanEntity tripPlan in database.TripPlans
                   where tripPlan.EventId == eventId
                   select tripPlan;
        }
    }
}
