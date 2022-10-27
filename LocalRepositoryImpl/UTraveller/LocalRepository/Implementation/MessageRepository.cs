using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class MessageRepository : BaseRemotableEntityRepository<MessageEntity>, IMessageRepository
    {
        public MessageRepository(LocalDatabase database)
            : base(database)
        {
        }


        public IEnumerable<MessageEntity> GetMessagesOfEvent(long eventId)
        {
            return from m in database.Messages
                   where m.EventId == eventId && !m.IsDeleted
                   select m;
        }


        public MessageEntity GetById(long id)
        {
            return GetById(database.Messages, id);
        }
        

        public override void Insert(MessageEntity entity)
        {
            InsertEntityInTable(database.Messages, entity);
        }


        public override void Delete(MessageEntity entity)
        {
            DeleteEntityFromTable(database.Messages, entity);
        }


        public IEnumerable<MessageEntity> GetUnSyncMessagesOfEvent(long eventId)
        {
            return from MessageEntity message in database.Messages
                   where message.EventId == eventId && (message.IsSync == false || message.IsDeleted == true)
                   select message;
        }


        public void DeleteFromEvent(EventEntity eventEntity)
        {
            database.Messages.DeleteAllOnSubmit(from MessageEntity message in database.Messages
                                              where message.EventId == eventEntity.Id
                                              select message);
        }

        public int GetMessagesQuantity(long eventId)
        {
            return (from MessageEntity message in database.Messages
                    where message.Event.Id == eventId && !message.IsDeleted
                    select message).Count();
        }


        public override IEnumerable<MessageEntity> GetAllIncludedMarkedAsDeleted(long eventId)
        {
            return from MessageEntity message in database.Messages
                   where message.EventId == eventId
                   select message;
        }
    }
}
