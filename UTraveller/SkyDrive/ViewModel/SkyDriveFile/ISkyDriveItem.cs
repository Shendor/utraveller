using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.SkyDrive.ViewModel
{
    public interface ISkyDriveItem
    {
        Uri IconUri { get; }

        string Name { get; }

        void SendOpenMessage();
    }
}
