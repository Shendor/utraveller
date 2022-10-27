using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class GroupedEventPhotoViewModel : BaseListViewModel<ITimeLineItem<IDateItem>>
    {
        public GroupedEventPhotoViewModel(IGrouping<string, ITimeLineItem<IDateItem>> photoViewModelsGroup)
            :base(photoViewModelsGroup)
        {
            Day = photoViewModelsGroup.Key;
        }


        public string Day
        {
            protected set;
            get;
        }


        public override bool Equals(object obj)
        {
            if (obj is GroupedEventPhotoViewModel)
            {
                return Day.Equals(((GroupedEventPhotoViewModel)obj).Day);
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
