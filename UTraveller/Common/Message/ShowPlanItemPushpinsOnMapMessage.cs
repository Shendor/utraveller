using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class ShowPlanItemPushpinsOnMapMessage
    {
        public ShowPlanItemPushpinsOnMapMessage(IList<BasePlanItem> planItems)
        {
            this.PlanItems = planItems;
        }


        public IList<BasePlanItem> PlanItems
        {
            get;
            private set;
        }
    }
}
