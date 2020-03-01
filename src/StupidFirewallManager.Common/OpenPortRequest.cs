using System;
using System.Collections.Generic;
using System.Text;

namespace StupidFirewallManager.Common
{
    /// <summary>
    /// This is the class that will be encrypted and sent
    /// from client to server.
    /// </summary>
    public class OpenPortRequest
    {
        public OpenPortRequest(int portToOpen, DateTime endOpeningDate, string ipAddress)
        {
            PortToOpen = portToOpen;
            EndOpeningDate = endOpeningDate;
            IpAddress = ipAddress;
        }

        /// <summary>
        /// This is the port we want to open
        /// </summary>
        public Int32 PortToOpen { get; private set; }

        /// <summary>
        /// This is the end of opening Date, remember that the
        /// server could leave the port opened for a lesser time
        /// if needed.
        /// </summary>
        public DateTime EndOpeningDate { get; private set; }

        /// <summary>
        /// Ip address to scope port opening to.
        /// </summary>
        public String IpAddress { get; private set; }
    }
}
