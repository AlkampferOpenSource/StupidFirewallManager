using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidFirewallManager.Core
{
    public class Sealer
    {
        private readonly Configuration _configuration;
        private readonly FirewallManager _firewallManager;

        public Sealer(Configuration configuration)
        {
            _configuration = configuration;
            _firewallManager = new FirewallManager();
        }

        public void Seal()
        {
            _firewallManager.ApplyUdpRules(_configuration.Rules);
        }
    }
}
