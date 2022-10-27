using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class DeletedEventPhotoMessage : DeletedPhotoMessage
    {
        public DeletedEventPhotoMessage(Photo photo)
            : base(photo)
        {
        } 
    }
}
