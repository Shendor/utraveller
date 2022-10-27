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
    public class EventEntity : IRemotableEntity<long>
    {
        private long id;
        private long remoteId;
        private string name;
        private DateTime startDate;
        private DateTime? endDate;
        private int photosQuantity;
        private byte[] image;
        private bool isCurrent;
        private bool isSync;
        private long userId;
        private bool isDeleted;
        private EntitySet<PhotoEntity> photos;
        private EntitySet<RouteEntity> routes;
        private EntitySet<MoneySpendingEntity> moneySpendings;

        public EventEntity()
        {
            photosQuantity = -1;
            EndDate = null;
            photos = new EntitySet<PhotoEntity>(
              new Action<PhotoEntity>(this.AddPhoto),
              new Action<PhotoEntity>(this.RemovePhoto));
            routes = new EntitySet<RouteEntity>(
              new Action<RouteEntity>(this.AddRoute),
              new Action<RouteEntity>(this.RemoveRoute));
            moneySpendings = new EntitySet<MoneySpendingEntity>(
             new Action<MoneySpendingEntity>(this.AddMoneySpending),
             new Action<MoneySpendingEntity>(this.RemoveMoneySpending));
        }


        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
                CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get { return id; }
            set { id = value; }
        }


        [Column]
        public long UserId
        {
            get { return userId; }
            set { userId = value; }
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
        public int PhotosQuantity
        {
            get { return photosQuantity; }
            set { photosQuantity = value; }
        }


        [Column]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }


        [Column(CanBeNull = true)]
        public DateTime? EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }


        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] Image
        {
            get { return image; }
            set { image = value; }
        }


        [Column]
        public bool IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = value; }
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
        public DateTime? ChangeDate
        {
            get;
            set;
        }


        [Association(Storage = "photos", OtherKey = "EventId", ThisKey = "Id")]
        public EntitySet<PhotoEntity> Photos
        {
            get { return this.photos; }
            set { this.photos.Assign(value); }
        }


        [Association(Storage = "routes", OtherKey = "EventId", ThisKey = "Id")]
        public EntitySet<RouteEntity> Routes
        {
            get { return this.routes; }
            set { this.routes.Assign(value); }
        }


        [Association(Storage = "moneySpendings", OtherKey = "EventId", ThisKey = "Id")]
        public EntitySet<MoneySpendingEntity> MoneySpendings
        {
            get { return this.moneySpendings; }
            set { this.moneySpendings.Assign(value); }
        }


        private void AddPhoto(PhotoEntity photo)
        {
            photo.EventId = this.id;
        }


        private void RemovePhoto(PhotoEntity photo)
        {
            photo.EventId = 0;
        }


        private void RemoveRoute(RouteEntity route)
        {
            route.EventId = 0;
        }


        private void AddRoute(RouteEntity route)
        {
            route.EventId = this.id;
        }


        private void RemoveMoneySpending(MoneySpendingEntity moneySpending)
        {
            moneySpending.EventId = 0;
        }


        private void AddMoneySpending(MoneySpendingEntity moneySpending)
        {
            moneySpending.EventId = this.id;
        }
    }
}
