using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface ITripPlanService
    {
        TripPlan GetTripPlan(Event e);

        void AddTripPlan(TripPlan tripPlan, Event e);

        void UpdateTripPlan(TripPlan tripPlan, Event e);

        void DeleteTripPlan(TripPlan tripPlan, Event e);

        void AddPlanItem(TripPlan tripPlan, BasePlanItem planItem, Event e);

        void UpdatePlanItem(TripPlan tripPlan, BasePlanItem oldPlanItem, BasePlanItem planItem, Event e);

        void DeletePlanItem(TripPlan tripPlan, BasePlanItem planItem, Event e);
    }
}
