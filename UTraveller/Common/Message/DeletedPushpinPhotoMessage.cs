using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class DeletedPushpinPhotoMessage : DeletedPhotoMessage
    {
        public DeletedPushpinPhotoMessage(Photo photo)
            : base(photo)
        {
        } 
    }
}
