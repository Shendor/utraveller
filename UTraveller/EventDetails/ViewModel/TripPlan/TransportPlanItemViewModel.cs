using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class FlightPlanItemViewModel : BasePlanItemViewModel<TransportPlanItem>
    {
        public FlightPlanItemViewModel(TransportPlanItem planItem)
            : base(planItem)
        {

        }


        public override string Description
        {
            get { return string.Format("To: {0}. \r {1}", basePlanItem.Destination, basePlanItem.Description); }
        }


        public override string Caption
        {
            get { return PlanItemTypeUtil.GetPlanItemTypeName(basePlanItem.Type); }
        }


        public override Uri Icon
        {
            get { return PlanItemTypeUtil.GetIcon(basePlanItem.Type); }
        }
    }
}
