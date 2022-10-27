using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Model
{
    public class RouteSkyDriveFileRequest : OneDriveFileRequest
    {
        public RouteSkyDriveFileRequest(LiveConnectSession session)
            : base(session)
        {
            Extensions.Add(FileExtensionType.KML);
            Extensions.Add(FileExtensionType.KMZ);
        }

        public RouteSkyDriveFileRequest(string fileName, LiveConnectSession session, string chosenExtension)
            : base(fileName, session)
        {
            Extensions.Add(FileExtensionType.KML);
            Extensions.Add(FileExtensionType.KMZ);
            ChosenExtension = chosenExtension;
        }
    }
}
