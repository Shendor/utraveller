using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class MessageRemoteModel : BaseRemoteModel
    {

        public string Text { get; set; }


        public GeoLocationModel Coordinate { get; set; }


        public DateTime Date { get; set; }


        public string FacebookPostId { get; set; }
    }
}
