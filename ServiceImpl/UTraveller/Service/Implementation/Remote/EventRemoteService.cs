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
    public class EventRemoteService : BaseRemoteService, IEventRemoteService
    {
        private IEventRepository eventRepository;
        private IModelMapper<Event, EventRemoteModel> eventRemoteMapper;
        private IWebService webService;
        private IUserService userService;
        private INetworkConnectionCheckService networkCheckService;


        public EventRemoteService(IEventRepository eventRepository, IModelMapper<Event, EventRemoteModel> eventRemoteMapper,
            IWebService webService, IUserService userService, INetworkConnectionCheckService networkCheckService)
        {
            this.eventRepository = eventRepository;
            this.webService = webService;
            this.eventRemoteMapper = eventRemoteMapper;
            this.userService = userService;
            this.networkCheckService = networkCheckService;
        }


        public async Task<IEnumerable<Event>> GetEvents(User user)
        {
            List<Event> events = null;
            if (networkCheckService.HasConnection && user.RESTAccessToken != null)
            {
                var url = string.Format(ServiceResources.REST_Get_Events, user.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<IList<EventRemoteModel>>>(url);

                if (result != null && result.ResponseObject != null)
                {
                    events = new List<Event>();
                    foreach (var item in result.ResponseObject)
                    {
                        events.Add(eventRemoteMapper.MapEntity(item));
                    }
                }
            }
            return events;
        }


        public async Task<bool> AddEvent(Event e, User user)
        {
            if (networkCheckService.HasConnection)
            {
                if (user.RESTAccessToken != null)
                {
                    var url = string.Format(ServiceResources.REST_Add_Event, user.RESTAccessToken);
                    var eventRemoteModel = eventRemoteMapper.MapModel(e);
                    var result = await webService.PostAsync<EventRemoteModel, RemoteModel<long?>>(url, eventRemoteModel);

                    if (hasResponseWithoutErrors(result))
                    {
                        var eventEntity = eventRepository.GetById(e.Id);
                        e.RemoteId = eventEntity.RemoteId = result.ResponseObject.Value;
                        e.ChangeDate = eventEntity.ChangeDate = result.ChangeDate;
                        e.IsSync = eventEntity.IsSync = true;
                        eventRepository.Update(eventEntity);
                        return true;
                    }
                }
            }
            return false;
        }


        public async Task<bool> DeleteEvent(Event e)
        {
            if (networkCheckService.HasConnection)
            {
                var currentUser = userService.GetCurrentUser();
                if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
                {
                    var url = string.Format(ServiceResources.REST_Delete_Event, e.RemoteId, currentUser.RESTAccessToken);
                    var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                    return hasResponseWithoutErrors(result);
                }
            }
            return false;
        }


        public async Task<bool> UpdateEvent(Event e)
        {
            if (networkCheckService.HasConnection)
            {
                var currentUser = userService.GetCurrentUser();
                if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
                {
                    var url = string.Format(ServiceResources.REST_Update_Event, currentUser.RESTAccessToken);
                    var eventRemoteModel = eventRemoteMapper.MapModel(e);
                    var result = await webService.PostAsync<EventRemoteModel, RemoteModel<bool>>(url, eventRemoteModel);
                    
                    if (hasResponseWithoutErrors(result))
                    {
                        var eventEntity = eventRepository.GetById(e.Id);
                        e.IsSync = eventEntity.IsSync = true;
                        e.ChangeDate = eventEntity.ChangeDate = result.ChangeDate;
                        eventRepository.Update(eventEntity);
                        
                        return result.ResponseObject;
                    }
                }
            }
            return true;
        }


        public async Task<Event> GetEvent(User user, long id)
        {
            if (networkCheckService.HasConnection && user.RESTAccessToken != null && id > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_Event, id, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<EventRemoteModel>>(url);

                if (hasResponseWithoutErrors(result))
                {
                    return eventRemoteMapper.MapEntity(result.ResponseObject);
                }
            }
            return null;
        }
    }
}
