using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class TripPlanRemoteModelMapper : IModelMapper<TripPlan, TripPlanRemoteModel>
    {
        public TripPlan MapEntity(TripPlanRemoteModel entity)
        {
            var tripPlan = new TripPlan();
            tripPlan.RemoteId = entity.Id;
            tripPlan.ChangeDate = entity.ChangeDate;
            tripPlan.PlanItems = JsonConvert.DeserializeObject<IList<PlanItem>>(entity.PlanItemsJson);
            tripPlan.FlightPlanItems = JsonConvert.DeserializeObject<IList<TransportPlanItem>>(entity.FlightPlanItemsJson);
            tripPlan.RentPlanItems = JsonConvert.DeserializeObject<IList<RentPlanItem>>(entity.RentPlanItemsJson);

            return tripPlan;
        }

        public TripPlanRemoteModel MapModel(TripPlan model)
        {
            var tripPlan = new TripPlanRemoteModel();
            tripPlan.Id = model.RemoteId;
            tripPlan.ChangeDate = model.ChangeDate;
            tripPlan.PlanItemsJson = JsonConvert.SerializeObject(model.PlanItems);
            tripPlan.FlightPlanItemsJson = JsonConvert.SerializeObject(model.FlightPlanItems);
            tripPlan.RentPlanItemsJson = JsonConvert.SerializeObject(model.RentPlanItems);

            return tripPlan;
        }
    }
}
