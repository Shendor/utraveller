using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class ErrorRemoteModel
    {
        public ErrorRemoteModel()
        {
        }

        public ErrorRemoteModel(string errorCode)
            : this(errorCode, null)
        {
        }


        public ErrorRemoteModel(string errorCode, string message)
        {
            ErrorCode = errorCode;
            Error = message;
        }

        public string ErrorCode
        {
            get;
            set;
        }


        public string Error
        {
            get;
            set;
        }
    }
}
