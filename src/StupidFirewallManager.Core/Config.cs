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
        public String Name { get; set; }

        public Int32 UdpPort { get; set; }

        public Int32 TcpPort { get; set; }

        public String Secret { get; set; }
    }
}
