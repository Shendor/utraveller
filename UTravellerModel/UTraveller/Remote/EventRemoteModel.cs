using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class EventRemoteModel : BaseRemoteModel
    {
        
        public String Name { get; set; }


        public DateTime StartDate { get; set; }


        public DateTime? EndDate { get; set; }


        public byte[] Image { get; set; }
    }
}
