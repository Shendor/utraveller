using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.TripPlanEditor.Messages;
using UTraveller.TripPlanEditor.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.ViewModel
{
    public class EditFlightPlanItemViewModel : BaseEditPlanItemViewModel
    {
        public EditFlightPlanItemViewModel(FlightPlanItem planItemToUpdate):base(planItemToUpdate)
        {
            Destination = planItemToUpdate != null ? planItemToUpdate.Destination : null;
        }

        public string Destination
        {
            get;
            set;
        }


        public override void SavePlanItem(PlanItemTypeModel type)
        {
            if (type.EditPlanItemType == EditPlanItemType.Flight)
            {
                var flightPlanItem = new FlightPlanItem();
                flightPlanItem.Address = Address;
                flightPlanItem.Coordinate = Location;
                flightPlanItem.IsVisited = IsVisited;
                if (IsDateEnabled)
                {
                    flightPlanItem.Date = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
                }
                flightPlanItem.Description = Description;
                flightPlanItem.Destination = Destination;
                MessengerInstance.Send<PlanItemSavedMessage>(new PlanItemSavedMessage(planItemToUpdate, flightPlanItem));
            }
        }
    }
}
