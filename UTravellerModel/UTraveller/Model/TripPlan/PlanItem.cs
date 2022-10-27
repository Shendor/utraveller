using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class PlanItem : BasePlanItem
    {
        public PlanItem()
        {
            Type = PlanItemType.Other;
        }


        public PlanItemType Type
        {
            get;
            set;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is PlanItem))
            {
                return false;
            }

            var planItem = (PlanItem)obj;
            return planItem.Date == this.Date && (planItem.Type.Equals(Type)) &&
                (planItem.Coordinate == null || planItem.Coordinate.Equals(Coordinate));
        }


        public override int GetHashCode()
        {
            var hashCode = Date == null ? 1 : Date.GetHashCode();
            hashCode += Coordinate == null ? 0 : Coordinate.GetHashCode();
            hashCode += Type.GetHashCode();
            return hashCode;
        }
    }
}
