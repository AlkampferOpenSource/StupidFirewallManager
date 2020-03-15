using StupidFirewallManager.Common;
using System;
using System.Net;
using System.Threading;

namespace StupidFirewallManager.Core
{
    public class Sealer
    {
        private readonly Configuration _configuration;
        private readonly FirewallManager _firewallManager;
        private readonly Timer _timer;

        public Sealer(Configuration configuration)
        {
            _configuration = configuration;
            _firewallManager = new FirewallManager();
            _timer = new Timer(CheckCallback, null, 1000, 1000 * 60 * 10);
        }

        private void CheckCallback(object state)
        {
            _firewallManager.CheckForExpiredRules(_configuration.Rules);
        }

        public void Seal()
        {
            _firewallManager.ApplyUdpRules(_configuration.Rules);
            _firewallManager.ApplyBasicTcpRules(_configuration.Rules);
        }

        public void OpenPortWithTimeout(int tcpPort, IPEndPoint endpoint, DateTime dateTime)
        {
            _firewallManager.OpenPortWithTimeout(tcpPort, endpoint, dateTime);
        }
    }
}
