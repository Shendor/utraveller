using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Model;

namespace UTraveller.Service.Implementation.Remote
{
    public class PhotoUploadFacebookService : IPhotoUploadService<FacebookPhotoUploadRequest>
    {
        private const string ALBUM_NAME = "utraveler photos private album";

        private IFacebookClientService facebookService;
        private IFacebookAuthService facebookAuthService;
        private FacebookAlbum albumToUpload;
        private IImageService imageService;
        private IImageCropService imageCropService;

        public PhotoUploadFacebookService(IFacebookClientService facebookService, IImageService imageService,
            IImageCropService imageCropService, IFacebookAuthService facebookAuthService)
        {
            this.facebookService = facebookService;
            this.imageService = imageService;
            this.imageCropService = imageCropService;
            this.facebookAuthService = facebookAuthService;
        }


        public async Task<string> UploadAndGetUrl(FacebookPhotoUploadRequest request)
        {
            if (!facebookAuthService.IsSignedIn())
            {
                return null;
            }

            if (albumToUpload == null)
            {
                var albums = await facebookService.GetAlbums();
                if (albums != null)
                {
                    foreach (var album in albums)
                    {
                        if (album.Name.Equals(ALBUM_NAME))
                        {
                            albumToUpload = album;
                            break;
                        }
                    }
                    if (albumToUpload == null)
                    {
                        var album = new FacebookAlbum();
                        album.Name = ALBUM_NAME;
                        album.PrivacyType = FacebookPrivacyType.SELF;
                        var result = await facebookService.CreateAlbum(album);
                        album.Id = result;
                        albumToUpload = album;
                    }
                }
            }

            if (albumToUpload != null && albumToUpload.Id != null && request.Photo.ImageStream != null)
            {
                var imageContent = request.Photo.ImageStream;
                   // await imageCropService.ChangeResolution(request.Photo.ImageStream, request.Photo.Width, request.Photo.Height,
                   //DEFAULT_COMPRESSED_WIDTH, DEFAULT_COMPRESSED_HEIGHT);

                var photoContentRequest = new SocialPostImageData();
                photoContentRequest.Name = request.Description + " " + request.Photo.Date.ToString("d");
                photoContentRequest.Description = request.Photo.Description;
                photoContentRequest.FileName = request.Photo.Name;
                photoContentRequest.PrivacyType = FacebookPrivacyType.SELF;
                photoContentRequest.ImageContent = imageService.ToBytes(imageContent);

                var uploadId = await facebookService.AddPhotoToAlbum(albumToUpload, photoContentRequest);
                if (uploadId != null)
                {
                    request.Photo.FacebookPhotoId = uploadId;
                    var imgUrl = await facebookService.GetImagePostUrl(request.Photo.FacebookPhotoId);
                    return request.Photo.ImageUrl = imgUrl;
                }
            }
            return null;
        }


        public async Task<bool> Delete(FacebookPhotoUploadRequest request)
        {
            if (facebookAuthService.IsSignedIn() && request.Photo.FacebookPhotoId != null)
            {
                return await facebookService.DeletePhoto(request.Photo.FacebookPhotoId);
            }
            return true;
        }
    }
}
