using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.Model
{
    public class PlanItemTypeModel : IComparable<PlanItemTypeModel>
    {

        public PlanItemTypeModel(string name)
            : this(null, name)
        {
        }


        public PlanItemTypeModel(Enum type, string name)
        {
            SelectedPlanItemType = type;
            Name = name;
        }


        public EditPlanItemType EditPlanItemType
        {
            get
            {
                if (SelectedPlanItemType is PlanItemType)
                {
                    return Model.EditPlanItemType.Other;
                }
                else if (SelectedPlanItemType is RentPlanItemType)
                {
                    return Model.EditPlanItemType.Rent;
                }
                else
                {
                    return Model.EditPlanItemType.Transport;
                }
            }
        }


        public Enum SelectedPlanItemType
        {
            get;
            private set;
        }


        public string Name
        {
            get;
            private set;
        }

        public Uri Icon
        {
            get { return PlanItemTypeUtil.GetIcon(SelectedPlanItemType); }
        }

        public override string ToString()
        {
            return Name;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is PlanItemTypeModel))
            {
                return false;
            }

            return ((PlanItemTypeModel)obj).Name == this.Name;
        }


        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        public int CompareTo(PlanItemTypeModel other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
