using System;

namespace StupidFirewallManager.Core
{
    public static class Constants
    {
        public const String SealRulePrefix = "_sfm_block_";
        public const String UdpPrefix = SealRulePrefix + "udp_";
        public const String TcpStaticPrefix = SealRulePrefix + "tcpstatic_";
        public const String TcpRulePrefix = SealRulePrefix + "tcprule_";
    }
}
