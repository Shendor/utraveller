using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class PlanItemViewModel : BasePlanItemViewModel<PlanItem>
    {

        public PlanItemViewModel(PlanItem planItem)
            : base(planItem)
        {
        }


        public override string Description
        {
            get { return string.IsNullOrEmpty(basePlanItem.Description) ? basePlanItem.Address : basePlanItem.Description; }
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
