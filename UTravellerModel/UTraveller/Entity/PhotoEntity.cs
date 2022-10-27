using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Entity
{
    [Table]
    public class PhotoEntity : IRemotableEntity<long>
    {
        private long id;
        private long remoteId;
        private string name;
        private DateTime date;
        private string imageUrl;
        private byte[] thumbnail;
        private int width;
        private int height;
        private string description;
        private double latitude;
        private double longitude;
        private bool isSync;
        private bool isDeleted;
        private string facebookPostId;
        private string facebookPhotoId;
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
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        [Column]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        [Column]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        [Column]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }

        [Column]
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        [Column(DbType="float")]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }


        [Column(DbType = "float")]
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
        public bool IsSync
        {
            get { return isSync; }
            set { isSync = value; }
        }


        [Column]
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }


        [Column]
        public string FacebookPostId
        {
            get { return facebookPostId; }
            set { facebookPostId = value; }
        }


        [Column]
        public string FacebookPhotoId
        {
            get { return facebookPhotoId; }
            set { facebookPhotoId = value; }
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

        //public override bool Equals(object obj)
        //{
        //    if (obj == this)
        //    {
        //        return true;
        //    }
        //    if (!obj.GetType().Equals(this.GetType()))
        //    {
        //        return false;
        //    }
        //    var photoItem = (PhotoEntity)obj;
        //    return photoItem.Id.Equals(Id) && photoItem.Name.Equals(Name) && photoItem.Date.Equals(Date);
        //}

        //public override int GetHashCode()
        //{
        //    int hashCode = id.GetHashCode();
        //    hashCode += 37 * Name.GetHashCode();
        //    hashCode += 37 * Date.GetHashCode();

        //    return hashCode;
        //}
    }
}
