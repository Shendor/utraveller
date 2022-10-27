using RepositoryApi.UTraveller.Repository.Api;
using ServiceImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation.Remote
{
    public class RouteRemoteService : BaseRemoteService, IRouteRemoteService
    {
        private IRouteRepository routeRepository;
        private IModelMapper<Route, RouteRemoteModel> routeRemoteMapper;
        private IWebService webService;
        private IUserService userService;

        public RouteRemoteService(IRouteRepository routeRepository,
            IModelMapper<Route, RouteRemoteModel> routeRemoteMapper,
            IWebService webService, IUserService userService)
        {
            this.routeRepository = routeRepository;
            this.webService = webService;
            this.routeRemoteMapper = routeRemoteMapper;
            this.userService = userService;
        }


        public async Task<bool> AddRoute(Route route, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Add_Route, e.RemoteId, currentUser.RESTAccessToken);
                var routeRemoteModel = routeRemoteMapper.MapModel(route);
                var result = await webService.PostAsync<RouteRemoteModel, RemoteModel<long?>>(url, routeRemoteModel);

                if (hasResponseWithoutErrors(result))
                {
                    var entity = routeRepository.GetById(route.Id);
                    route.RemoteId = entity.RemoteId = result.ResponseObject.Value;
                    route.IsSync = entity.IsSync = true;
                    route.ChangeDate = entity.ChangeDate = result.ChangeDate;
                    routeRepository.Update(entity);
                    return true;
                }
            }
            return false;
        }


        public async Task<bool> DeleteRoute(Route route, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && route.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Delete_Route, e.RemoteId, route.RemoteId, currentUser.RESTAccessToken);
                var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                return hasResponseWithoutErrors(result) && result.ResponseObject.Value;
            }
            return false;
        }


        public async Task<IEnumerable<Route>> GetRoutes(Event e)
        {
            List<Route> routes = null;
            var user = userService.GetCurrentUser();
            if (user.Id == e.UserId && user.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_Routes, user.RemoteId, e.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<IList<RouteRemoteModel>>>(url);

                if (result != null && result.ResponseObject != null)
                {
                    routes = new List<Route>(); 
                    foreach (var item in result.ResponseObject)
                    {
                        routes.Add(routeRemoteMapper.MapEntity(item));
                    }
                }
            }
            return routes;
        }
    }
}
