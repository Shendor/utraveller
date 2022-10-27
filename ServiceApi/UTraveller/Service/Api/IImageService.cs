using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace UTraveller.Service.Api
{
    public interface IImageService
    {
        Size GetImageSize(Stream imageStream);

        IBuffer StreamToBuffer(Stream stream);

        Stream BufferToStream(IBuffer buffer);

        void WriteBytesToBitmapImage(byte[] origin, BitmapImage bitmapImage);

        byte[] ToBytes(BitmapImage origin);

        byte[] ToBytes(Stream origin);

        Task<byte[]> ReadRemoteImage(string url);
    }
}
