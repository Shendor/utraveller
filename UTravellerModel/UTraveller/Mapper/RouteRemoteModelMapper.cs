using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTravellerModel.UTraveller.Converter;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class RouteRemoteModelMapper : IModelMapper<Route, RouteRemoteModel>
    {

        public Route MapEntity(RouteRemoteModel entity)
        {
            var route = new Route();
            route.RemoteId = entity.Id;
            route.Color = HexColorConverter.GetColorFromHex(entity.Color);
            route.Description = entity.Description;
            route.Name = entity.Name;
            route.ChangeDate = entity.ChangeDate;
            route.Polygons = JsonConvert.DeserializeObject<IList<RoutePolygon>>(entity.PolygonsJson);
            route.Pushpins = JsonConvert.DeserializeObject<IList<RoutePushpin>>(entity.PushpinsJson);
            route.Coordinates = JsonConvert.DeserializeObject<IList<RouteCoordinates>>(entity.CoordinatesJson);

            return route;
        }


        public RouteRemoteModel MapModel(Route model)
        {
            var routeRemoteModel = new RouteRemoteModel();
            routeRemoteModel.Id = model.RemoteId;
            routeRemoteModel.Color = model.Color.ToString();
            routeRemoteModel.Description = model.Description;
            routeRemoteModel.Name = model.Name;
            routeRemoteModel.ChangeDate = model.ChangeDate;
            routeRemoteModel.PolygonsJson = JsonConvert.SerializeObject(model.Polygons);
            routeRemoteModel.PushpinsJson = JsonConvert.SerializeObject(model.Pushpins, new JsonSerializerSettings()
            {
                ContractResolver = new WritablePropertiesOnlyResolver()
            });
            routeRemoteModel.CoordinatesJson = JsonConvert.SerializeObject(model.Coordinates);

            return routeRemoteModel;
        }
    }
}
