using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class FacebookUserProfile
    {
        public string Id
        {
            get;
            set;
        }


        public string Name
        { 
            get; set; 
        }


        public string Username
        {
            get;
            set;
        }


        public string Email
        {
            get;
            set;
        }


        public string ImageUrl
        {
            get { return String.Format("https://graph.facebook.com/{0}/picture?type=large", Id); } 
        }
        
    }
}
