using Microsoft.Live;
using ServiceImpl.UTraveller.Service.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;

namespace UTraveller.Service.Implementation.Remote
{
    public class PhotoUploadOneDriveService : IPhotoUploadService<OneDrivePhotoUploadRequest>
    {
        private static readonly string DEFAULT_ALBUM_NAME = "uTraveler";
        private string folderUploadUrl;
        private ISocialClientAccessToken<LiveConnectSession> liveAccessToken;
        private IImageCropService imageCropService;
        

        public PhotoUploadOneDriveService(ISocialClientAccessToken<LiveConnectSession> liveAccessToken,
            IImageCropService imageCropService)
        {
            this.liveAccessToken = liveAccessToken;
            this.imageCropService = imageCropService;
        }


        public async Task<string> UploadAndGetUrl(OneDrivePhotoUploadRequest request)
        {
            if (liveAccessToken.AccessToken == null)
            {
                return null;
            }

            LiveConnectClient liveClient = new LiveConnectClient(liveAccessToken.AccessToken);
            await InitializeFolderUploadUrl(liveClient);
            
            if (request.Photo.ImageStream != null)
            {
                var imageContent = await imageCropService.ChangeResolution(request.Photo.ImageStream, request.Photo.Width, request.Photo.Height);

                var uploadResult = await liveClient.UploadAsync(folderUploadUrl, request.Photo.Name, imageContent, OverwriteOption.Rename);
                request.Photo.FacebookPhotoId = uploadResult.Result["id"].ToString();
                var img = await liveClient.GetAsync(request.Photo.FacebookPhotoId);

                return request.Photo.ImageUrl = img.Result != null ? img.Result["source"].ToString() : null;
            }
            return null;
        }


        private async Task InitializeFolderUploadUrl(LiveConnectClient liveClient)
        {
            if (folderUploadUrl == null)
            {
                LiveOperationResult operationResult = await liveClient.GetAsync("/me/skydrive/files");

                var data = (List<object>)operationResult.Result["data"];
                foreach (IDictionary<string, object> content in data)
                {
                    var isDirectory = content["type"].Equals("folder") || content["type"].Equals("album") ? true : false;
                    var fileName = (string)content["name"];
                    var uploadLocation = (string)content["id"];
                    if (isDirectory && fileName.Equals(DEFAULT_ALBUM_NAME))
                    {
                        folderUploadUrl = uploadLocation;
                        break;
                    }
                }
                if (data.Count > 0 && folderUploadUrl == null)
                {
                    var folderData = new Dictionary<string, object>();
                    folderData.Add("name", DEFAULT_ALBUM_NAME);
                    var result = await liveClient.PostAsync("me/skydrive", folderData);
                    folderUploadUrl = result.Result["id"].ToString();
                }
            }
        }


        public async Task<bool> Delete(OneDrivePhotoUploadRequest request)
        {
            if (liveAccessToken.AccessToken != null && request.Photo.FacebookPhotoId != null)
            {
                LiveConnectClient liveClient = new LiveConnectClient(liveAccessToken.AccessToken);
                var result = await liveClient.DeleteAsync(request.Photo.FacebookPhotoId);
                return result.Result != null;
            }
            return true;
        }
    }
}
