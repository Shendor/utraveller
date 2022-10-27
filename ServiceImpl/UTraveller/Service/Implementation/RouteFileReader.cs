using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Phone.Maps.Controls;
using ServiceApi.UTraveller.Service.Model;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using UTraveller.Common.Converter;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class RouteFileReader : IRouteFileReader
    {
        private static readonly string NO_NAME = "No name";
        private static byte[] defaultPushpinContent;

        private INetworkConnectionCheckService networkConnectionCheckService;
        private IDictionary<string, byte[]> iconsCache;

        public RouteFileReader(INetworkConnectionCheckService networkConnectionCheckService)
        {
            this.networkConnectionCheckService = networkConnectionCheckService;
            iconsCache = new Dictionary<string, byte[]>();
        }

        public async Task<RouteInfo> ReadRoute(Stream fileStream)
        {
            RouteInfo routeInfo = null;

            try
            {
                fileStream.Position = 0;
                KmlFile kmlFile = KmlFile.Load(fileStream);
                Kml kml = kmlFile.Root as Kml;

                if (kml != null)
                {
                    routeInfo = new RouteInfo();
                    var styles = ReadAndGroupStyles(kml);
                    ReadDocumentInfo(routeInfo, kml);
                    ReadCoordinates(routeInfo, kml);
                    ReadPolygons(routeInfo, styles, kml);
                    await ReadPlacemarks(routeInfo, styles, kml);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                fileStream.Close();
            }
            return routeInfo;
        }


        private static void ReadDocumentInfo(RouteInfo routeInfo, Kml kml)
        {
            foreach (var document in kml.Flatten().OfType<Document>())
            {
                routeInfo.Name = document.Name == null || document.Name.Equals(string.Empty) ? NO_NAME : document.Name;
                if (document.Description != null)
                {
                    routeInfo.Description = string.IsNullOrEmpty(document.Description.Text) ?
                        document.Description.Text : document.Description.Text.Replace("<br>", "\r"); ;
                }
            }
        }


        private async Task ReadPlacemarks(RouteInfo routeInfo, IDictionary<string, Style> styles, Kml kml)
        {
            var urlDictionary = new Dictionary<RoutePushpin, string>();
            foreach (var placemark in kml.Flatten().OfType<Placemark>())
            {
                if (placemark.StyleUrl != null && styles.ContainsKey(placemark.StyleUrl.ToString()))
                {
                    var style = styles[placemark.StyleUrl.ToString()];
                    var pushpin = new RoutePushpin();
                    pushpin.Description = (placemark.Description != null ? placemark.Name + "\r" + placemark.Description.Text.Replace("<br>", "\r")
                        : placemark.Name).Replace("gx_image_links:", String.Empty);
                    if (placemark.Geometry.GetType() == typeof(Point))
                    {
                        var point = (Point)placemark.Geometry;
                        pushpin.Coordinate = new GeoCoordinate(point.Coordinate.Latitude, point.Coordinate.Longitude);
                    }
                    if (style.Icon != null && style.Icon.Color != null)
                    {
                        var color = style.Icon.Color.Value;
                        pushpin.Color = Color.FromArgb(255, color.Red, color.Green, color.Blue);
                    }
                    if (style.Icon != null && style.Icon.Icon != null && style.Icon.Icon.Href.IsAbsoluteUri)
                    {
                        urlDictionary.Add(pushpin, style.Icon.Icon.Href.AbsoluteUri);
                        routeInfo.Pushpins.Add(pushpin);
                    }
                }
            }

            if (networkConnectionCheckService.HasConnection)
            {
                foreach (var pushpin in routeInfo.Pushpins)
                {
                    var uri = urlDictionary[pushpin];
                    if (iconsCache.ContainsKey(uri))
                    {
                        pushpin.ThumbnailContent = iconsCache[uri];
                    }
                    else
                    {
                        await DownloadRemoteImage(uri, pushpin);
                    }
                }
            }
            else
            {
                foreach (var pushpin in routeInfo.Pushpins)
                {
                    pushpin.ThumbnailContent = DefaultPushpinContent;
                }
            }
        }


        private static void ReadCoordinates(RouteInfo routeInfo, Kml kml)
        {
            routeInfo.Coordinates = new List<RouteCoordinates>();
            int lineNumber = 1;
            foreach (var line in kml.Flatten().OfType<LineString>())
            {
                int i = 0;
                var routeCoordinates = new RouteCoordinates(lineNumber++);
                if (line.Coordinates.Count > 1000)
                {
                    foreach (var coordinate in line.Coordinates)
                    {
                        if (i++ % 2 == 0)
                        {
                            routeCoordinates.Coordinates.Add(new GeoLocationModel(coordinate.Latitude, coordinate.Longitude));
                        }
                    }
                }
                else
                {
                    foreach (var coordinate in line.Coordinates)
                    {
                        routeCoordinates.Coordinates.Add(new GeoLocationModel(coordinate.Latitude, coordinate.Longitude));
                    }
                }
                routeInfo.Coordinates.Add(routeCoordinates);
            }
        }


        private static void ReadPolygons(RouteInfo routeInfo, IDictionary<string, Style> styles, Kml kml)
        {
            foreach (var polygon in kml.Flatten().OfType<Polygon>())
            {
                RoutePolygon routePolygon = new RoutePolygon();
                var placemark = polygon.Parent as Placemark;
                if (placemark != null && styles.ContainsKey(placemark.StyleUrl.ToString()))
                {
                    routePolygon.Name = placemark.Name;
                    var style = styles[placemark.StyleUrl.ToString()];
                    if (style.Polygon != null && style.Polygon.Color != null)
                    {
                        var color = style.Polygon.Color.Value;
                        routePolygon.Color = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
                    }
                }
                foreach (var coordinate in polygon.OuterBoundary.LinearRing.Coordinates)
                {
                    routePolygon.Coordinates.Add(coordinate.Latitude);
                    routePolygon.Coordinates.Add(coordinate.Longitude);
                }
                routeInfo.Polygons.Add(routePolygon);
            }
        }


        private static IDictionary<string, Style> ReadAndGroupStyles(Kml kml)
        {
            var styles = new Dictionary<string, Style>();
            foreach (var style in kml.Flatten().OfType<Style>())
            {

                styles["#" + style.Id.Replace("-normal", "").Replace("-highlight", "")] = style;
            }
            return styles;
        }


        private async Task DownloadRemoteImage(string uri, RoutePushpin pushpin)
        {
            var httpClient = CreateHttpClient();
            try
            {
                var response = await httpClient.GetByteArrayAsync(new Uri(uri));
                if (response != null)
                {
                    pushpin.ThumbnailContent = response;
                    iconsCache.Add(uri, pushpin.ThumbnailContent);
                }
            }
            catch (Exception ex)
            {
                pushpin.ThumbnailContent = DefaultPushpinContent;
                Debug.WriteLine("Cannot read Icon from url for pushpin: " + ex.Message);
            }
        }

        private static byte[] DefaultPushpinContent
        {
            get
            {
                if (defaultPushpinContent == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    using (var stream = assembly.GetManifestResourceStream("ServiceImpl.Resource.pushpin.png"))
                    {
                        defaultPushpinContent = new byte[stream.Length];
                        stream.Read(defaultPushpinContent, 0, defaultPushpinContent.Length);
                    }
                }
                return defaultPushpinContent;
            }
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.AcceptCharset.Add(new StringWithQualityHeaderValue("UTF-8"));
            client.Timeout = System.TimeSpan.FromSeconds(10);
            return client;
        }
    }
}
