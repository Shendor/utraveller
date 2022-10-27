using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.PhotoCrop.Message
{
    public class ImageToCropChangedMessage : ObjectMessage<Stream>
    {
        public ImageToCropChangedMessage(Stream imageStream, int cropWidth, int cropHeight, object cropImageTypeId)
            : base(imageStream)
        {
            CropWidth = cropWidth;
            CropHeight = cropHeight;
            CropImageTypeId = cropImageTypeId;
        }


        public int CropWidth
        {
            get;
            private set;
        }


        public int CropHeight
        {
            get;
            private set;
        }


        public object CropImageTypeId 
        {
            get; set; 
        }
    }
}
