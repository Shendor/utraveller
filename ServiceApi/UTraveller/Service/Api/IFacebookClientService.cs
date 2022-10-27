using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UTraveller.Service.Model;

namespace UTraveller.Service.Api
{
    public interface IFacebookClientService
    {
        Task<FacebookUserProfile> GetUserProfile();

        Task<string> PostImage(SocialPostImageData socialPostImageData);

        Task<string> PostStatus(SocialPostData socialPostData);

        Task<string> CreateAlbum(FacebookAlbum name);

        Task<IList<FacebookAlbum>> GetAlbums();

        Task<string> PostPhotoToAlbum(FacebookAlbum album, SocialPostImageData imagePostData);

        Task<string> AddPhotoToAlbum(FacebookAlbum album, SocialPostImageData imagePostData);

        Task<bool> DeletePhoto(string photoId);

        Task CommentPhotoPost(long photoId, string commentText);

        Task CommentMessagePost(long messageId, string commentText);

        Task<FacebookComments> GetPhotoComments(long photoId);

        Task<FacebookComments> GetMessageComments(long messageId);

        Task<FacebookComments> GetComments(string url);

        Task<FacebookLikes> GetPhotoLikes(long photoId);

        Task<FacebookLikes> GetMessageLikes(long messageId);

        Task<string> GetImagePostUrl(string postId);

        bool IsPhotoPosted(long id);

        bool IsMessagePosted(long id);
    }
}
