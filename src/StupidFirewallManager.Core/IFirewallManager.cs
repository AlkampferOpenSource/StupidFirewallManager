using NetFwTypeLib;
using StupidFirewallManager.Common;
using System;
using System.Collections.Generic;
using System.Net;

namespace StupidFirewallManager.Core
{
    /// <summary>
    /// Interface that capture how the program is interfaced
    /// with Windows firewall.
    /// </summary>
    public interface IFirewallManager
    {
        void ApplyBasicTcpRules(FirewallRule[] rules);
        void ApplyUdpRules(FirewallRule[] rules);
        void CheckForExpiredRules(FirewallRule[] rules);
        void OpenPortWithTimeout(int tcpPort, IPEndPoint endpoint, DateTime dateTime);
        IEnumerable<INetFwRule> SearchFirewallRules();
    }
}