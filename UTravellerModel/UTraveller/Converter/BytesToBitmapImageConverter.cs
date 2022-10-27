using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTraveller.Common.Converter
{
    public class BytesToBitmapImageConverter : IConverter<byte[], BitmapImage>
    {
        public BitmapImage Convert(byte[] origin)
        {
            BitmapImage bitmapImage = new BitmapImage();

            if (origin != null)
            {
                ToStream(bitmapImage, origin);
            }
            return bitmapImage;
        }


        public static void ToStream(BitmapImage bitmapImage, byte[] origin)
        {
            if (origin != null)
            {
                using (MemoryStream ms = new MemoryStream(origin))
                {
                    bitmapImage.SetSource(ms);
                }
            }
        }


        public static byte[] ToBytes(Stream origin)
        {
            origin.Position = 0;
            byte[] bytes = new byte[origin.Length];
            origin.Read(bytes, 0, (int)origin.Length);
            return bytes;
        }

    }
}
