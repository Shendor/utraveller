using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class RouteRepository : BaseRemotableEntityRepository<RouteEntity>, IRouteRepository
    {
        public RouteRepository(LocalDatabase database) : base(database) { }

        public IEnumerable<RouteEntity> GetRoutesOfEvent(long eventId)
        {
            return from RouteEntity r in database.Routes
                   where r.EventId == eventId && !r.IsDeleted
                   select r;
        }

        public override void Insert(RouteEntity entity)
        {
            InsertEntityInTable(database.Routes, entity);
        }

        public override void Delete(RouteEntity entity)
        {
            DeleteEntityFromTable(database.Routes, entity);
        }

        public RouteEntity GetById(long id)
        {
            return GetById(database.Routes, id);
        }

        public IEnumerable<RouteEntity> GetUnSyncRoutesOfEvent(long eventId)
        {
            return from RouteEntity route in database.Routes
                   where route.EventId == eventId && (route.IsSync == false || route.IsDeleted == true)
                   select route;
        }


        public void DeleteFromEvent(EventEntity eventEntity)
        {
            database.Routes.DeleteAllOnSubmit(from RouteEntity route in database.Routes
                                                where route.EventId == eventEntity.Id
                                                select route);
        }


        public int GetRoutesQuantity(long eventId)
        {
            return (from RouteEntity route in database.Routes
                    where route.Event.Id == eventId && !route.IsDeleted
                    select route).Count();
        }


        public override IEnumerable<RouteEntity> GetAllIncludedMarkedAsDeleted(long eventId)
        {
            return from RouteEntity route in database.Routes
                   where route.EventId == eventId
                   select route;
        }
    }
}
