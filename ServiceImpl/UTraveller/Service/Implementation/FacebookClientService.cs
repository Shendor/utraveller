using Facebook;
using Newtonsoft.Json;
using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Exceptions;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Entity;

namespace UTraveller.Service.Implementation
{
    public class FacebookClientService : IFacebookClientService
    {
        private ISocialClientAccessToken<string> facebookAccessToken;
        private IPhotoRepository photoRepository;
        private IMessageRepository messageRepository;

        public FacebookClientService(ISocialClientAccessToken<string> facebookAccessToken,
            IPhotoRepository facebookPhotoPostRepository,
            IMessageRepository facebookMessagePostRepository)
        {
            this.facebookAccessToken = facebookAccessToken;
            this.photoRepository = facebookPhotoPostRepository;
            this.messageRepository = facebookMessagePostRepository;
        }


        public async Task<FacebookUserProfile> GetUserProfile()
        {
            var userProfile = new FacebookUserProfile();
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                try
                {
                    var result = await facebookClient.GetTaskAsync("/me");
                    if (result != null)
                    {
                        userProfile = JsonConvert.DeserializeObject<FacebookUserProfile>(result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return userProfile;
        }


        public async Task<string> PostImage(SocialPostImageData socialPostImageData)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var args = new Dictionary<string, object>();
                args["name"] = socialPostImageData.Name;
                args["link"] = socialPostImageData.Link;
                args["caption"] = socialPostImageData.Caption;
                args["description"] = socialPostImageData.Description;
                args["picture"] = new FacebookMediaObject
                {
                    ContentType = "image/jpeg",
                    FileName = socialPostImageData.FileName

                }.SetValue(socialPostImageData.ImageContent);

                args["message"] = socialPostImageData.Message;
                args["actions"] = "actions";
                args["privacy"] = new { value = socialPostImageData.PrivacyType.ToString() };

                var postId = await facebookClient.PostTaskAsync("me/photos", args);
                return postId.ToString();
            }
            return null;
        }


        public async Task<string> PostStatus(SocialPostData socialPostData)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var args = new Dictionary<string, object>();
                args["name"] = socialPostData.Name;
                args["caption"] = socialPostData.Caption;
                args["description"] = socialPostData.Description;
                args["message"] = socialPostData.Message;
                args["privacy"] = new { value = socialPostData.PrivacyType.ToString() };

                dynamic postId = await facebookClient.PostTaskAsync("me/feed", args);
                return postId["id"].ToString();
            }
            return null;
        }


        public async Task<string> CreateAlbum(FacebookAlbum album)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                Dictionary<string, object> albumParameters = new Dictionary<string, object>();
                albumParameters.Add("name", album.Name);
                albumParameters.Add("privacy", new { value = album.PrivacyType.ToString() });
                dynamic postId = await facebookClient.PostTaskAsync("/me/albums", albumParameters);
                return postId != null ? postId["id"].ToString() : null;
            }
            return null;
        }


        public async Task<string> PostPhotoToAlbum(FacebookAlbum album, SocialPostImageData imagePostData)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var args = new Dictionary<string, object>();
                args["name"] = imagePostData.Name;
                args["caption"] = imagePostData.Caption;
                args["description"] = imagePostData.Description;
                args["picture"] = new FacebookMediaObject
                {
                    ContentType = "image/jpeg",
                    FileName = imagePostData.FileName

                }.SetValue(imagePostData.ImageContent);

                args["message"] = imagePostData.Message;
                args["actions"] = "actions";
                args["privacy"] = new { value = imagePostData.PrivacyType.ToString() };

                if (string.IsNullOrEmpty(album.Id))
                {
                    album.Id = await CreateAlbum(album);
                }
                if (album.Id != null)
                {
                    dynamic postId = await facebookClient.PostTaskAsync(album.Id + "/photos", args);
                    return postId["id"].ToString();
                }
            }
            return null;
        }


        public async Task<string> AddPhotoToAlbum(FacebookAlbum album, SocialPostImageData imagePostData)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var args = new Dictionary<string, object>();
                args["name"] = imagePostData.Name;
                args["caption"] = imagePostData.Caption;
                args["description"] = imagePostData.Description;
                args["picture"] = new FacebookMediaObject
                {
                    ContentType = "image/jpeg",
                    FileName = imagePostData.FileName

                }.SetValue(imagePostData.ImageContent);

                args["message"] = imagePostData.Message;
                args["actions"] = "actions";
                args["timeline_visibility"] = "hidden";
                args["is_hidden"] = 1;
                args["no_story"] = 1;
                args["privacy"] = new { value = imagePostData.PrivacyType.ToString() };

                dynamic postId = await facebookClient.PostTaskAsync(album.Id + "/photos", args);
                return postId["id"].ToString();

            }
            return null;
        }


        public async Task<bool> DeletePhoto(string photoId)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var url = "https://graph.facebook.com/{0}?access_token={1}";
                try
                {
                    var result = await facebookClient.DeleteTaskAsync(String.Format(url, photoId, facebookAccessToken.AccessToken));
                    return (bool)result;
                }
                catch (FacebookOAuthException ex)
                {
                    Debug.WriteLine("Error when deleting facebook photo: " + ex.Message);
                }
            }
            return false;
        }


        public async Task<IList<FacebookAlbum>> GetAlbums()
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var albums = await facebookClient.GetTaskAsync("me/albums") as JsonObject;
                return JsonConvert.DeserializeObject<IList<FacebookAlbum>>(((JsonObject)albums)["data"].ToString());
            }
            return null;
        }


        public async Task CommentPhotoPost(long photoId, string commentText)
        {
            var postEntity = photoRepository.GetById(photoId);
            if (postEntity != null)
            {
                await CommentPost(commentText, postEntity.FacebookPostId);
            }
        }


        public async Task CommentMessagePost(long messageId, string commentText)
        {
            var postEntity = messageRepository.GetById(messageId);
            if (postEntity != null)
            {
                await CommentPost(commentText, postEntity.FacebookPostId);
            }
        }


        public async Task<FacebookComments> GetPhotoComments(long photoId)
        {
            var postEntity = photoRepository.GetById(photoId);
            if (postEntity != null)
            {
                return await GetCommentsForPostId(postEntity.FacebookPostId);
            }
            return null;
        }


        public async Task<FacebookComments> GetMessageComments(long messageId)
        {
            var postEntity = messageRepository.GetById(messageId);
            if (postEntity != null)
            {
                return await GetCommentsForPostId(postEntity.FacebookPostId);
            }
            return null;
        }


        public async Task<FacebookComments> GetComments(string url)
        {
            FacebookComments fbComments = null;
            try
            {
                var facebookClient = createFacebookClient();
                if (facebookClient != null)
                {
                    var comments = await facebookClient.GetTaskAsync(url);
                    fbComments = JsonConvert.DeserializeObject<FacebookComments>(((JsonObject)comments).ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot get facebook comments: " + ex.Message);
            }
            return fbComments;
        }


        public async Task<FacebookLikes> GetPhotoLikes(long photoId)
        {
            var postEntity = photoRepository.GetById(photoId);
            if (postEntity != null)
            {
                return await GetLikes(postEntity.FacebookPostId);
            }
            return null;
        }


        public async Task<FacebookLikes> GetMessageLikes(long messageId)
        {
            var postEntity = messageRepository.GetById(messageId);
            if (postEntity != null)
            {
                return await GetLikes(postEntity.FacebookPostId);
            }
            return null;
        }


        public async Task<string> GetImagePostUrl(string postId)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                dynamic result = await facebookClient.GetTaskAsync(postId) as JsonObject;
                return result != null ? result["source"].ToString() : null;
            }
            return null;
        }


        public bool IsPhotoPosted(long id)
        {
            var entity = photoRepository.GetById(id);
            return entity != null && entity.FacebookPostId != null;
        }


        public bool IsMessagePosted(long id)
        {
            var entity = messageRepository.GetById(id);
            return entity != null && entity.FacebookPostId != null;
        }


        private async Task<FacebookLikes> GetLikes(string postId)
        {
            FacebookLikes fbLikes = null;
            try
            {
                var facebookClient = createFacebookClient();
                if (facebookClient != null)
                {
                    var likes = await facebookClient.GetTaskAsync(postId + "/likes?summary=1");
                    fbLikes = JsonConvert.DeserializeObject<FacebookLikes>(((JsonObject)likes).ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot get facebook comments: " + ex.Message);
            }
            return fbLikes;
        }


        private async Task<FacebookComments> GetCommentsForPostId(string postId)
        {
            return await GetComments(postId + "/comments?summary=1");
        }


        private async Task CommentPost(string commentText, string postId)
        {
            var facebookClient = createFacebookClient();
            if (facebookClient != null)
            {
                var args = new Dictionary<string, object>();
                args["message"] = commentText;
                await facebookClient.PostTaskAsync(postId + "/comments", args);
            }
        }


        private FacebookClient createFacebookClient()
        {
            if (facebookAccessToken.AccessToken != null)
            {
                var facebookClient = new FacebookClient(facebookAccessToken.AccessToken);
                facebookClient.AccessToken = facebookAccessToken.AccessToken;
                return facebookClient;
            }
            else
            {
                return null;
            }
        }

    }
}
