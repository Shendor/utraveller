using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation.PhotoTransfer
{
    public class RemotePhotoLoaderService : IImageLoaderService
    {
        private IImageLoaderService mediaLibraryPhotoLoaderService;

        public RemotePhotoLoaderService(IImageLoaderService mediaLibraryPhotoLoaderService)
        {
            this.mediaLibraryPhotoLoaderService = mediaLibraryPhotoLoaderService;
        }


        public async Task<Stream> Load(Photo photo)
        {
            Stream stream = await mediaLibraryPhotoLoaderService.Load(photo);
            if (stream == null)
            {
                try
                {
                    WebClient client = new WebClient();
                    stream = await client.OpenReadTaskAsync(photo.ImageUrl);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Cannot load image of photo remotely for url {0}: {1}",
                        photo.ImageUrl, ex.Message));
                }
            }
            return stream;
        }
    }
}
