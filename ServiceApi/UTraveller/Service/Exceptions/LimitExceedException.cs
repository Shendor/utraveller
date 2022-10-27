using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApi.UTraveller.Service.Exceptions
{
    public class LimitExceedException : Exception
    {
        public LimitExceedException(int limit, string entityName)
            : base("Limit exceeded for this entity " + entityName)
        {
            Limit = limit;
        }

        public int Limit
        {
            get;
            private set;
        }
    }
}
