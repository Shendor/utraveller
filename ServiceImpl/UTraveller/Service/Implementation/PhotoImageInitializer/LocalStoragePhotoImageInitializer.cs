using ServiceImpl.UTraveller.Service.Implementation.PhotoImageInitializer;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class LocalStoragePhotoImageInitializer : BasePhotoImageInitializer
    {
        private IImageTransferService localStoragePhotoTransferService;

        public LocalStoragePhotoImageInitializer(IImageTransferService localStoragePhotoTransferService)
        {
            this.localStoragePhotoTransferService = localStoragePhotoTransferService;
        }

        public override async Task<bool> InitializeImage(Photo photo)
        {
            photo.InitializeImage(await localStoragePhotoTransferService.Load(photo));
            return photo.ImageStream != null;
        }
    }
}
