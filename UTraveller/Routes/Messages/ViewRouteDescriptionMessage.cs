using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Routes.Messages
{
    public class ViewRouteDescriptionMessage
    {
        public ViewRouteDescriptionMessage(string description)
        {
            Description = description;
        }

        public string Description
        {
            get;
            set;
        }
    }
}
