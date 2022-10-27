using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.TripPlanEditor.Model;

namespace UTraveller.TripPlanEditor.Messages
{
    public class EditPlanItemTypeChangedMessage
    {
        public EditPlanItemTypeChangedMessage(EditPlanItemType editPlanItemType)
        {
            EditPlanItemType = editPlanItemType;
        }


        public EditPlanItemType EditPlanItemType
        {
            get;
            private set;
        }
    }
}
