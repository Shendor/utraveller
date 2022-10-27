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
    public class AppPropertiesEntity : IBaseEntity<long>
    {

        public AppPropertiesEntity()
        {
            IsUploadToFacebook = true;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
          CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get;
            set;
        }


        [Column]
        public long UserId
        {
            get;
            set;
        }


        [Column]
        public byte[] BackgroundColor
        {
            get;
            set;
        }


        [Column]
        public byte[] MainColor
        {
            get;
            set;
        }


        [Column]
        public byte[] TextColor
        {
            get;
            set;
        }


        [Column]
        public double CoverOpacity
        {
            get;
            set;
        }


        [Column]
        public double PanelsOpacity
        {
            get;
            set;
        }


        [Column]
        public bool IsLandscapeCover
        {
            get;
            set;
        }


        [Column]
        public bool IsUploadToFacebook
        {
            get;
            set;
        }


        [Column]
        public bool IsConnectToServerAutomatically
        {
            get;
            set;
        }


        [Column]
        public bool IsAllowGeoPosition
        {
            get;
            set;
        }

        [Column]
        public string LimitationCode
        {
            get;
            set;
        }
    }
}
