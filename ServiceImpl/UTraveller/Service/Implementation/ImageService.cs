using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using System.IO;
using ExifLib;
using UTraveller.Service.Api;
using Windows.Foundation;
using System.Windows.Media.Imaging;
using System.Net;
using System.Diagnostics;

namespace UTraveller.Service.Implementation
{
    public class ImageService : IImageService
    {
        public Size GetImageSize(Stream imageStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                imageStream.Position = 0;
                imageStream.CopyTo(memoryStream);

                try
                {
                    // Some streams do not support flushing
                    imageStream.Flush();
                }
                catch (Exception)
                {
                }

                var source = new BitmapImage();
                source.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                source.SetSource(memoryStream);

                var size = new Size(source.PixelWidth, source.PixelHeight);
                source.UriSource = null;
                source = null;
                imageStream.Position = 0;
                return size;
            }
        }


        public IBuffer StreamToBuffer(Stream stream)
        {
            var memoryStream = stream as MemoryStream;

            if (memoryStream == null)
            {
                using (memoryStream = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memoryStream);

                    try
                    {
                        // Some stream types do not support flushing

                        stream.Flush();
                    }
                    catch (Exception)
                    {
                    }

                    return memoryStream.GetWindowsRuntimeBuffer();
                }
            }
            else
            {
                return memoryStream.GetWindowsRuntimeBuffer();
            }
        }


        public Stream BufferToStream(IBuffer buffer)
        {
            return buffer.AsStream();
        }


        public void WriteBytesToBitmapImage(byte[] origin, BitmapImage bitmapImage)
        {
            if (origin != null)
            {
                using (MemoryStream ms = new MemoryStream(origin))
                {
                    bitmapImage.SetSource(ms);
                }
            }
        }


        public byte[] ToBytes(BitmapImage origin)
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


        public byte[] ToBytes(Stream origin)
        {
            origin.Position = 0;
            byte[] bytes = new byte[origin.Length];
            origin.Read(bytes, 0, (int)origin.Length);
            return bytes;
        }


        public async Task<byte[]> ReadRemoteImage(string url)
        {
            WebClient client = new WebClient();
            try
            {
                var stream = await client.OpenReadTaskAsync(url);
                return ToBytes(stream);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
