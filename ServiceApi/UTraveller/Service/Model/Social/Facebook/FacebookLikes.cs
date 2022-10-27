using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class FacebookLikes
    {
        public List<FacebookUserProfile> Data
        {
            get;
            set;
        }


        public FacebookPaging Paging
        {
            get;
            set;
        }


        public FacebookPostSummary Summary
        {
            get;
            set;
        }
    }
}
