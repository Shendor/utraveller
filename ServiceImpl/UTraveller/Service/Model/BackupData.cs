using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace ServiceImpl.UTraveller.Service.Model
{
    public class BackupData
    {

        public BackupData()
        {
            Date = DateTime.Now;
        }

        public DateTime Date
        {
            get;
            set;
        }

        public Event Trip
        {
            get;
            set;
        }

        public IEnumerable<Photo> Photos
        {
            get;
            set;
        }

        public IEnumerable<Message> Messages
        {
            get;
            set;
        }

        public IEnumerable<MoneySpending> Expenses
        {
            get;
            set;
        }

        public IEnumerable<Route> Routes
        {
            get;
            set;
        }

        public TripPlan TripPlan
        {
            get;
            set;
        }
    }
}
