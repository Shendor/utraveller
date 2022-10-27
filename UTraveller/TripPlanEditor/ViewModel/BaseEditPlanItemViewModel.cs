using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;
using UTraveller.TripPlanEditor.Messages;
using UTraveller.TripPlanEditor.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.ViewModel
{
    public abstract class BaseEditPlanItemViewModel : BaseViewModel
    {
        private static DateTime previousDate = DateTime.Now;
        private static DateTime previousTime = DateTime.Now;
        private static bool isDateEnabledPreviously = true;

        protected BasePlanItem planItemToUpdate;
        private string address;
        private string description;
        private DateTime date;
        private DateTime time;
        private bool isDateEnabled;

        public BaseEditPlanItemViewModel(BasePlanItem planItemToUpdate)
        {
            this.planItemToUpdate = planItemToUpdate;
            if (planItemToUpdate != null)
            {
                Date = planItemToUpdate.Date != null ? planItemToUpdate.Date.Value : DateTime.Now;
                Time = Date;
                Address = planItemToUpdate.Address;
                Location = planItemToUpdate.Coordinate;
                Description = planItemToUpdate.Description;
                IsVisited = planItemToUpdate.IsVisited;
                IsDateEnabled = planItemToUpdate.Date != null;
            }
            else
            {
                Date = previousDate;
                Time = previousTime;
                IsDateEnabled = isDateEnabledPreviously;
            }

            ChangeLocationCommand = new ActionCommand(ChangeLocation);
        }


        public ICommand ChangeLocationCommand
        {
            get;
            private set;
        }


        public DateTime Date
        {
            get { return date; }
            set { previousDate = date = value; }
        }


        public DateTime Time
        {
            get { return time; }
            set { previousTime = time = value; }
        }


        public bool IsDateEnabled
        {
            get { return isDateEnabled; }
            set { isDateEnabledPreviously = isDateEnabled = value; }
        }


        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                RaisePropertyChanged("Address");
            }
        }


        public GeoCoordinate Location
        {
            get;
            set;
        }


        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged("Description");
            }
        }


        public bool IsVisited
        {
            get;
            set;
        }


        private void ChangeLocation()
        {
            MessengerInstance.Send<EditPlanItemCoordinateMessage>(new EditPlanItemCoordinateMessage(Location));
        }

        public abstract void SavePlanItem(PlanItemTypeModel type);

    }
}
