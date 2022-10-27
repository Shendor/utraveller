using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class TripPlanRemoteModel : BaseRemoteModel
    {
        public string PlanItemsJson
        {
            get;
            set;
        }


        public string FlightPlanItemsJson
        {
            get;
            set;
        }


        public string RentPlanItemsJson
        {
            get;
            set;
        }
    }
}
