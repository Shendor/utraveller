using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using ServiceImpl.UTraveller.Service.Implementation;
using Windows.Devices.Geolocation;
using System.Diagnostics;
using Microsoft.Phone.Maps.Services;

namespace UTraveller.Service.Implementation
{
    public class GeoCoordinateService : IGeoCoordinateService
    {
        private static readonly double PRECISION = 0.003;

        public bool IsNeighbours(GeoCoordinate geoCoordinate1, GeoCoordinate geoCoordinate2)
        {
            double distance = Math.Sqrt(Math.Pow((geoCoordinate2.Latitude - geoCoordinate1.Latitude), 2) +
                Math.Pow((geoCoordinate2.Longitude - geoCoordinate1.Longitude), 2));

            return distance <= PRECISION;
        }


        public async Task<GeoCoordinate> ApplyCurrentLocation([OptionalAttribute]double movementThreshold, [OptionalAttribute]double timeout)
        {
            var geoLocator = new Geolocator();
            geoLocator.MovementThreshold = movementThreshold;
            Geoposition result = null;
            try
            {
                result = await geoLocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(timeout));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't get geoposition: " + ex.Message);
            }

            return new GeoCoordinate(result == null ? 0 : result.Coordinate.Point.Position.Latitude,
              result == null ? 0 : result.Coordinate.Point.Position.Longitude);
        }


        public void FindAddressForCoordinate(GeoCoordinate coordinate, AddressFoundDelegate addressFoundDelegate)
        {
            var query = new ReverseGeocodeQuery()
            {
                GeoCoordinate = coordinate
            };
            
            query.QueryCompleted += (s, e) =>
            {
                StringBuilder address = new StringBuilder();
                if (e.Error == null)
                {
                    foreach (var result in e.Result)
                    {
                        bool hasCity = !string.IsNullOrEmpty(result.Information.Address.City);
                        bool hasStreet = !string.IsNullOrEmpty(result.Information.Address.Street);

                        if ((!hasCity || !hasStreet) && !string.IsNullOrEmpty(result.Information.Address.Country))
                        {
                            address.Append(result.Information.Address.Country);
                        }
                        if (hasCity)
                        {
                            if (address.Length > 0)
                            {
                                address.Append(", ");
                            }
                            address.Append(result.Information.Address.City);
                        }
                        if (!string.IsNullOrEmpty(result.Information.Address.District))
                        {
                            address.Append(", ").Append(result.Information.Address.District);
                        }
                        if (hasStreet)
                        {
                            address.Append(", ").Append(result.Information.Address.Street);
                        }
                        if (!string.IsNullOrEmpty(result.Information.Address.HouseNumber))
                        {
                            address.Append(" ").Append(result.Information.Address.HouseNumber);
                        }
                        break;
                    }
                }
                addressFoundDelegate(address.ToString());
            };
            query.QueryAsync();
        }


        public void FindCoordinateForAddress(string address, GeoCoordinateFoundDelegate coordinateFoundDelegate)
        {
            var query = new GeocodeQuery()
            {
                GeoCoordinate = new GeoCoordinate(0, 0),
                MaxResultCount = 1,
                SearchTerm = address
            };

            query.QueryCompleted += (s, e) =>
            {
                GeoCoordinate coordinate = null;
                if (e.Error == null)
                {
                    foreach (var getCoordinate in e.Result)
                    {
                        coordinate = getCoordinate.GeoCoordinate;
                        break;
                    }
                }
                coordinateFoundDelegate(coordinate);
            };

            query.QueryAsync();
        }
    }
}
