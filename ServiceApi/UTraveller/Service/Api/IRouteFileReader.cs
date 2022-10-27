using Microsoft.Phone.Maps.Controls;
using ServiceApi.UTraveller.Service.Model;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IRouteFileReader
    {
        Task<RouteInfo> ReadRoute(Stream fileStream);
    }
}
