using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class BaseUserRemoteModel
    {
        public BaseUserRemoteModel()
        {
            RegisterDate = DateTime.Now;
        }

        public long Id 
        { 
            get;
            set;
        }


        public DateTime RegisterDate
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        public byte[] Avatar
        {
            get;
            set;
        }


        public byte[] Cover
        {
            get;
            set;
        }

    }
}
