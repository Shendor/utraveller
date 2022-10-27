using LocalRepositoryImpl.UTraveller.LocalRepository.Implementation;
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
    public class TripPlanRemoteService : BaseRemoteService, ITripPlanRemoteService
    {
        private ITripPlanRepository tripPlanRepository;
        private IModelMapper<TripPlan, TripPlanRemoteModel> tripPlanRemoteMapper;
        private IWebService webService;
        private IUserService userService;

        public TripPlanRemoteService(ITripPlanRepository tripPlanRepository,
            IModelMapper<TripPlan, TripPlanRemoteModel> tripPlanRemoteMapper, IWebService webService,
            IUserService userService)
        {
            this.tripPlanRepository = tripPlanRepository;
            this.tripPlanRemoteMapper = tripPlanRemoteMapper;
            this.webService = webService;
            this.userService = userService;
        }


        public async Task<TripPlan> GetTripPlanOfEvent(Event e)
        {
            TripPlan tripPlan = null;
            var user = userService.GetCurrentUser();
            if (user.Id == e.UserId && user.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_TripPlan, e.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<TripPlanRemoteModel>>(url);

                if (hasResponseWithoutErrors(result))
                {
                    tripPlan = tripPlanRemoteMapper.MapEntity(result.ResponseObject);
                }
            }
            return tripPlan;
        }

        public async Task<bool> AddTripPlan(TripPlan tripPlan, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Add_TripPlan, e.RemoteId, currentUser.RESTAccessToken);
                var tripPlanRemoteModel = tripPlanRemoteMapper.MapModel(tripPlan);
                var result = await webService.PostAsync<TripPlanRemoteModel, RemoteModel<long?>>(url, tripPlanRemoteModel);

                if (hasResponseWithoutErrors(result))
                {
                    var tripPlanEntity = tripPlanRepository.GetById(tripPlan.Id);
                    tripPlan.RemoteId = tripPlanEntity.RemoteId = result.ResponseObject.Value;
                    tripPlanEntity.IsSync = tripPlan.IsSync = true;
                    tripPlanEntity.ChangeDate = tripPlan.ChangeDate = result.ChangeDate;
                    tripPlanRepository.Update(tripPlanEntity);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UpdateTripPlan(TripPlan tripPlan, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                if (tripPlan.RemoteId == 0)
                {
                    await AddTripPlan(tripPlan, e);
                }
                var url = string.Format(ServiceResources.REST_Update_TripPlan, e.RemoteId, currentUser.RESTAccessToken);
                var tripPlanRemoteModel = tripPlanRemoteMapper.MapModel(tripPlan);
                var result = await webService.PostAsync<TripPlanRemoteModel, RemoteModel<bool?>>(url, tripPlanRemoteModel);

                if (hasResponseWithoutErrors(result) && result.ResponseObject.Value)
                {
                    var tripPlanEntity = tripPlanRepository.GetById(tripPlan.Id);
                    tripPlanEntity.IsSync = tripPlan.IsSync = true;
                    tripPlanEntity.ChangeDate = tripPlan.ChangeDate = result.ChangeDate;
                    tripPlanRepository.Update(tripPlanEntity);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteTripPlan(Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Delete_TripPlan, e.RemoteId, currentUser.RESTAccessToken);
                var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                return hasResponseWithoutErrors(result) && result.ResponseObject.Value;
            }
            return false;
        }
    }
}
