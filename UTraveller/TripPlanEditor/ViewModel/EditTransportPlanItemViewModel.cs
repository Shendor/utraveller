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
    public class EditTransportPlanItemViewModel : BaseEditPlanItemViewModel
    {
        public EditTransportPlanItemViewModel(TransportPlanItem planItemToUpdate):base(planItemToUpdate)
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
            if (type.EditPlanItemType == EditPlanItemType.Transport && type.SelectedPlanItemType != null)
            {
                var transportPlanItem = new TransportPlanItem();
                transportPlanItem.Address = Address;
                transportPlanItem.Coordinate = Location;
                transportPlanItem.IsVisited = IsVisited;
                if (IsDateEnabled)
                {
                    transportPlanItem.Date = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
                }
                transportPlanItem.Description = Description;
                transportPlanItem.Destination = Destination;
                transportPlanItem.Type = (TransportPlanItemType)type.SelectedPlanItemType;
                MessengerInstance.Send<PlanItemSavedMessage>(new PlanItemSavedMessage(planItemToUpdate, transportPlanItem));
            }
        }
    }
}
