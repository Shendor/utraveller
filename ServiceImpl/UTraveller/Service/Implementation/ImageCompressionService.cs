using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class ImageCompressionService : IImageCompressionService
    {
        public void CompressImage(BitmapImage image)
        {
            image.DecodePixelWidth = image.PixelWidth / 3;
            image.DecodePixelHeight = image.PixelHeight / 3;
        }
    }
}
