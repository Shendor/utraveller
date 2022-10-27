using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Util
{
    public class GeoCoordinateUtils
    {
        public static LocationRectangle GetArea(ICollection<GeoCoordinate> coordinates)
        {
            if (coordinates.Count > 0)
            {
                return LocationRectangle.CreateBoundingRectangle(coordinates);
            }
            else
            {
                return new LocationRectangle();
            }
        }

        public static GeoCoordinate GetCenter(ICollection<GeoCoordinate> coordinates)
        {
            var center = new GeoCoordinate();

            double minLatitude = Double.MaxValue;
            double minLongitude = Double.MaxValue;
            double maxLatitude = 0;
            double maxLongitude = 0;

            foreach (var coordinate in coordinates)
            {
                if (coordinate.Latitude < minLatitude)
                {
                    minLatitude = coordinate.Latitude;
                }
                if (coordinate.Longitude < minLongitude)
                {
                    minLongitude = coordinate.Longitude;
                }

                if (coordinate.Latitude > maxLatitude)
                {
                    maxLatitude = coordinate.Latitude;
                }
                if (coordinate.Longitude > maxLongitude)
                {
                    maxLongitude = coordinate.Longitude;
                }
            }

            if (maxLongitude != 0)
            {
                center = new GeoCoordinate((maxLatitude + minLatitude) / 2, (maxLongitude + minLongitude) / 2);
            }
            return center;
        }

        /// <summary>
        /// Converts geo coordinates to string. Format: latitude1;longitude1 latitude2;longitude2 ...
        /// </summary>
        /// <param name="coordinates">Geo coordinates</param>
        /// <returns>string representation</returns>
        public static string ToString(ICollection<GeoCoordinate> coordinates)
        {
            StringBuilder coordinatesAsString = new StringBuilder();
            var format = (IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(NumberFormatInfo));
            foreach (var coordinate in coordinates)
            {
                coordinatesAsString.Append(coordinate.Latitude.ToString(format)).Append(";").
                    Append(coordinate.Longitude.ToString(format)).Append(" ");
            }
            coordinatesAsString.Remove(coordinatesAsString.Length - 1, 1); // remove last space character
            return coordinatesAsString.ToString();
        }

        /// <summary>
        /// Converts string of geo coordinates to collection of geo coordinates with latitude and longitude
        /// </summary>
        /// <param name="coordinatesString">string coordinates</param>
        /// <returns>geo coordinates</returns>
        public static GeoCoordinateCollection FromString(string coordinatesString)
        {
            var format = (IFormatProvider)CultureInfo.InvariantCulture.GetFormat(typeof(NumberFormatInfo));
            var convertedCoordinates = new GeoCoordinateCollection();
            var coordinatePairs = coordinatesString.Split(' ');
            foreach (var coordinatePair in coordinatePairs)
            {
                var coordinateAsString = coordinatePair.Split(';');
                double latitude = 0;
                double longitude = 0;
                Double.TryParse(coordinateAsString[0], NumberStyles.Any, format, out latitude);
                Double.TryParse(coordinateAsString[1], NumberStyles.Any, format, out longitude);

                convertedCoordinates.Add(new GeoCoordinate(latitude, longitude));
            }

            return convertedCoordinates;
        }

        internal static GeoCoordinateCollection FromGeoCoordinates(ICollection<GeoLocationModel> locations)
        {
            var geoCoordinateCollection = new GeoCoordinateCollection();
            foreach (var location in locations)
            {
                geoCoordinateCollection.Add(new GeoCoordinate(location.Lat, location.Lng));
            }

            return geoCoordinateCollection;
        }
    }
}
