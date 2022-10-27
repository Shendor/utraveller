using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class AccessTokenRemoteModel
    {
        public string Access_token
        {
            get;
            set;
        }


        public string Token_type
        {
            get;
            set;
        }


        public string Refresh_token
        {
            get;
            set;
        }
    }
}
