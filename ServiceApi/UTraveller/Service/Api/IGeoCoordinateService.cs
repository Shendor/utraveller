using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface IGeoCoordinateService
    {
        void FindAddressForCoordinate(GeoCoordinate coordinate, AddressFoundDelegate addressFoundDelegate);

        void FindCoordinateForAddress(string address, GeoCoordinateFoundDelegate coordinateFoundDelegate);

        Task<GeoCoordinate> ApplyCurrentLocation(double movementThreshold = 20, double timeout = 5);

        bool IsNeighbours(GeoCoordinate geoCoordinate1, GeoCoordinate geoCoordinate2);
    }


    public delegate void AddressFoundDelegate(string address);

    public delegate void GeoCoordinateFoundDelegate(GeoCoordinate coordinate);
}
