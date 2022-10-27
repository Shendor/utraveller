using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class RentPlanItemViewModel : BasePlanItemViewModel<RentPlanItem>
    {
        private RentPlanItem rentPlanItem;
        private bool isCheckOutDate;

        public RentPlanItemViewModel(RentPlanItem rentPlanItem)
            : this(rentPlanItem, false)
        {

        }


        public RentPlanItemViewModel(RentPlanItem rentPlanItem, bool isCheckOutDate)
            : base(rentPlanItem)
        {
            this.rentPlanItem = rentPlanItem;
            this.isCheckOutDate = isCheckOutDate;
        }


        public override string ShortFormattedDate
        {
            get
            {
                var dateType = isCheckOutDate ? AppResources.PlanItem_CheckOut : AppResources.PlanItem_CheckIn;
                return Date != null ? string.Format("{0} ({1})",
                    Date.Value.ToString(AppResources.Short_No_Week_Name_Date_Format, App.CurrentCulture), dateType) :
                    UNDATED_LABEL;
            }
        }


        public override DateTime? Date
        {
            get { return isCheckOutDate ? basePlanItem.CheckOutDate : basePlanItem.Date; }
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
