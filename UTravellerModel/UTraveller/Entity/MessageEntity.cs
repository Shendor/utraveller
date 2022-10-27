using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Entity
{
    [Table]
    public class MessageEntity : IRemotableEntity<long>
    {
        private long id;
        private long remoteId;
        private string text;
        private DateTime date;
        private double latitude;
        private double longitude;
        private bool isSync;
        internal EntityRef<EventEntity> eventEntity;


        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
            CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get { return id; }
            set { id = value; }
        }


        [Column]
        public long RemoteId
        {
            get { return remoteId; }
            set { remoteId = value; }
        }


        [Column]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }


        [Column]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }


        [Column]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }


        [Column]
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }


        [Column]
        public long EventId
        {
            get;
            set;
        }


        [Column]
        public string FacebookPostId
        {
            get;
            set;
        }


        [Column]
        public bool IsSync
        {
            get { return isSync; }
            set { isSync = value; }
        }


        [Column]
        public bool IsDeleted
        {
            get;
            set;
        }


        [Column]
        public DateTime? ChangeDate
        {
            get;
            set;
        }


        [Association(Storage = "eventEntity", ThisKey = "EventId", OtherKey = "Id", IsForeignKey = true, DeleteOnNull = true)]
        public EventEntity Event
        {
            get { return eventEntity.Entity; }
            set
            {
                eventEntity.Entity = value;
                if (value != null)
                {
                    EventId = value.Id;
                }
            }
        }
    }
}
