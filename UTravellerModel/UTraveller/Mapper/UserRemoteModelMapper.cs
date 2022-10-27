using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class UserRemoteModelMapper : IModelMapper<User, UserRemoteModel>
    {
        public User MapEntity(UserRemoteModel entity)
        {
            var user = new User();
            user.RemoteId = entity.Id;
            user.Name = entity.Name;
            user.Email = entity.Email;
            user.Description = entity.Description;
            user.AvatarContent = entity.Avatar;
            user.CoverContent = entity.Cover;

            return user;
        }

        public UserRemoteModel MapModel(User model)
        {
            var userRemoteModel = new UserRemoteModel();
            userRemoteModel.Id = model.RemoteId;
            userRemoteModel.Name = model.Name;
            userRemoteModel.Email = model.Email;
            userRemoteModel.Description = model.Description;
            userRemoteModel.Avatar = model.AvatarContent;
            userRemoteModel.Cover = model.CoverContent;

            return userRemoteModel;
        }
    }
}
