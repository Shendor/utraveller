using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class SocialPostData
    {
        public SocialPostData()
        {
            Caption = String.Empty;
            Description = String.Empty;
            Message = String.Empty;
        }

        public long DataId
        {
            get;
            set;
        }


        public string Caption
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }


        public string Message
        {
            get;
            set;
        }


        public string Link
        {
            get; set;
        }


        public FacebookPrivacyType PrivacyType
        {
            get;
            set;
        }
    }
}
