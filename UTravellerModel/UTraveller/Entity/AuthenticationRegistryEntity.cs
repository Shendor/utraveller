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
    public class AuthenticationRegistryEntity : IBaseEntity<long>
    {
        private long id;
        private string email;
        private string password;
        private DateTime signInDate;

        public AuthenticationRegistryEntity()
        {
            signInDate = DateTime.Now;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "BIGINT NOT NULL Identity",
           CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public long Id
        {
            get { return id; }
            set { id = value; }
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
        public DateTime SignInDate
        {
            get { return signInDate; }
            set { signInDate = value; }
        }
    }
}
