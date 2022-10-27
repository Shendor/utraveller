using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.Messages
{
    public class PlanItemSavedMessage
    {
        public PlanItemSavedMessage(BasePlanItem planItem) : this(null, planItem)
        {
        }


        public PlanItemSavedMessage(BasePlanItem oldPlanItem, BasePlanItem planItem)
        {
            this.OldPlanItem = oldPlanItem;
            this.PlanItem = planItem;
        }


        public BasePlanItem OldPlanItem
        {
            get;
            private set;
        }


        public BasePlanItem PlanItem
        {
            get;
            private set;
        }

    }
}
