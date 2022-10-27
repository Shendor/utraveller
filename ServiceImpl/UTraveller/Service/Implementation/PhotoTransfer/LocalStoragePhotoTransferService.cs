using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class LocalStoragePhotoTransferService : IImageTransferService
    {
        private static readonly string SAVE_ERROR_MESSAGE = "Cannot save file with name {0} because it already exists";
        private static readonly string LOAD_ERROR_MESSAGE = "Cannot load file with name {0}";

        private IImageLoaderService mediaLibraryPhotoTransferService;
        private IImageCompressionService imageCompressionService;
        private IImageCropService imageDecodingService;

        public LocalStoragePhotoTransferService(IImageCompressionService imageCompressionService,
            IImageCropService imageDecodingService, IImageLoaderService mediaLibraryPhotoTransferService)
        {
            this.imageCompressionService = imageCompressionService;
            this.imageDecodingService = imageDecodingService;
            this.mediaLibraryPhotoTransferService = mediaLibraryPhotoTransferService;
        }

        public void Save(Photo photo)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!iso.FileExists(photo.Name))
                {
                    SavePhotoAsJpeg(photo, iso); //TODO name should depend on event id
                }
                else
                {
                    iso.Dispose();
                    throw new IsolatedStorageException(String.Format(SAVE_ERROR_MESSAGE, photo.Name));
                }
            }
        }

        public void Delete(Photo photo)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                iso.DeleteFile(photo.Name); //TODO name should depend on event id
            }
        }

        public async Task<Stream> Load(Photo photo)
        {
            //byte[] data;
            Stream stream = null;

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.FileExists(photo.Name))
                    {
                        using (IsolatedStorageFileStream isfs = isf.OpenFile(photo.Name, FileMode.Open, FileAccess.Read))
                        {
                            //data = new byte[isfs.Length];
                            //isfs.Read(data, 0, data.Length);
                            stream = new MemoryStream();
                            isfs.CopyTo(stream);
                            isfs.Close();
                        }
                        //stream = new MemoryStream(data);
                    }
                }

                if (stream == null)
                {
                    stream = mediaLibraryPhotoTransferService.Load(photo).Result;
                }

                return stream;
            }
            catch (Exception e)
            {
                throw new IsolatedStorageException(String.Format(LOAD_ERROR_MESSAGE, photo.Name), e);
            }
        }


        private void SavePhotoAsJpeg(Photo photo, IsolatedStorageFile iso)
        {
            using (IsolatedStorageFileStream isostream = iso.CreateFile(photo.Name))
            {
                //BitmapImage bitmap = new BitmapImage();
                //bitmap.SetSource(imageStream);
                //imageCompressionService.CompressImage(bitmap);

                int desiredWidth = Photo.DEFAULT_COMPRESS_WIDTH;
                int desiredHeight = Photo.DEFAULT_COMPRESS_HEIGHT;
                if (photo.Width < photo.Height)
                {
                    desiredWidth = desiredHeight;
                    desiredHeight = Photo.DEFAULT_COMPRESS_WIDTH;
                }
                imageDecodingService.ChangeResolution(photo.ImageStream, isostream, desiredWidth, desiredHeight);

                //WriteableBitmap writableBitmap = new WriteableBitmap(bitmap);
                //Extensions.SaveJpeg(writableBitmap, isostream, bitmap.DecodePixelWidth, bitmap.DecodePixelHeight, 0, 100);

                //bitmap.ClearValue(BitmapImage.UriSourceProperty);

                photo.ImageStream.Close();
                isostream.Close();
            }
            Debug.WriteLine(Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage);
        }

    }
}
