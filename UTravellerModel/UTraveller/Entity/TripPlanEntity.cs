using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Entity
{
    [Table]
    public class TripPlanEntity : IRemotableEntity<long>
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
           CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get;
            set;
        }


        [Column]
        public long RemoteId
        {
            get;
            set;
        }


        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string PlanItems
        {
            get;
            set;
        }


        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string FlightPlanItems
        {
            get;
            set;
        }


        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public string RentPlanItems
        {
            get;
            set;
        }


        [Column]
        public bool IsDeleted
        {
            get;
            set;
        }


        [Column]
        public bool IsSync
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


        [Column]
        public long EventId
        {
            get;
            set;
        }
    }
}
