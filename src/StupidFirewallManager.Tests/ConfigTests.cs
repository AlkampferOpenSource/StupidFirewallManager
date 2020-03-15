using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using StupidFirewallManager.Common;
using StupidFirewallManager.Common.Encryption;
using System;
using System.IO;
using System.Text;

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
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(testValue));
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonStream(ms)
                .Build();

            var config = new Configuration();
            config.Bind(configuration);

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

            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(testValue));
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonStream(ms)
                .Build();

            var config = new Configuration();
            config.Bind(configuration);

            var rule = config.Rules[1];
            Assert.That(rule.Name, Is.EqualTo("test"));
            Assert.That(rule.UdpPort, Is.EqualTo(1));
            Assert.That(rule.TcpPort, Is.EqualTo(2));
            Assert.That(rule.Secret, Is.EqualTo("x"));
        }

        [Test]
        public void Can_save_and_reload_symmetric_key()
        {
            Configuration _config = new Configuration()
            {
                CertificatesBaseFolder = Path.GetTempPath(),
            };
            var key = Encryptor.GenerateAsymmetricKeys("HelloPwd");
            var id = Guid.NewGuid().ToString();
            _config.SaveKey(id, key);
            var reloaded = _config.LoadPrivateKey(id);
            Assert.That(reloaded.PrivateKey, Is.EquivalentTo(key.PrivateKey));
            Assert.That(reloaded.PublicKey, Is.EquivalentTo(key.PublicKey));
            Assert.That(reloaded.Salt, Is.EquivalentTo(key.Salt));
        }

        [Test]
        public void Can_overwrite_key()
        {
            Configuration _config = new Configuration()
            {
                CertificatesBaseFolder = Path.GetTempPath(),
            };
            var key = Encryptor.GenerateAsymmetricKeys("HelloPwd");
            var id = Guid.NewGuid().ToString();
            _config.SaveKey(id, key);

            var key2 = Encryptor.GenerateAsymmetricKeys("HelloPwd");
            _config.SaveKey(id, key2);

            var reloaded = _config.LoadPrivateKey(id);
            Assert.That(reloaded.PrivateKey, Is.EquivalentTo(key2.PrivateKey));
            Assert.That(reloaded.PublicKey, Is.EquivalentTo(key2.PublicKey));
            Assert.That(reloaded.Salt, Is.EquivalentTo(key2.Salt));
        }
    }
}
