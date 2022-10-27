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
    public class RoutePushpinEntity : IBaseEntity<long>
    {
        private long id;
        private string description;
        private double longitude;
        private double latitude;
        private byte[] thumbnail;
        private byte rColor;
        private byte gColor;
        private byte bColor;
        private long routeId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
                CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        [Column]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Column]
        public double Longitude
        {
            get { return longitude; }
            set { longitude = value; }
        }

        [Column]
        public double Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
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
        public long RouteId
        {
            get { return routeId; }
            set { routeId = value; }
        }

    }
}
