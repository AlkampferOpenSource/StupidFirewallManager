using NUnit.Framework;
using StupidFirewallManager.Common;
using StupidFirewallManager.Core;

namespace StupidFirewallManager.Tests.Explicit
{
    //[TestFixture]
    [Explicit]
    public class ComFirewallInterfaceTests
    {
        //[Test]
        [Explicit]
        public void CanExtractRules()
        {
            FirewallManager sut = new FirewallManager();
            Assert.DoesNotThrow(() => sut.SearchFirewallRules());
        }

        //[Test]
        [Explicit]
        public void Can_add_simple_udp_rule()
        {
            FirewallManager sut = new FirewallManager();
            var rules = new FirewallRule[]
            {
                new FirewallRule("test", 1000, 1000, "any")
            };
            sut.ApplyUdpRules(rules);
        }
    }
}
