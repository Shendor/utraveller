using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;

namespace UTraveller.EventDetails.ViewModel
{
    public class GroupedPlanItemViewModel : BaseListViewModel<IPlanItemViewModel>
    {
        public GroupedPlanItemViewModel(IGrouping<string, IPlanItemViewModel> pair)
            : base(pair)
        {
            Day = pair.Key;
        }


        public string Day
        {
            protected set;
            get;
        }


        public override bool Equals(object obj)
        {
            if (obj is GroupedPlanItemViewModel)
            {
                return Day.Equals(((GroupedPlanItemViewModel)obj).Day);
            }
            return false;
        }
        
        
        public override int GetHashCode()
        {
            return Day.GetHashCode();
        }


        public override string ToString()
        {
            return Day;
        }
    }
}
