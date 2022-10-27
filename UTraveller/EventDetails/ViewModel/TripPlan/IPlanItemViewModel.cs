using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public interface IPlanItemViewModel : IComparable<IPlanItemViewModel>
    {

        BasePlanItem BasePlanItem
        {
            get;
        }


        DateTime? Date
        {
            get;
        }


        string Caption
        {
            get;
        }


        Uri Icon
        {
            get;
        }


        string Description
        {
            get;
        }

        void EditPlanItem();
    }
}
