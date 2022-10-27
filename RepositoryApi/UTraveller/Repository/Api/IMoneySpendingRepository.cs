using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IMoneySpendingRepository : IBaseRepository<MoneySpendingEntity, long>, IDeleteFromEventRepository,
         IMarkDeleteRepository<MoneySpendingEntity>
    {
        IEnumerable<MoneySpendingEntity> GetMoneySpendingsForEvent(long eventId);

        IEnumerable<MoneySpendingEntity> GetUnSyncMoneySpendingsOfEvent(long eventId);

        int GetMoneySpendingsQuantity(long eventId);
    }
}
