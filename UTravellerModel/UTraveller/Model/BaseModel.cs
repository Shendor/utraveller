using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public abstract class BaseModel
    {
        private long id;
        private long remoteId;
        private bool isSync;
        private bool isDeleted;

        public long Id
        {
            get { return id; }
            set { id = value; }
        }


        public long RemoteId
        {
            get { return remoteId; }
            set { remoteId = value; }
        }


        public bool IsSync
        {
            get { return isSync; }
            set { isSync = value; }
        }


        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }


        public DateTime? ChangeDate
        {
            get;
            set;
        }
    }
}
