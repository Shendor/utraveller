using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class FlightPlanItemViewModel : BasePlanItemViewModel<FlightPlanItem>
    {
        public FlightPlanItemViewModel(FlightPlanItem planItem)
            : base(planItem)
        {

        }


        public override string Description
        {
            get { return string.Format("To: {0}. \r {1}", basePlanItem.Destination, basePlanItem.Description); }
        }


        public override string Caption
        {
            get { return "Flight"; }
        }


        public override Uri Icon
        {
            get { return new Uri("/Assets/Icons/flight.png", UriKind.Relative); }
        }
    }
}
