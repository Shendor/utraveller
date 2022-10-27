using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class PhotoRemoteModel : BaseRemoteModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string ImageUrl { get; set; }

        public GeoLocationModel Coordinate { get; set; }

        public byte[] Thumbnail { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string FacebookPhotoId { get; set; }

        public string FacebookPostId { get; set; }
    }
}
