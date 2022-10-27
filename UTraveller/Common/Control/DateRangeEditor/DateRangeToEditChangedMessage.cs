using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Common.Control.DateRangeEditor
{
    public class DateRangeToEditChangedMessage
    {
        public DateRangeToEditChangedMessage(object token, DateTime startDate, DateTime? endDate)
        {
            Token = token;
            StartDate = startDate;
            EndDate = endDate;
        }

        public object Token
        {
            get;
            set;
        }


        public DateTime StartDate
        {
            get;
            set;
        }


        public DateTime? EndDate
        {
            get;
            set;
        }
    }
}
