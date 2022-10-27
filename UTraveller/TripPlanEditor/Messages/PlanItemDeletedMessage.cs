using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.Messages
{
    public class PlanItemDeletedMessage
    {

        public PlanItemDeletedMessage(BasePlanItem planItem)
        {
            this.PlanItem = planItem;
        }


        public BasePlanItem PlanItem
        {
            get;
            private set;
        }
    }
}
