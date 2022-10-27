using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class ChoosedPhotosMessage : ObjectsCollectionMessage<Photo>
    {
        public ChoosedPhotosMessage(ICollection<Photo> photos)
        {
            Objects = photos;
        }

        public ICollection<Photo> Photos
        {
            get { return Objects; }
        }
    }
}
