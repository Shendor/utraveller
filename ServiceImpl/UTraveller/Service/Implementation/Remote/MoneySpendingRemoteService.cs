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
    public class MoneySpendingRemoteService : BaseRemoteService, IMoneySpendingRemoteService
    {
        private IMoneySpendingRepository moneySpendingRepository;
        private IModelMapper<MoneySpending, MoneySpendingRemoteModel> moneySpendingRemoteMapper;
        private IWebService webService;
        private IUserService userService;

        public MoneySpendingRemoteService(IMoneySpendingRepository moneySpendingRepository,
            IModelMapper<MoneySpending, MoneySpendingRemoteModel> moneySpendingRemoteMapper,
            IWebService webService, IUserService userService)
        {
            this.moneySpendingRepository = moneySpendingRepository;
            this.webService = webService;
            this.moneySpendingRemoteMapper = moneySpendingRemoteMapper;
            this.userService = userService;
        }


        public async Task<bool> AddMoneySpendingForEvent(MoneySpending moneySpending, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Add_MoneySpending, e.RemoteId, currentUser.RESTAccessToken);
                var moneySpendingRemoteModel = moneySpendingRemoteMapper.MapModel(moneySpending);
                var result = await webService.PostAsync<MoneySpendingRemoteModel, RemoteModel<long?>>(url, moneySpendingRemoteModel);

                if (hasResponseWithoutErrors(result))
                {
                    var entity = moneySpendingRepository.GetById(moneySpending.Id);
                    moneySpending.RemoteId = entity.RemoteId = result.ResponseObject.Value;
                    moneySpending.IsSync = entity.IsSync = true;
                    moneySpendingRepository.Update(entity);
                    return true;
                }
            }
            return false;
        }

        
        public async Task<bool> DeleteMoneySpending(MoneySpending moneySpending, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && moneySpending.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Delete_MoneySpending, e.RemoteId, moneySpending.RemoteId, currentUser.RESTAccessToken);
                var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                return hasResponseWithoutErrors(result) && result.ResponseObject.Value;
            }
            return false;
        }

        public async Task<IEnumerable<MoneySpending>> GetMoneySpendingsForEvent(Event e)
        {
            List<MoneySpending> moneySpendigns = null;
            var user = userService.GetCurrentUser();
            if (user.Id == e.UserId && user.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_MoneySpendings, user.RemoteId, e.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<IList<MoneySpendingRemoteModel>>>(url);

                if (hasResponseWithoutErrors(result))
                {
                    moneySpendigns = new List<MoneySpending>(); 
                    foreach (var item in result.ResponseObject)
                    {
                        moneySpendigns.Add(moneySpendingRemoteMapper.MapEntity(item));
                    }
                }
            }
            return moneySpendigns;
        }

    }
}
