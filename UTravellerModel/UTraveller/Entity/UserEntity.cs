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
    public class UserEntity : IBaseEntity<long>
    {
        private long id;
        private long remoteId;
        private string name;
        private string email;
        private string password;
        private string description;
        private DateTime registerDate;
        private byte[] avatar;
        private byte[] cover;
        private bool isSync;
        
        public UserEntity()
        {
            registerDate = DateTime.Now;
        }


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
        public string Email
        {
            get { return email; }
            set { email = value; }
        }


        [Column]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        [Column]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        [Column]
        public DateTime RegisterDate
        {
            get { return registerDate; }
            set { registerDate = value; }
        }


        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }


        [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
        public byte[] Cover
        {
            get { return cover; }
            set { cover = value; }
        }


        [Column]
        public bool IsSync
        {
            get { return isSync; }
            set { isSync = value; }
        }
    }
}
