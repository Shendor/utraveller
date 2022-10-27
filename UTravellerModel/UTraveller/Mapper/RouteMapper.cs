using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTraveller.Common.Converter;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class RouteMapper : IModelMapper<Route, RouteEntity>
    {
        public Route MapEntity(RouteEntity entity)
        {
            var route = new Route();
            route.Id = entity.Id;
            route.RemoteId = entity.RemoteId;
            route.IsSync = entity.IsSync;
            route.Name = entity.Name;
            route.Description = entity.Description;
            route.Color = Color.FromArgb(255, entity.R, entity.G, entity.B);
            route.ChangeDate = entity.ChangeDate;
            route.IsDeleted = entity.IsDeleted;

            return route;
        }


        public RouteEntity MapModel(Route model)
        {
            var route = new RouteEntity();
            route.Id = model.Id;
            route.RemoteId = model.RemoteId;
            route.IsSync = model.IsSync;
            route.IsDeleted = model.IsDeleted;
            route.Name = model.Name;
            route.Description = model.Description;
            route.R = model.Color.R;
            route.G = model.Color.G;
            route.B = model.Color.B;
            route.ChangeDate = model.ChangeDate;
            route.Coordinates = JsonConvert.SerializeObject(model.Coordinates); 
            route.Polygons = JsonConvert.SerializeObject(model.Polygons);
            route.Pushpins = JsonConvert.SerializeObject(model.Pushpins, new JsonSerializerSettings()
            {
                ContractResolver = new WritablePropertiesOnlyResolver()
            });

            return route;
        }
    }
}
