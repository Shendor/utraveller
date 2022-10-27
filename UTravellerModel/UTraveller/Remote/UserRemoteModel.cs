using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class UserRemoteModel : BaseUserRemoteModel
    {

        public string Password
        {
            get;
            set;
        }


        public string Email
        {
            get;
            set;
        }
    }
}
