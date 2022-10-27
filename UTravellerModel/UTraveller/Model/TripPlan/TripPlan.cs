using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class TripPlan : BaseModel
    {
        public TripPlan()
        {
            PlanItems = new List<PlanItem>();
            FlightPlanItems = new List<TransportPlanItem>();
            RentPlanItems = new List<RentPlanItem>();
        }


        public ICollection<PlanItem> PlanItems
        {
            get;
            set;
        }


        public ICollection<TransportPlanItem> FlightPlanItems
        {
            get;
            set;
        }


        public ICollection<RentPlanItem> RentPlanItems
        {
            get;
            set;
        }
    }
}
