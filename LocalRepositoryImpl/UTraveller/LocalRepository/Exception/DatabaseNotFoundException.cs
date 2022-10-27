using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalRepositoryImpl.UTraveller.LocalRepository
{
    public class DatabaseNotFoundException : System.Exception
    {
        public DatabaseNotFoundException()
            : base("Database was not provided for repository")
        {
        }
    }
}
