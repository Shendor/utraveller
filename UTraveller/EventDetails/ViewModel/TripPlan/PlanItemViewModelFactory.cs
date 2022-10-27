using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class PlanItemViewModelFactory
    {
        public static IList<IPlanItemViewModel> CreatePlanItemViewModel(BasePlanItem basePlanItem)
        {
            var planItems = new List<IPlanItemViewModel>();
            if (basePlanItem is PlanItem)
            {
                planItems.Add(new PlanItemViewModel((PlanItem)basePlanItem));
            }
            else if (basePlanItem is RentPlanItem)
            {
                planItems.Add(new RentPlanItemViewModel((RentPlanItem)basePlanItem));
                if (((RentPlanItem)basePlanItem).CheckOutDate != null)
                {
                    planItems.Add(new RentPlanItemViewModel((RentPlanItem)basePlanItem, true));
                }
            }
            else if (basePlanItem is TransportPlanItem)
            {
                planItems.Add(new FlightPlanItemViewModel((TransportPlanItem)basePlanItem));
            }
            return planItems;
        }
    }
}
