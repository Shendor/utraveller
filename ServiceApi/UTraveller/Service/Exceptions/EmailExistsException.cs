using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Exceptions
{
    public class EmailExistsException : Exception
    {
        public EmailExistsException()
            : base("Email already exists")
        {
        }
    }
}
