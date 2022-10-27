using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.EventMap.ViewModel.Map;

namespace UTraveller.EventMap.Messages
{
    public class FilterPlanItemPushpinMessage
    {
        private ViewModel.Map.PlanItemLegendViewModel planItemLegendViewModel;

        public FilterPlanItemPushpinMessage(PlanItemLegendViewModel planItemLegendViewModel)
        {
            this.PlanItemLegend = planItemLegendViewModel;
        }


        public PlanItemLegendViewModel PlanItemLegend
        {
            get;
            private set;
        }
    }
}
