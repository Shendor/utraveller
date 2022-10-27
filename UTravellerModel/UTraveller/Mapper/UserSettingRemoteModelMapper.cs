using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTravellerModel.UTraveller.Converter;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class UserSettingRemoteModelMapper : IModelMapper<AppProperties, UserSettingRemoteModel>
    {
        public AppProperties MapEntity(UserSettingRemoteModel entity)
        {
            var userSetting = new AppProperties();
            userSetting.Background = new SolidColorBrush(HexColorConverter.GetColorFromHex(entity.BackgroundColor));
            userSetting.MainColor = new SolidColorBrush(HexColorConverter.GetColorFromHex(entity.MainColor));
            userSetting.TextColor = new SolidColorBrush(HexColorConverter.GetColorFromHex(entity.TextColor));
            userSetting.IsLandscapeCover = entity.IsLandscapeCover;
            userSetting.Limitation = new Limitation();
            userSetting.Limitation.Code = entity.LimitCode;

            return userSetting;
        }

        public UserSettingRemoteModel MapModel(AppProperties model)
        {
            var userSetting = new UserSettingRemoteModel();
            userSetting.BackgroundColor = model.Background.Color.ToString();
            userSetting.MainColor = model.MainColor.Color.ToString();
            userSetting.TextColor = model.TextColor.Color.ToString();
            userSetting.IsLandscapeCover = model.IsLandscapeCover;
            userSetting.LimitCode = model.Limitation.Code;

            return userSetting;
        }
    }
}
