using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace ServiceImpl.UTraveller.Service.Model
{
    public class OneDrivePhotoUploadRequest
    {

        public OneDrivePhotoUploadRequest(Photo photo)
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
