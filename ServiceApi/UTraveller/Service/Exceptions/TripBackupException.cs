using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Exceptions
{
    public class TripBackupException : Exception
    {
        public TripBackupException(string message)
            : base(message)
        {

        }
    }
}
