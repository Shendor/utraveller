using RepositoryApi.UTraveller.Repository.Api;
using ServiceApi.UTraveller.Service.Exceptions;
using ServiceImpl;
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
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation
{
    public class EventService : BaseCacheableEntityService, IEventService
    {
        private IEventRepository eventRepository;
        private IPhotoRepository photoRepository;
        private IMessageRepository messageRepository;
        private IMoneySpendingRepository moneySpendingRepository;
        private IRouteRepository routeRepository;
        private IModelMapper<Event, EventEntity> eventMapper;
        private IAppPropertiesService appPropertiesService;

        public EventService(IEventRepository eventRepository, IPhotoRepository photoRepository,
            IModelMapper<Event, EventEntity> eventMapper,
            IMoneySpendingRepository moneySpendingRepository, IMessageRepository messageRepository,
            IRouteRepository routeRepository, IAppPropertiesService appPropertiesService)
        {
            this.eventRepository = eventRepository;
            this.photoRepository = photoRepository;
            this.messageRepository = messageRepository;
            this.moneySpendingRepository = moneySpendingRepository;
            this.routeRepository = routeRepository;
            this.eventMapper = eventMapper;
            this.appPropertiesService = appPropertiesService;
        }

        public IEnumerable<Event> GetEvents(User user)
        {
            return GetEventsLocally(user);
        }


        public void AddEvent(Event e, User user)
        {
            var eventsQuantity = eventRepository.GetEventsQuantity(user.Id);
            if (!IsLimitExceeded(user, eventsQuantity + 1))
            {
                AddEventLocally(e, user);
            }
        }


        public bool DeleteEvent(Event e)
        {
            bool isDeleted = false;
            var eventEntity = eventRepository.GetById(e.Id);
            if (eventEntity != null)
            {
                DeleteEventLocally(eventEntity);
                isDeleted = true;
            }
            return isDeleted;
        }


        public void UpdateEvent(Event e)
        {
            UpdateEventLocally(e);
        }


        public void ChangeCurrentEvent(Event e)
        {
            eventRepository.ChangeCurrentEvent(e.Id, e.IsCurrent);
        }


        public Event GetCurrentEvent(User user)
        {
            var eventEntity = eventRepository.FindCurrentEvent(user.Id);
            return eventEntity != null ? eventMapper.MapEntity(eventEntity) : null;
        }


        public int GetEventsQuantity(User user)
        {
            return eventRepository.GetEventsQuantity(user.Id);
        }

        public bool IsEventNameExist(string eventName, User user)
        {
            return eventRepository.IsEventNameExist(eventName, user.Id);
        }

        private void DeleteEventLocally(EventEntity eventEntity)
        {
            photoRepository.DeleteFromEvent(eventEntity);
            messageRepository.DeleteFromEvent(eventEntity);
            moneySpendingRepository.DeleteFromEvent(eventEntity);
            routeRepository.DeleteFromEvent(eventEntity);

            eventRepository.Delete(eventEntity);
        }


        private bool UpdateEventLocally(Event e)
        {
            var eventEntity = eventRepository.GetById(e.Id);
            if (eventEntity != null)
            {
                eventEntity.Name = e.Name;
                eventEntity.StartDate = e.Date;
                eventEntity.EndDate = e.EndDate;
                eventEntity.PhotosQuantity = e.PhotosQuantity;
                eventEntity.Image = e.Image;
                eventEntity.IsSync = e.IsSync;
                eventEntity.ChangeDate = e.ChangeDate;
                eventRepository.Update(eventEntity);
                return true;
            }
            return false;
        }


        private ICollection<Event> GetEventsLocally(User user)
        {
            var events = new List<Event>();
            foreach (var eventEntity in eventRepository.GetEventsOfUser(user.Id))
            {
                events.Add(eventMapper.MapEntity(eventEntity));
            }
            return events;
        }


        private bool IsLimitExceeded(User user, int eventsQuantity)
        {
            var properties = appPropertiesService.GetPropertiesForUser(user.Id);
            if (properties.Limitation.TripLimit < eventsQuantity)
            {
                throw new LimitExceedException(properties.Limitation.TripLimit, "Trip");
            }
            else
            {
                return false;
            }
        }


        private void AddEventLocally(Event e, User user)
        {
            e.UserId = user.Id;
            var eventEntity = eventMapper.MapModel(e);
            eventRepository.Insert(eventEntity);
            e.Id = eventEntity.Id;
        }

    }
}
