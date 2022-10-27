using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;
using Microsoft.Phone;
using Windows.Foundation;
using ExifLib;

namespace UTraveller.Service.Implementation
{
    public class ImageCropService : IImageCropService
    {
        public const int DEFAULT_COMPRESSED_WIDTH = 1280;
        public const int DEFAULT_COMPRESSED_HEIGHT = 768;

        private IImageService imageService;

        public ImageCropService(IImageService imageService)
        {
            this.imageService = imageService;
        }


        public Stream CropImage(Stream origin, System.Windows.Point topLeft, System.Windows.Point bottomRight)
        {
            origin.Position = 0;
            using (StreamImageSource source = new StreamImageSource(origin))
            {
                var topLeftFoundationPoint = new Point(Math.Round(topLeft.X), Math.Round(topLeft.Y));
                var bottomRightFoundationPoint = new Point(Math.Round(bottomRight.X), Math.Round(bottomRight.Y));

                var reframingFilter = new ReframingFilter()
                {
                    ReframingArea = new Rect(topLeftFoundationPoint, bottomRightFoundationPoint)
                };

                var filterEffect = new FilterEffect(source)
                {
                    Filters = new List<IFilter>() { reframingFilter }
                };

                var renderer = new JpegRenderer(filterEffect)
                {
                    OutputOption = OutputOption.PreserveAspectRatio,
                    Quality = 1.0,
                    Size = new Size(bottomRightFoundationPoint.X - topLeftFoundationPoint.X,
                        bottomRightFoundationPoint.Y - topLeftFoundationPoint.Y)
                };


                IBuffer buffer = null;

                Task.Run(async () => { buffer = await renderer.RenderAsync(); }).Wait();

                return buffer.AsStream();
            }
        }


        public void ChangeResolution(Stream origin, Stream result, int desiredWidth, int desiredHeight, int quality = 100)
        {
            if (origin != null)
            {
                var decodedImage = PictureDecoder.DecodeJpeg(origin, desiredWidth, desiredHeight);
                Extensions.SaveJpeg(decodedImage, result, decodedImage.PixelWidth, decodedImage.PixelHeight, 0, quality);
            }
        }


        public async Task<Stream> ChangeResolution(Stream origin, int originWidth, int originHeight,
            int desiredWidth = DEFAULT_COMPRESSED_WIDTH, int desiredHeight = DEFAULT_COMPRESSED_HEIGHT)
        {
            if (origin != null && (desiredWidth < originWidth || desiredHeight < originHeight))
            {
                using (var imageProvider = new StreamImageSource(origin, ImageFormat.Jpeg))
                {
                    IFilterEffect effect = new FilterEffect(imageProvider);

                    Size desiredSize = new Size(desiredWidth, desiredHeight);

                    using (var renderer = new JpegRenderer(effect))
                    {
                        renderer.OutputOption = OutputOption.PreserveAspectRatio;

                        renderer.Size = desiredSize;

                        IBuffer buffer = await renderer.RenderAsync();
                        return buffer.AsStream();
                    }
                }
            }
            else
            {
                return origin;
            }
        }


        public byte[] ChangeResolutionAndGetBytes(Stream origin, int desiredWidth, int desiredHeight, int quality = 100)
        {
            if (origin != null)
            {
                using (var result = new MemoryStream())
                {
                    ChangeResolution(origin, result, desiredWidth, desiredHeight, quality);
                    return imageService.ToBytes(result);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the area needed to crop an image to the desired height and width.
        /// </summary>
        /// <param name="imageSize">The size of the image.</param>
        /// <param name="desiredSize">The desired size to crop the image to.</param>
        /// <returns></returns>
        private static Rect GetCropArea(Size imageSize, Size desiredSize)
        {
            // how big is the picture compared to the phone?
            var widthRatio = desiredSize.Width / imageSize.Width;
            var heightRatio = desiredSize.Height / imageSize.Height;

            // the ratio is the same, no need to crop it
            if (widthRatio == heightRatio) return new Rect(0, 0, imageSize.Width, imageSize.Height);

            double cropWidth;
            double cropHeight;
            if (widthRatio < heightRatio)
            {
                cropHeight = imageSize.Height;
                cropWidth = desiredSize.Width / heightRatio;
            }
            else
            {
                cropHeight = desiredSize.Height / widthRatio;
                cropWidth = imageSize.Width;
            }

            int left = (int)(imageSize.Width - cropWidth) / 2;
            int top = (int)(imageSize.Height - cropHeight) / 2;

            var rect = new Rect(left, top, cropWidth, cropHeight);
            return rect;
        }
    }
}
