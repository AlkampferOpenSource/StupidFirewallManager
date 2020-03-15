using Microsoft.Extensions.Configuration;
using StupidFirewallManager.Common.Encryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StupidFirewallManager.Common
{
    public class Configuration
    {
        public void Bind(IConfiguration configuration)
        {
            configuration.Bind(this);

            List<FirewallRule> rulesList = new List<FirewallRule>();
            var rules = configuration.GetSection("Rules").GetChildren().ToList();
            foreach (var rule in rules)
            {
                var firewallRule = new FirewallRule();
                rule.Bind(firewallRule);
                rulesList.Add(firewallRule);
            }

            Rules = rulesList.ToArray();
        }

        public FirewallRule[] Rules { get; set; }

        public FirewallRule GetRuleFromUdpPort(int port)
        {
            return Rules.FirstOrDefault(r => r.UdpPort == port);
        }

        public FirewallRule GetRuleFromTcpPort(int port)
        {
            return Rules.FirstOrDefault(r => r.TcpPort == port);
        }

        public string CertificatesBaseFolder { get; set; }

        public void SaveKey(string clientName, AsymmetricKey key)
        {
            File.WriteAllBytes(GetPrivateFileName(clientName), key.PrivateKey);
            File.WriteAllBytes(GetSaltFileName(clientName), key.Salt);
            File.WriteAllBytes(GetPublicFileName(clientName), key.PublicKey);
        }

        public AsymmetricKey LoadPrivateKey(string clientName)
        {
            var privateKey = File.ReadAllBytes(GetPrivateFileName(clientName));
            var salt = File.ReadAllBytes(GetSaltFileName(clientName));
            var publicKey = File.ReadAllBytes(GetPublicFileName(clientName));

            return new AsymmetricKey(salt, publicKey, privateKey);
        }

        public byte[] LoadPublicKey(string clientName)
        {
            return File.ReadAllBytes(GetPublicFileName(clientName));
        }

        private string GetPrivateFileName(string clientName)
        {
            return Path.Combine(CertificatesBaseFolder, $"{clientName}.privatekey");
        }

        private string GetPublicFileName(string clientName)
        {
            return Path.Combine(CertificatesBaseFolder, $"{clientName}.publickey");
        }

        private string GetSaltFileName(string clientName)
        {
            return Path.Combine(CertificatesBaseFolder, $"{clientName}.salt");
        }
    }

    public class FirewallRule
    {
        internal FirewallRule()
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
