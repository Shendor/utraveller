using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class SocialPostImageData : SocialPostData
    {
        public byte[] ImageContent
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }
    }
}
