using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class RemoteModel<T>
    {
        public RemoteModel()
        {
        }


        public RemoteModel(T response)
        {
            ResponseObject = response;
        }


        public DateTime? ChangeDate 
        { 
            get; set; 
        }


        public ErrorRemoteModel Error
        {
            get;
            set;
        }


        public T ResponseObject
        {
            get;
            set;
        }
    }
}
