using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTraveller.Common.Converter;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class RoutePushpinMapper : IModelMapper<RoutePushpin, RoutePushpinEntity>
    {
        public RoutePushpin MapEntity(RoutePushpinEntity entity)
        {
            var pushpin = new RoutePushpin();
            pushpin.Id = entity.Id;
            pushpin.Description = entity.Description;
            pushpin.Coordinate = new System.Device.Location.GeoCoordinate(entity.Latitude, entity.Longitude);
            pushpin.ThumbnailContent = entity.Thumbnail;

            byte alpha = entity.R == 0 && entity.G == 0 && entity.B == 0 ? (byte)0 : (byte)255;
            pushpin.Color = Color.FromArgb(alpha, entity.R, entity.G, entity.B);
            
            return pushpin;
        }

        public RoutePushpinEntity MapModel(RoutePushpin model)
        {
            var entity = new RoutePushpinEntity();
            entity.Id = model.Id;
            entity.Description = model.Description;
            entity.Latitude = model.Coordinate.Latitude;
            entity.Longitude = model.Coordinate.Longitude;
            entity.Thumbnail = model.ThumbnailContent;
            entity.R = model.Color.R;
            entity.G = model.Color.G;
            entity.B = model.Color.B;

            return entity;
        }
    }
}
