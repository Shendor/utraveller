using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class TripRestoreResult
    {
        public TripRestoreResult()
        {
            FailedTrips = new List<FailedTripResult>();
            TripsRestoredWithWarnings = new List<FailedTripResult>();
            RestoredFilenames = new List<string>();
        }

        public ICollection<string> RestoredFilenames
        {
            get;
            set;
        }

        public ICollection<FailedTripResult> TripsRestoredWithWarnings
        {
            get;
            set;
        }

        public ICollection<FailedTripResult> FailedTrips
        {
            get;
            set;
        }

        public bool HasErrors
        {
            get { return FailedTrips.Count > 0; }
        }

        public bool IsSuccess
        {
            get { return !HasErrors && RestoredFilenames.Count > 0; }
        }

        public string ErrorMessage
        {
            get
            {
                return CreateMessage(FailedTrips);
            }
        }

        public string WarningMessage
        {
            get
            {
                return CreateMessage(TripsRestoredWithWarnings);
            }
        }

        private string CreateMessage(ICollection<FailedTripResult> failedTripsResult)
        {
            var errorMessage = new StringBuilder();
            foreach (var failedTrip in failedTripsResult)
            {
                errorMessage.Append(string.Format("{0} : '{1}'\n", failedTrip.Filename, failedTrip.Reason));
            }
            return errorMessage.ToString();
        }

        public bool HasWarnings
        {
            get { return TripsRestoredWithWarnings.Count > 0; }
        }
    }

    public class FailedTripResult
    {
        public FailedTripResult(string filename, string reason)
        {
            Filename = filename;
            Reason = reason;
        }

        public string Filename
        {
            get;
            set;
        }

        public string Reason
        {
            get;
            set;
        }
    }
}
