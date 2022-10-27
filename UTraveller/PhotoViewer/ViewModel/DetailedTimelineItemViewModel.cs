using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.PhotoViewer.ViewModel
{
    public class DetailedTimelineItemViewModel : BaseTimeLineItemViewModel<IDateItem>, IDescriptionedTimeLineItem
    {
        public DetailedTimelineItemViewModel(int positionNumber, IDateItem dateItem)
            : base(dateItem)
        {
            PositionNumber = positionNumber;
        }


        public long Id
        {
            get { return ((BaseModel)dateItem).Id; }
        }


        public int PositionNumber
        {
            get;
            private set;
        }


        public bool IsSelected
        {
            get;
            set;
        }


        public BitmapImage Image
        {
            get { return DateItem is Photo ? ((Photo)DateItem).Image : null; }
        }


        public string Text
        {
            get { return DateItem is Message ? ((Message)DateItem).Text : ((Photo)DateItem).Description; }
        }


        public string ImageLoadStatusText
        {
            get { return DateItem is Photo ? 
                string.Format("Can't find photo with name '{0}' on your device or on the Cloud :(", ((Photo)DateItem).Name) : null; }
        }


        public void UpdateDescription()
        {
            RaisePropertyChanged("Text");
        }


        protected override void OnTimeLineItemDeleted()
        {
            throw new NotImplementedException();
        }

    }
}
