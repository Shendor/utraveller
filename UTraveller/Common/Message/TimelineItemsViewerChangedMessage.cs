using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class TimelineItemsViewerChangedMessage<T> : ObjectsCollectionMessage<T> where T : IDateItem
    {
        public TimelineItemsViewerChangedMessage(ICollection<T> photos)
            : this(default(T), photos)
        {
        }

        public TimelineItemsViewerChangedMessage(T startFrom, ICollection<T> photos)
            : base(photos)
        {
            StartFrom = startFrom;
        }

        public T StartFrom
        {
            get;
            set;
        }
    }
}
