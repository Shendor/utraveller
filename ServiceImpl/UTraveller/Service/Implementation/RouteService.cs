using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpKml.Dom;
using UTravellerModel.UTraveller.Model;
using RepositoryApi.UTraveller.Repository.Api;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Implementation.Internal;
using Newtonsoft.Json;
using System.Device.Location;
using System.Diagnostics;
using ServiceApi.UTraveller.Service.Exceptions;

namespace UTraveller.Service.Implementation
{
    public class RouteService : BaseCacheableEntityService, IRouteService
    {
        private IRouteRepository routeRepository;
        private IRoutePushpinRepository routePushpinRepository;
        private IModelMapper<Route, RouteEntity> routeMapper;
        private IAppPropertiesService appPropertiesService;

        public RouteService(IRouteRepository routeRepository, IRoutePushpinRepository routePushpinRepository,
            IModelMapper<Route, RouteEntity> mapper, IAppPropertiesService appPropertiesService)
        {
            this.routeRepository = routeRepository;
            this.routePushpinRepository = routePushpinRepository;
            this.routeMapper = mapper;
            this.appPropertiesService = appPropertiesService;
        }


        public IEnumerable<Route> GetRoutes(Event e)
        {
            return GetRoutesLocally(e);
        }


        public bool InitializeRouteData(Route route)
        {
            var entity = routeRepository.GetById(route.Id);
            if (entity != null)
            {
                route.Coordinates = JsonConvert.DeserializeObject<List<RouteCoordinates>>(entity.Coordinates);
                route.Polygons = JsonConvert.DeserializeObject<List<RoutePolygon>>(entity.Polygons);
                route.Pushpins = JsonConvert.DeserializeObject<List<RoutePushpin>>(entity.Pushpins);

                return true;
            }

            return false;
        }


        public void AddRoute(Route route, Event e)
        {
            var routesQuantity = GetRoutesQuantity(e.Id);
            if (!IsLimitExceeded(e, routesQuantity + 1))
            {
                AddRouteLocally(route, e);
            }
        }


        public void DeleteRoute(Route route, Event e)
        {
            var routeEntity = routeRepository.GetById(route.Id);
            if (routeEntity != null)
            {
                routeRepository.Delete(routeEntity);
            }
        }


        private void AddRouteLocally(Route route, Event e)
        {
            var routeEntity = routeMapper.MapModel(route);
            routeEntity.EventId = e.Id;
            routeRepository.Insert(routeEntity);
            route.Id = routeEntity.Id;
        }


        private ICollection<Route> GetRoutesLocally(Event e)
        {
            var routes = new List<Route>();
            foreach (var routeEntity in routeRepository.GetRoutesOfEvent(e.Id))
            {
                routes.Add(routeMapper.MapEntity(routeEntity));
            }
            return routes;
        }


        public int GetRoutesQuantity(long eventId)
        {
            return routeRepository.GetRoutesQuantity(eventId);
        }


        private bool IsLimitExceeded(Event e, int routesQuantity)
        {
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (properties.Limitation.RoutesLimit < routesQuantity)
            {
                throw new LimitExceedException(properties.Limitation.RoutesLimit, "Route");
            }
            else
            {
                return false;
            }
        }


        public Type GetEntityType()
        {
            return typeof(Route);
        }
    }
}
