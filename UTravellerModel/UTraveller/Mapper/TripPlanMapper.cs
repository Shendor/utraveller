using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class TripPlanMapper : IModelMapper<TripPlan, TripPlanEntity>
    {
        public TripPlan MapEntity(TripPlanEntity entity)
        {
            TripPlan tripPlan = null;
            if (entity != null)
            {
                tripPlan = new TripPlan();
                tripPlan.Id = entity.Id;
                tripPlan.IsDeleted = entity.IsDeleted;
                tripPlan.IsSync = entity.IsSync;
                tripPlan.RemoteId = entity.RemoteId;
                tripPlan.ChangeDate = entity.ChangeDate;
                tripPlan.PlanItems = JsonConvert.DeserializeObject<List<PlanItem>>(entity.PlanItems);
                tripPlan.FlightPlanItems = JsonConvert.DeserializeObject<List<TransportPlanItem>>(entity.FlightPlanItems);
                tripPlan.RentPlanItems = JsonConvert.DeserializeObject<List<RentPlanItem>>(entity.RentPlanItems);
            }
            return tripPlan;
        }


        public TripPlanEntity MapModel(TripPlan model)
        {
            TripPlanEntity tripPlanEntity = null;
            if (model != null)
            {
                tripPlanEntity = new TripPlanEntity();
                tripPlanEntity.Id = model.Id;
                tripPlanEntity.IsDeleted = model.IsDeleted;
                tripPlanEntity.IsSync = model.IsSync;
                tripPlanEntity.RemoteId = model.RemoteId;
                tripPlanEntity.ChangeDate = model.ChangeDate;
                tripPlanEntity.PlanItems = JsonConvert.SerializeObject(model.PlanItems);
                tripPlanEntity.FlightPlanItems = JsonConvert.SerializeObject(model.FlightPlanItems);
                tripPlanEntity.RentPlanItems = JsonConvert.SerializeObject(model.RentPlanItems);
            }
            return tripPlanEntity;
        }
    }
}
