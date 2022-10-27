using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class DeletedPhotoMessage
    {
        public DeletedPhotoMessage(Photo photo)
        {
            Photo = photo;
        }

        public Photo Photo
        {
            get;
            set;
        }
    }
}
