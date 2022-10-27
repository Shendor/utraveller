using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class ExcludeEventPhotosChangedMessage : ObjectsCollectionMessage<Photo>
    {
        public ExcludeEventPhotosChangedMessage(ICollection<Photo> photos)
            : base(photos)
        {
        }
    }
}
