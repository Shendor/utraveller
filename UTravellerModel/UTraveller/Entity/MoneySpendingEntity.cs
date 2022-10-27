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
    public class MoneySpendingEntity : IRemotableEntity<long>
    {
        private long id;
        private long remoteId;
        private MoneySpendingCategory moneySpendingCategory;
        private decimal amount;
        private string description;
        private CurrencyType currency;
        private DateTime date;
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

        [Column(CanBeNull=false)]
        public MoneySpendingCategory MoneySpendingCategory
        {
            get { return moneySpendingCategory; }
            set { moneySpendingCategory = value; }
        }


        [Column(CanBeNull = false)]
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }


        [Column]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        [Column(CanBeNull = false)]
        public CurrencyType Currency
        {
            get { return currency; }
            set { currency = value; }
        }


        [Column]
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
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
            get;
            set;
        }


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
                    value.MoneySpendings.Add(this);
                }
            }
        }

    }
}
