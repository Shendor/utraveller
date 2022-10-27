using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class RentPlanItem : BasePlanItem
    {
        public RentPlanItem()
        {
            Type = RentPlanItemType.Hotel;
        }


        public RentPlanItemType Type
        {
            get;
            set;
        }


        public DateTime? CheckOutDate
        {
            get;
            set;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is RentPlanItem))
            {
                return false;
            }

            var planItem = (RentPlanItem)obj;
            return planItem.Date == this.Date && (planItem.Type.Equals(Type)) && planItem.CheckOutDate == CheckOutDate &&
                (planItem.Coordinate == null || planItem.Coordinate.Equals(Coordinate));
        }


        public override int GetHashCode()
        {
            var hashCode = Date == null ? 1 : Date.GetHashCode();
            hashCode += CheckOutDate == null ? 1 : CheckOutDate.GetHashCode();
            hashCode += Coordinate == null ? 0 : Coordinate.GetHashCode();
            hashCode += Type.GetHashCode();
            
            return hashCode;
        }
    }
}
