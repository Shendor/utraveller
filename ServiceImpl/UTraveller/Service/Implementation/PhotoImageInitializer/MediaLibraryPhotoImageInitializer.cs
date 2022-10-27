using ServiceImpl.UTraveller.Service.Implementation.PhotoImageInitializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class MediaLibraryPhotoImageInitializer : BasePhotoImageInitializer
    {
        private IImageLoaderService mediaLibraryPhotoLoaderService;

        public MediaLibraryPhotoImageInitializer(IImageLoaderService mediaLibraryPhotoLoaderService)
        {
            this.mediaLibraryPhotoLoaderService = mediaLibraryPhotoLoaderService;
        }

        public override async Task<bool> InitializeImage(Photo photo)
        {
            var result = await mediaLibraryPhotoLoaderService.Load(photo);
            photo.InitializeImage(result);
            return photo.ImageStream != null;
        }
    }
}
