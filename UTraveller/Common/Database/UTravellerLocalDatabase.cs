using LocalRepositoryImpl.UTraveller.LocalRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Database
{
    public class UTravellerLocalDatabase : LocalDatabase
    {
        public UTravellerLocalDatabase()
            : base("Data Source=isostore:/utraveller_database_local.sdf")
        {
        }
    }
}
