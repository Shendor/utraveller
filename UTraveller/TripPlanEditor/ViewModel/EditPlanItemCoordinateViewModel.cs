using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Control;
using UTraveller.Common.ViewModel;
using UTraveller.TripPlanEditor.Messages;
using UTravellerModel.UTraveller.Model;
using Windows.Devices.Geolocation;

namespace UTraveller.TripPlanEditor.ViewModel
{
    public class EditPlanItemCoordinateViewModel : BaseViewModel
    {
        private static GeoCoordinate previousCoordinate = new GeoCoordinate(48, 8);
        private static readonly string UNDEFINED_ADDRESS_NAME = "Undefined Address";

        private PushpinDescriptionService routePushpinDescriptionService;
        private string address;

        public EditPlanItemCoordinateViewModel(PushpinDescriptionService routePushpinDescriptionService)
        {
            this.routePushpinDescriptionService = routePushpinDescriptionService;

            MessengerInstance.Register<EditPlanItemCoordinateMessage>(this, OnEditPlanItemCoordinate);
            MessengerInstance.Register<ViewPlanItemCoordinateMessage>(this, OnViewPlanItemCoordinate);
        }


        public override void Cleanup()
        {
            if (Coordinate != null)
            {
                previousCoordinate = new GeoCoordinate(Coordinate.Latitude, Coordinate.Longitude);
            }
            Coordinate = null;
            address = null;
        }


        public bool IsViewMode
        {
            get;
            private set;
        }


        public bool IsEditAvailableFirstTime
        {
            get;
            private set;
        }


        public GeoCoordinate Coordinate
        {
            get;
            set;
        }


        public void ShowAddress()
        {
            if (Coordinate != null)
            {
                routePushpinDescriptionService.Show(string.IsNullOrEmpty(address) ? UNDEFINED_ADDRESS_NAME : address, Coordinate);
            }
        }


        public void ApplyCoordinate(GeoCoordinate coordinate)
        {
            if (!IsViewMode)
            {
                MessengerInstance.Send<ApplyPlanItemCoordinate>(new ApplyPlanItemCoordinate(coordinate));
            }
        }


        private void OnEditPlanItemCoordinate(EditPlanItemCoordinateMessage message)
        {
            IsViewMode = false;
            ChangeCoordinateOfPushpin(message.Coordinate);
        }


        private void OnViewPlanItemCoordinate(ViewPlanItemCoordinateMessage message)
        {
            IsViewMode = true;
            address = message.Address;
            ChangeCoordinateOfPushpin(message.Coordinate);
        }


        private void ChangeCoordinateOfPushpin(GeoCoordinate coordinate)
        {
            if (coordinate == null)
            {
                IsEditAvailableFirstTime = true;
                Coordinate = previousCoordinate;
            }
            else
            {
                IsEditAvailableFirstTime = false;
                Coordinate = coordinate;
            }
            RaisePropertyChanged("Coordinate");
        }
    }
}
