using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTraveller.Service.Api
{
    public interface IImageCompressionService
    {
        void CompressImage(BitmapImage image);
    }
}
