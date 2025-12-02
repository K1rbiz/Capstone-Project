using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Networking;

namespace Capstone_Project_v0._1.Services
{
    public interface IConnectivityService
    {
        bool HasInternet();
    }

    public class ConnectivityService : IConnectivityService
    {
        public bool HasInternet()
        {
            return Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        }
    }
}