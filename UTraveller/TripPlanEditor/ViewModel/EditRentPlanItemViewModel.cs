using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTraveller.TripPlanEditor.Messages;
using UTraveller.TripPlanEditor.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.ViewModel
{
    public class EditRentPlanItemViewModel : BaseEditPlanItemViewModel
    {
        public EditRentPlanItemViewModel(RentPlanItem planItemToUpdate)
            : base(planItemToUpdate)
        {
            IsCheckOutDateEnabled = planItemToUpdate == null || planItemToUpdate.CheckOutDate != null;
            if (planItemToUpdate != null)
            {
                CheckOutDate = planItemToUpdate.CheckOutDate != null ? planItemToUpdate.CheckOutDate.Value : DateTime.Now;
                CheckOutTime = CheckOutDate;
            }
            else
            {
                CheckOutDate = Date.AddDays(1);
                CheckOutTime = Date.AddDays(1);
            }
        }


        public DateTime CheckOutDate
        {
            get;
            set;
        }


        public DateTime CheckOutTime
        {
            get;
            set;
        }



        public bool IsCheckOutDateEnabled
        {
            get;
            set;
        }


        public override void SavePlanItem(PlanItemTypeModel type)
        {
            if (type.EditPlanItemType == EditPlanItemType.Rent)
            {
                var rentPlanItem = new RentPlanItem();
                rentPlanItem.Address = Address;
                rentPlanItem.Coordinate = Location;
                rentPlanItem.IsVisited = IsVisited;
                if (IsDateEnabled)
                {
                    rentPlanItem.Date = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
                } 
                if (IsCheckOutDateEnabled)
                {
                    rentPlanItem.CheckOutDate = new DateTime(CheckOutDate.Year, CheckOutDate.Month, CheckOutDate.Day,
                        CheckOutTime.Hour, CheckOutTime.Minute, 0);
                }
                rentPlanItem.Description = Description;
                rentPlanItem.Type = (RentPlanItemType)type.SelectedPlanItemType;
                MessengerInstance.Send<PlanItemSavedMessage>(new PlanItemSavedMessage(planItemToUpdate, rentPlanItem));
            }
        }
    }
}
