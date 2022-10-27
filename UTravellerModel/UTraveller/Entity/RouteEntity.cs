using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Entity
{
    [Table]
    public class RouteEntity : IRemotableEntity<long>
    {
        private long id;
        private long remoteId;
        private string name;
        private string description;
        private string coordinates;
        private string polygons;
        private string pushpins;
        private string lines;
        private byte rColor;
        private byte gColor;
        private byte bColor;
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
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        [Column]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }

        [Column]
        public byte R
        {
            get { return rColor; }
            set { rColor = value; }
        }

        [Column]
        public byte G
        {
            get { return gColor; }
            set { gColor = value; }
        }

        [Column]
        public byte B
        {
            get { return bColor; }
            set { bColor = value; }
        }


        [Column]
        public long EventId
        {
            get;
            set;
        }


        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string Polygons
        {
            get { return polygons; }
            set { polygons = value; }
        }

        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string Pushpins
        {
            get { return pushpins; }
            set { pushpins = value; }
        }


        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string Lines
        {
            get { return lines; }
            set { lines = value; }
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
