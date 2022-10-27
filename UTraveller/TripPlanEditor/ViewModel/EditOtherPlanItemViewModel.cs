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
    public class EditOtherPlanItemViewModel : BaseEditPlanItemViewModel
    {

        public EditOtherPlanItemViewModel(BasePlanItem planItemToUpdate) : base(planItemToUpdate)
        {
        }

        public override void SavePlanItem(PlanItemTypeModel type)
        {
            if (type.EditPlanItemType == EditPlanItemType.Other)
            {
                var planItem = new PlanItem();
                planItem.Address = Address;
                planItem.Coordinate = Location;
                planItem.IsVisited = IsVisited;
                if (IsDateEnabled)
                {
                    planItem.Date = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
                }
                planItem.Description = Description;
                planItem.Type = (PlanItemType)type.SelectedPlanItemType;
                MessengerInstance.Send<PlanItemSavedMessage>(new PlanItemSavedMessage(planItemToUpdate, planItem));
            }
        }
    }
}
