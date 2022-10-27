using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class BaseUserRemoteModelMapper : IModelMapper<User, BaseUserRemoteModel>
    {
        public User MapEntity(BaseUserRemoteModel entity)
        {
            var user = new User();
            user.RemoteId = entity.Id;
            user.Name = entity.Name;
            user.Description = entity.Description;
            user.AvatarContent = entity.Avatar;
            user.CoverContent = entity.Cover;

            return user;
        }

        public BaseUserRemoteModel MapModel(User model)
        {
            var userRemoteModel = new BaseUserRemoteModel();
            userRemoteModel.Id = model.RemoteId;
            userRemoteModel.Name = model.Name;
            userRemoteModel.Description = model.Description;
            userRemoteModel.Avatar = model.AvatarContent;
            userRemoteModel.Cover = model.CoverContent;

            return userRemoteModel;
        }
    }
}
