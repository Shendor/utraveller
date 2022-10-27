using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class AppPropertiesMapper : IModelMapper<AppProperties, AppPropertiesEntity>
    {
        private static readonly IDictionary<string, string> limits = new Dictionary<string, string>();

        static AppPropertiesMapper()
        {
            limits.Add(Limitation.CODE1, "false;10;25;25;50;1;false;");
            limits.Add(Limitation.CODE2, "true;20;80;80;100;25;true;");
        }

        public AppProperties MapEntity(AppPropertiesEntity entity)
        {
            var properties = new AppProperties();
            properties.Id = entity.Id;
            properties.CoverOpacity = entity.CoverOpacity;
            properties.PanelsOpacity = entity.PanelsOpacity;
            properties.IsLandscapeCover = entity.IsLandscapeCover;
            properties.IsUploadToFacebook = entity.IsUploadToFacebook;
            properties.IsAllowGeoPosition = entity.IsAllowGeoPosition;
            properties.IsConnectToServerAutomatically = entity.IsConnectToServerAutomatically;
            properties.Limitation = ToLimitation(entity.LimitationCode);
            properties.MainColor =
                new SolidColorBrush(Color.FromArgb(255, entity.MainColor[0], entity.MainColor[1], entity.MainColor[2]));
            properties.Background =
                 new SolidColorBrush(Color.FromArgb(255, entity.BackgroundColor[0], entity.BackgroundColor[1], entity.BackgroundColor[2]));
            properties.TextColor =
                 new SolidColorBrush(Color.FromArgb(255, entity.TextColor[0], entity.TextColor[1], entity.TextColor[2]));

            return properties;
        }


        public AppPropertiesEntity MapModel(AppProperties model)
        {
            var entity = new AppPropertiesEntity();
            entity.Id = entity.Id;
            entity.CoverOpacity = model.CoverOpacity;
            entity.PanelsOpacity = model.PanelsOpacity;
            entity.IsLandscapeCover = model.IsLandscapeCover;
            entity.IsUploadToFacebook = model.IsUploadToFacebook;
            entity.IsAllowGeoPosition = model.IsAllowGeoPosition;
            entity.IsConnectToServerAutomatically = model.IsConnectToServerAutomatically;
            entity.LimitationCode = model.Limitation.Code;
            entity.MainColor = new byte[] { model.MainColor.Color.R, model.MainColor.Color.G, model.MainColor.Color.B };
            entity.BackgroundColor = new byte[] { model.Background.Color.R, model.Background.Color.G, model.Background.Color.B };
            entity.TextColor = new byte[] { model.TextColor.Color.R, model.TextColor.Color.G, model.TextColor.Color.B };

            return entity;
        }


        private Limitation ToLimitation(string limitationCode)
        {
            var limitations = limits[limitationCode].Split(';');
            var limitation = new Limitation();
            limitation.Code = limitationCode;
            limitation.IsExtendedColors = bool.Parse(limitations[0]);
            limitation.TripLimit = int.Parse(limitations[1]);
            limitation.PhotosLimit = int.Parse(limitations[2]);
            limitation.MessagesLimit = int.Parse(limitations[3]);
            limitation.MoneySpendingsLimit = int.Parse(limitations[4]);
            limitation.RoutesLimit = int.Parse(limitations[5]);
            limitation.IsRouteEditorEnabled = bool.Parse(limitations[6]);

            return limitation;
        }
    }
}
