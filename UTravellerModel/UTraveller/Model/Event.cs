using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class Event : BaseModel, IDateItem
    {
        private long userId;
        private string name;
        private DateTime creationDate;
        private DateTime startDate;
        private DateTime? endDate;
        private int photosQuantity;
        private byte[] image;
        private bool isCurrent;

        public Event()
        {
            photosQuantity = -1;
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public DateTime CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }


        public DateTime Date
        {
            get { return startDate; }
            set
            {
                startDate = value;
                if (endDate != null && startDate > endDate)
                {
                    startDate = endDate.Value;
                    endDate = value;
                }
            }
        }


        public DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                if (endDate != null && endDate.Value < Date)
                {
                    endDate = Date;
                    Date = value.Value;
                }
            }
        }


        public byte[] Image
        {
            get { return image; }
            set { image = value; }
        }


        public int PhotosQuantity
        {
            get { return photosQuantity; }
            set { photosQuantity = value; }
        }


        public string DateRange
        {
            get
            {
                string result = null;
                if (EndDate == null ||
                    Date.Day == EndDate.Value.Day && Date.Month == EndDate.Value.Month && Date.Year == EndDate.Value.Year)
                {
                    result = string.Format("{0} (1 day)", Date.ToShortDateString());
                }
                else
                {
                    var totalDays = (int)(EndDate.Value - Date).TotalDays;
                    string daysText = totalDays == 1 ? "day" : "days";
                    result = string.Format("{0} - {1} ({2} {3})",
                        Date.ToShortDateString(), EndDate.Value.ToShortDateString(), totalDays, daysText);
                }
                return result;
            }
        }


        public bool IsCurrent
        {
            get { return isCurrent; }
            set { isCurrent = value; }
        }


        public long UserId
        {
            get { return userId; }
            set { userId = value; }
        }

    }
}
