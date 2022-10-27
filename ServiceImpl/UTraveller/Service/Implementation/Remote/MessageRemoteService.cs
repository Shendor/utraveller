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
    public class MessageRemoteService : BaseRemoteService, IMessageRemoteService
    {
        private IMessageRepository messageRepository;
        private IModelMapper<Message, MessageRemoteModel> messageRemoteMapper;
        private IWebService webService;
        private IUserService userService;

        public MessageRemoteService(IMessageRepository messageRepository, IModelMapper<Message, MessageRemoteModel> messageRemoteMapper,
            IWebService webService, IUserService userService)
        {
            this.messageRepository = messageRepository;
            this.webService = webService;
            this.messageRemoteMapper = messageRemoteMapper;
            this.userService = userService;
        }


        public async Task<IEnumerable<Message>> GetMessagesOfEvent(Event e)
        {
            List<Message> messages = null;
            var user = userService.GetCurrentUser();
            if (user.Id == e.UserId && user.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_Messages, user.RemoteId, e.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<IList<MessageRemoteModel>>>(url);

                if (result != null && result.ResponseObject != null)
                {
                    messages = new List<Message>();
                    foreach (var item in result.ResponseObject)
                    {
                        messages.Add(messageRemoteMapper.MapEntity(item));
                    }
                }
            }
            return messages;
        }


        public async Task<bool> AddMessageToEvent(Message message, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Add_Message, e.RemoteId, currentUser.RESTAccessToken);
                var messageRemoteModel = messageRemoteMapper.MapModel(message);
                var result = await webService.PostAsync<MessageRemoteModel, RemoteModel<long?>>(url, messageRemoteModel);

                if (hasResponseWithoutErrors(result))
                {
                    var messageEntity = messageRepository.GetById(message.Id);
                    message.RemoteId = messageEntity.RemoteId = result.ResponseObject.Value;
                    messageEntity.IsSync = message.IsSync = true;
                    messageEntity.ChangeDate = message.ChangeDate = result.ChangeDate;
                    messageRepository.Update(messageEntity);
                    return true;
                }
            }
            return false;
        }


        public async Task<bool> UpdateMessage(Message message, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && message.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Update_Message, e.RemoteId, message.RemoteId, currentUser.RESTAccessToken);
                var messageRemoteModel = messageRemoteMapper.MapModel(message);
                var result = await webService.PostAsync<MessageRemoteModel, RemoteModel<bool?>>(url, messageRemoteModel);

                if (hasResponseWithoutErrors(result) && result.ResponseObject.Value)
                {
                    var messageEntity = messageRepository.GetById(message.Id);
                    messageEntity.ChangeDate = message.ChangeDate = result.ChangeDate;
                    messageEntity.IsSync = message.IsSync = true;
                    messageRepository.Update(messageEntity);
                    return true;
                }
            }

            return false;
        }


        public async Task<bool> DeleteMessage(Message message, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && message.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Delete_Message, e.RemoteId, message.RemoteId, currentUser.RESTAccessToken);
                var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                return hasResponseWithoutErrors(result) && result.ResponseObject.Value;
            }
            return false;
        }
    }
}
