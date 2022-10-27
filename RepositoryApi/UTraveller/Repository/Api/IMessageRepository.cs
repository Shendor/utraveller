using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IMessageRepository : IBaseRepository<MessageEntity, long>, IDeleteFromEventRepository,
        IMarkDeleteRepository<MessageEntity>
    {
        IEnumerable<MessageEntity> GetMessagesOfEvent(long eventId);

        IEnumerable<MessageEntity> GetUnSyncMessagesOfEvent(long eventId);

        int GetMessagesQuantity(long eventId);
    }
}
