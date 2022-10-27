using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.PhotoCrop.Message
{
    public class ImageCroppedMessage : ObjectMessage<Stream>
    {
        public ImageCroppedMessage(Stream imageStream)
            : base(imageStream)
        {
        }
    }
}
