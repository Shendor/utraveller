using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTraveller.Common.Converter
{
    public class BitmapImageToBytesConverter : IConverter<BitmapImage, byte[]>
    {
        public byte[] Convert(BitmapImage origin)
        {
            if (origin != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    
                    WriteableBitmap btmMap = new WriteableBitmap(origin);

                    btmMap.SaveJpeg(ms, origin.PixelWidth, origin.PixelHeight, 0, 100);

                    return ms.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
    }
}
