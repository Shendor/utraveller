using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using Windows.Networking.Connectivity;

namespace UTraveller.Service.Implementation
{
    public class NetworkConnectionCheckService : INetworkConnectionCheckService
    {
        public bool HasConnection
        {
            get
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
        }
    }
}
