using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidFirewallManager.Core
{
    public class Configuration
    {
        public FirewallRule[] Rules { get; set; }
    }

    public class FirewallRule
    {
        [JsonConstructor]
        private FirewallRule()
        {

        }

        public FirewallRule(string name, int udpPort, int tcpPort, string secret)
        {
            Name = name;
            UdpPort = udpPort;
            TcpPort = tcpPort;
            Secret = secret;
        }

        public String Name { get; set; }

        public Int32 UdpPort { get; set; }

        public Int32 TcpPort { get; set; }

        public String Secret { get; set; }
    }
}
