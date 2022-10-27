using GalaSoft.MvvmLight;
using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using UTraveller.Common.Control;
using UTraveller.Common.ViewModel;
using UTraveller.PhotoCrop.Message;
using UTraveller.Resources;
using UTraveller.Service.Api;
using Windows.Storage.Streams;

namespace UTraveller.PhotoCrop.ViewModel
{
    public class PhotoCropViewModel : BaseViewModel
    {
        private static readonly int DEFAULT_DECODED_IMAGE_WIDTH = 1536;
        private static readonly int DEFAULT_DECODED_IMAGE_HEIGHT = 800;

        private IImageCropService imageCropService;
        private IImageService imageService;
        private Stream image;
        private Windows.Foundation.Size imageSize;
        private object messageDeliveryToken;
        private NotificationService notificationService;

        public PhotoCropViewModel(IImageCropService imageCropService, IImageService imageService,
            NotificationService notificationService)
        {
            this.imageCropService = imageCropService;
            this.imageService = imageService;
            this.notificationService = notificationService;

            Photo = new BitmapImage();
            Photo.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            MessengerInstance.Register<ImageToCropChangedMessage>(this, OnCropImageChanged);
        }


        public void Initialize()
        {
            CropWidth = 300;
            CropHeight = 300;
        }


        public override void Cleanup()
        {
            if (image != null)
            {
                messageDeliveryToken = null;
                Photo.UriSource = null;
                image = null;
            }
        }


        public BitmapImage Photo
        {
            get;
            set;
        }


        public int CropWidth
        {
            get;
            set;
        }


        public int CropHeight
        {
            get;
            set;
        }


        public double ImageHeight
        {
            get { return imageSize.Height; }
        }


        public double ImageWidth
        {
            get { return imageSize.Width; }
        }


        private void InitializeImage()
        {
            if (image != null)
            {
                try
                {
                    imageSize = imageService.GetImageSize(image);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot get image resolution. " + ex.Message);
                    imageSize = new Windows.Foundation.Size(DEFAULT_DECODED_IMAGE_WIDTH, DEFAULT_DECODED_IMAGE_HEIGHT);
                }

                if (imageSize.Width >= imageSize.Height)
                {
                    Photo.DecodePixelWidth = DEFAULT_DECODED_IMAGE_WIDTH;
                    Photo.DecodePixelHeight = 0;
                }
                else
                {
                    Photo.DecodePixelWidth = 0;
                    Photo.DecodePixelHeight = DEFAULT_DECODED_IMAGE_WIDTH;
                }
                image.Position = 0;
                Photo.SetSource(image);
            }
        }


        public void ApplyCrop(Point topLeft, Point bottomRight)
        {
            try
            {
                var croppedImage = imageCropService.CropImage(image, topLeft, bottomRight);
                MessengerInstance.Send<ImageCroppedMessage>(new ImageCroppedMessage(croppedImage), messageDeliveryToken);
            }
            catch (Exception ex)
            {
                notificationService.Show(AppResources.PhotoCrop_Wrong_Extension);
            }
        }


        private void OnCropImageChanged(ImageToCropChangedMessage message)
        {
            image = message.Object;
            CropWidth = message.CropWidth;
            CropHeight = message.CropHeight;
            messageDeliveryToken = message.CropImageTypeId;
            InitializeImage();
        }

    }
}
