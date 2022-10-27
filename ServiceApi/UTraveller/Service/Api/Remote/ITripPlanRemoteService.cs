using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface ITripPlanRemoteService
    {
        Task<TripPlan> GetTripPlanOfEvent(Event e);

        Task<bool> AddTripPlan(TripPlan tripPlan, Event e);

        Task<bool> UpdateTripPlan(TripPlan tripPlan, Event e);

        Task<bool> DeleteTripPlan(Event e);
    }
}
