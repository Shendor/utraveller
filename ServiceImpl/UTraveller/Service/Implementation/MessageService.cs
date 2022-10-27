using RepositoryApi.UTraveller.Repository.Api;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Implementation.Internal;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class MessageService : BaseCacheableEntityService, IMessageService
    {
        private IMessageRepository messageRepository;
        private IModelMapper<Message, MessageEntity> messageMapper;
        private IGeoCoordinateService geoLocationService;
        private ITaskExecutionManager taskExecutionManager;
        private IAppPropertiesService appPropertiesService;

        public MessageService(IMessageRepository messageRepository, IModelMapper<Message, MessageEntity> messageMapper,
             IGeoCoordinateService geoLocationService,
            ITaskExecutionManager taskExecutionManager, IAppPropertiesService appPropertiesService)
        {
            this.messageRepository = messageRepository;
            this.messageMapper = messageMapper;
            this.geoLocationService = geoLocationService;
            this.taskExecutionManager = taskExecutionManager;
            this.appPropertiesService = appPropertiesService;
        }


        public IEnumerable<Message> GetMessagesOfEvent(Event e)
        {
            return GetMessagesLocally(e);
        }


        public async Task AddMessageToEvent(Message message, Event e)
        {
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (!IsLimitExceeded(e, GetMessagesQuantity(e.Id) + 1))
            {
                AddMessageLocally(message, e);
                if (properties == null || properties.IsAllowGeoPosition)
                {
                    var position = await geoLocationService.ApplyCurrentLocation(50);
                    if (position != null && message.Id > 0)
                    {
                        message.Coordinate = position;
                        UpdateMessageLocally(message, e);
                    }
                }
            }
        }


        public void UpdateMessage(Message message, Event e)
        {
            UpdateMessageLocally(message, e);
        }


        public void UpdateMessages(List<Message> messages, Event e)
        {
            foreach (var message in messages)
            {
                UpdateMessage(message, e);
            }
        }


        public void DeleteMessage(Message message, Event e)
        {
            var entity = messageRepository.GetById(message.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                messageRepository.Delete(entity);
            }
        }


        private void AddMessageLocally(Message message, Event e)
        {
            var messageEntity = messageMapper.MapModel(message);
            messageEntity.EventId = e.Id;

            messageRepository.Insert(messageEntity);
            message.Id = messageEntity.Id;
        }


        private bool UpdateMessageLocally(Message message, Event e)
        {
            var entity = messageRepository.GetById(message.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                entity.Text = message.Text;
                entity.ChangeDate = message.ChangeDate;
                entity.FacebookPostId = message.FacebookPostId;
                entity.IsSync = message.IsSync;
                if (message.Coordinate != null)
                {
                    entity.Latitude = message.Coordinate.Latitude;
                    entity.Longitude = message.Coordinate.Longitude;
                }
                else
                {
                    entity.Latitude = 0;
                    entity.Longitude = 0;
                }
                messageRepository.Update(entity);
                return true;
            }
            return false;
        }


        private ICollection<Message> GetMessagesLocally(Event e)
        {
            var messages = new List<Message>();
            foreach (var messageEntity in messageRepository.GetMessagesOfEvent(e.Id))
            {
                messages.Add(messageMapper.MapEntity(messageEntity));
            }
            return messages;
        }


        public int GetMessagesQuantity(long eventId)
        {
            return messageRepository.GetMessagesQuantity(eventId);
        }


        private bool IsLimitExceeded(Event e, int messagesQuantity)
        {
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (properties.Limitation.MessagesLimit < messagesQuantity)
            {
                throw new LimitExceedException(properties.Limitation.MessagesLimit, "Message");
            }
            else
            {
                return false;
            }
        }


        public Type GetEntityType()
        {
            return typeof(Message);
        }
    }
}
