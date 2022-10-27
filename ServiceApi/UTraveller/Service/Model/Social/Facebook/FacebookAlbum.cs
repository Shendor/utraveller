using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class FacebookAlbum
    {
        public FacebookAlbum()
        {
            PrivacyType = FacebookPrivacyType.ALL_FRIENDS;
        }


        public string Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public FacebookPrivacyType PrivacyType
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
