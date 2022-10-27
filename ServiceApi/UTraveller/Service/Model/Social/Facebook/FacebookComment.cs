using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class FacebookComment
    {
        public FacebookComment(string message)
        {
            Message = message;
        }

        public FacebookUserProfile From
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

    }
}
