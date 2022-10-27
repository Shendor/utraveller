using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class TransportPlanItem : BasePlanItem
    {
        public TransportPlanItemType Type
        {
            get;
            set;
        }


        public string Destination
        {
            get;
            set;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is TransportPlanItem))
            {
                return false;
            }

            var planItem = (TransportPlanItem)obj;
            return planItem.Date == this.Date && (planItem.Type.Equals(Type)) && 
                (string.IsNullOrEmpty(planItem.Destination) || planItem.Destination.Equals(Destination)) &&
                (string.IsNullOrEmpty(planItem.Description) || planItem.Description.Equals(Description)) &&
                (planItem.Coordinate == null || planItem.Coordinate.Equals(Coordinate));
        }


        public override int GetHashCode()
        {
            var hashCode = Date == null ? 0 : Date.GetHashCode();
            hashCode += Type.GetHashCode();
            hashCode += string.IsNullOrEmpty(Destination) ? 0 : Destination.GetHashCode();
            hashCode += Coordinate == null ? 0 : Coordinate.GetHashCode();
            hashCode += string.IsNullOrEmpty(Description) ? 0 : Description.GetHashCode();
            return hashCode;
        }
    }
}
