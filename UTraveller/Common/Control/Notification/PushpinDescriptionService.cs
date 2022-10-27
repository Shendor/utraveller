using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;
using Windows.System;

namespace UTraveller.Common.Control
{
    public class PushpinDescriptionService : NotificationService
    {
        public static readonly string MESSAGE_TOKEN = typeof(PushpinDescriptionService).Name;
        private static readonly string DRIVE = "ms-drive-to";
        private static readonly string WALK = "ms-walk-to";

        public PushpinDescriptionService()
        {
            MessageToken = MESSAGE_TOKEN;
        }


        public GeoCoordinate Coordinate
        {
            get;
            private set;
        }


        public void Show(string text, GeoCoordinate coordinate)
        {
            this.Coordinate = coordinate;
            base.Show(text);
        }


        public async void DriveTo()
        {
            await LaunchNavigation(DRIVE);
        }


        public async void WalkTo()
        {
            await LaunchNavigation(WALK);
        }


        private async Task LaunchNavigation(string navigationType)
        {
            if (Coordinate != null)
            {
                Uri driveToUri = null;
                if (navigationType.Equals(DRIVE))
                {
                    driveToUri = new Uri(String.Format(
                                 navigationType + ":?destination.latitude={0}&destination.longitude={1}&destination.name={2}",
                                 Coordinate.Latitude, Coordinate.Longitude, Text));
                }
                else
                {
                    driveToUri = new Uri(String.Format(
                               navigationType + ":?destination.latitude={0}&destination.longitude={1}",
                               Coordinate.Latitude, Coordinate.Longitude));
                }

                if (!await Launcher.LaunchUriAsync(driveToUri))
                {
                    System.Windows.MessageBox.Show("Falied to run navigation!");
                }
            }
        }
    }
}
