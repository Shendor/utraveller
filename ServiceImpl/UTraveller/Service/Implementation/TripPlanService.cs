using RepositoryApi.UTraveller.Repository.Api;
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
    public class TripPlanService : BaseCacheableEntityService, ITripPlanService
    {
        private ITripPlanRepository tripPlanRepository;
        private IModelMapper<TripPlan, TripPlanEntity> tripPlanMapper;

        public TripPlanService(ITripPlanRepository tripPlanRepository, IModelMapper<TripPlan, TripPlanEntity> tripPlanMapper)
        {
            this.tripPlanRepository = tripPlanRepository;
            this.tripPlanMapper = tripPlanMapper;
        }


        public TripPlan GetTripPlan(Event e)
        {
            return GetTripPlanLocally(e);
        }


        public void AddTripPlan(TripPlan tripPlan, Event e)
        {
            var tripPlanEntity = tripPlanRepository.GetTripPlanOfEvent(e.Id);
            if (tripPlanEntity == null)
            {
                AddTripPlanLocally(tripPlan, e);
            }
        }


        public void AddPlanItem(TripPlan tripPlan, BasePlanItem planItem, Event e)
        {
            if (planItem is TransportPlanItem)
            {
                tripPlan.FlightPlanItems.Add((TransportPlanItem)planItem);
            }
            else if (planItem is RentPlanItem)
            {
                tripPlan.RentPlanItems.Add((RentPlanItem)planItem);
            }
            else
            {
                tripPlan.PlanItems.Add((PlanItem)planItem);
            }
            UpdateTripPlan(tripPlan, e);
        }

        
        public void UpdatePlanItem(TripPlan tripPlan, BasePlanItem oldPlanItem, BasePlanItem planItem, Event e)
        {
            if (planItem is TransportPlanItem)
            {
                tripPlan.FlightPlanItems.Remove((TransportPlanItem)oldPlanItem);
                tripPlan.FlightPlanItems.Add((TransportPlanItem)planItem);
            }
            else if (planItem is RentPlanItem)
            {
                tripPlan.RentPlanItems.Remove((RentPlanItem)oldPlanItem);
                tripPlan.RentPlanItems.Add((RentPlanItem)planItem);
            }
            else
            {
                tripPlan.PlanItems.Remove((PlanItem)oldPlanItem);
                tripPlan.PlanItems.Add((PlanItem)planItem);
            }
            UpdateTripPlan(tripPlan, e);
        }


        public void DeletePlanItem(TripPlan tripPlan, BasePlanItem planItem, Event e)
        {
            if (planItem is TransportPlanItem)
            {
                tripPlan.FlightPlanItems.Remove((TransportPlanItem)planItem);
            }
            else if (planItem is RentPlanItem)
            {
                tripPlan.RentPlanItems.Remove((RentPlanItem)planItem);
            }
            else
            {
                tripPlan.PlanItems.Remove((PlanItem)planItem);
            }
            UpdateTripPlan(tripPlan, e);
        }


        public void UpdateTripPlan(TripPlan tripPlan, Event e)
        {
            tripPlan.IsSync = false;
            UpdateTripPlanLocally(tripPlan, e);
        }


        public async void DeleteTripPlan(TripPlan tripPlan, Event e)
        {
            var entity = tripPlanRepository.GetById(tripPlan.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                tripPlanRepository.Delete(entity);
            }
        }


        private void AddTripPlanLocally(TripPlan tripPlan, Event e)
        {
            var tripPlaneEntity = tripPlanMapper.MapModel(tripPlan);
            tripPlaneEntity.EventId = e.Id;

            tripPlanRepository.Insert(tripPlaneEntity);
            tripPlan.Id = tripPlaneEntity.Id;
        }


        private bool UpdateTripPlanLocally(TripPlan tripPlan, Event e)
        {
            var entity = tripPlanRepository.GetById(tripPlan.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                var updatedEntity = tripPlanMapper.MapModel(tripPlan);
                entity.PlanItems = updatedEntity.PlanItems;
                entity.FlightPlanItems = updatedEntity.FlightPlanItems;
                entity.RentPlanItems = updatedEntity.RentPlanItems;
                entity.IsSync = updatedEntity.IsSync;

                tripPlanRepository.Update(entity);
                return true;
            }
            return false;
        }


        private TripPlan GetTripPlanLocally(Event e)
        {
            var tripPlanEntity = tripPlanRepository.GetTripPlanOfEvent(e.Id);
            return tripPlanMapper.MapEntity(tripPlanEntity);
        }


        public Type GetEntityType()
        {
            return typeof(TripPlan);
        }

    }
}
