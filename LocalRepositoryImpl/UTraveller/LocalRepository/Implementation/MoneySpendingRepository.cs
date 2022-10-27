using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class MoneySpendingRepository : BaseRemotableEntityRepository<MoneySpendingEntity>, IMoneySpendingRepository
    {
        public MoneySpendingRepository(LocalDatabase database) : base(database) { }

        public IEnumerable<MoneySpendingEntity> GetMoneySpendingsForEvent(long eventId)
        {
            return from MoneySpendingEntity mse in database.MoneySpending
                   where mse.EventId == eventId && !mse.IsDeleted
                   select mse; 
        }

        public MoneySpendingEntity GetById(long id)
        {
            return GetById(database.MoneySpending, id);
        }

        public override void Insert(MoneySpendingEntity entity)
        {
            InsertEntityInTable(database.MoneySpending, entity);
        }

        public override void Delete(MoneySpendingEntity entity)
        {
            DeleteEntityFromTable(database.MoneySpending, entity);
        }

        public IEnumerable<MoneySpendingEntity> GetUnSyncMoneySpendingsOfEvent(long eventId)
        {
            return from MoneySpendingEntity mse in database.MoneySpending
                   where mse.EventId == eventId && (mse.IsSync == false || mse.IsDeleted == true)
                   select mse;
        }


        public void DeleteFromEvent(EventEntity eventEntity)
        {
            database.MoneySpending.DeleteAllOnSubmit(from MoneySpendingEntity mse in database.MoneySpending
                                                where mse.EventId == eventEntity.Id
                                                select mse);
        }


        public int GetMoneySpendingsQuantity(long eventId)
        {
            return (from MoneySpendingEntity mse in database.MoneySpending
                    where mse.Event.Id == eventId && !mse.IsDeleted
                    select mse).Count();
        }


        public override IEnumerable<MoneySpendingEntity> GetAllIncludedMarkedAsDeleted(long eventId)
        {
            return from MoneySpendingEntity mse in database.MoneySpending
                   where mse.EventId == eventId
                   select mse;
        }
    }
}
