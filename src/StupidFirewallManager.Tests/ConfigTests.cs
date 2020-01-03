using Newtonsoft.Json;
using NUnit.Framework;
using StupidFirewallManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StupidFirewallManager.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void Basic_deserialization() 
        {
            const string testValue = @"
{
  ""Rules"" : [
    {
                ""Name"": ""Rdp"",
      ""UdpPort"": 23456,
      ""TcpPort"": 3389,
      ""Secret"": ""this_is_a_secret""
    }
  ]
}";
            var config = JsonConvert.DeserializeObject<Configuration>(testValue);
            Assert.That(config.Rules.Length, Is.EqualTo(1));
            var rule = config.Rules[0];
            Assert.That(rule.Name, Is.EqualTo("Rdp"));
            Assert.That(rule.UdpPort, Is.EqualTo(23456));
            Assert.That(rule.TcpPort, Is.EqualTo(3389));
            Assert.That(rule.Secret, Is.EqualTo("this_is_a_secret"));
        }

        [Test]
        public void Basic_deserialization_of_multiple_rules()
        {
            const string testValue = @"
{
  ""Rules"" : [
    {
                ""Name"": ""Rdp"",
      ""UdpPort"": 23456,
      ""TcpPort"": 3389,
      ""Secret"": ""this_is_a_secret""
    },
    {
      ""Name"": ""test"",
      ""UdpPort"": 1,
      ""TcpPort"": 2,
      ""Secret"": ""x""
    }
  ]
}";
            var config = JsonConvert.DeserializeObject<Configuration>(testValue);
            Assert.That(config.Rules.Length, Is.EqualTo(2));
            var rule = config.Rules[1];
            Assert.That(rule.Name, Is.EqualTo("test"));
            Assert.That(rule.UdpPort, Is.EqualTo(1));
            Assert.That(rule.TcpPort, Is.EqualTo(2));
            Assert.That(rule.Secret, Is.EqualTo("x"));
        }
    }
}
