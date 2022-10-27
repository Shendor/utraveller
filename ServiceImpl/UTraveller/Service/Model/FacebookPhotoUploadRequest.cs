using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Model
{
    public class FacebookPhotoUploadRequest
    {
        public FacebookPhotoUploadRequest(Photo photo, string description = "")
        {
            Photo = photo;
            Description = description;
        }


        public string Description
        {
            get;
            set;
        }


        public Photo Photo
        {
            get;
            set;
        }
    }
}
