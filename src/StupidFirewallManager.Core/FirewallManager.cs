using NetFwTypeLib;
using Serilog;
using StupidFirewallManager.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
namespace StupidFirewallManager.Core
{
    public class FirewallManager : IFirewallManager
    {
        public IEnumerable<INetFwRule> SearchFirewallRules()
        {
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            return fwPolicy
                .Rules
                .OfType<INetFwRule>()
                .Where(r => r.Name.StartsWith(Constants.SealRulePrefix))
                .ToList();
        }

        /// <summary>
        /// Create a series of block rules for udp to left open only the port
        /// specified in <paramref name="rules"/>
        /// </summary>
        /// <param name="rules"></param>
        public void ApplyUdpRules(FirewallRule[] rules)
        {
            var ports = rules.Select(r => r.UdpPort).ToArray();
            var portRanges = PortRangeHelper.GetRangeExclusive(ports);
            //now get already existing rules.
            var allUdpRules = SearchFirewallRules()
                .Where(r => r.Name.StartsWith(Constants.UdpPrefix));
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            //we need to create other rules, and be 100% sure that only new rules for udp are created
            var newRules = portRanges
                .Select(range => new
                {
                    Name = $"{Constants.UdpPrefix}_{range.LowerPortInclusive}_{range.UpperPortInclusive}",
                    Range = range
                })
                .ToList();

            var rulesToRemove = allUdpRules.Where(r => !newRules.Any(nr => nr.Name == r.Name)).ToList();
            foreach (var ruleToRemove in rulesToRemove)
            {
                fwPolicy.Rules.Remove(ruleToRemove.Name);
            }
            var rulesToAdd = newRules.Where(r => !allUdpRules.Any(ur => ur.Name == r.Name)).ToList();
            foreach (var ruleToAdd in rulesToAdd)
            {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.Description = "Created by StupidFirewallManager";
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.RemoteAddresses = "*"; // add more blocks comma separated
                firewallRule.LocalAddresses = "*";
                firewallRule.Name = ruleToAdd.Name;
                firewallRule.LocalPorts = $"{ruleToAdd.Range.LowerPortInclusive}-{ruleToAdd.Range.UpperPortInclusive}";
                fwPolicy.Rules.Add(firewallRule);
            }
        }

        /// <summary>
        /// Open a certain port, name of the rule includes timeout of opening port.
        /// </summary>
        /// <param name="tcpPort"></param>
        /// <param name="endpoint"></param>
        /// <param name="dateTime"></param>
        public void OpenPortWithTimeout(int tcpPort, IPEndPoint endpoint, DateTime dateTime)
        {
            var ruleName = $"{Constants.TcpRulePrefix}{dateTime.ToString("yyyyMMddHHmm")}_{Guid.NewGuid().ToString()}";
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Description = "Created by StupidFirewallManager";
            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.RemoteAddresses = endpoint.Address.ToString();
            firewallRule.LocalAddresses = "*";
            firewallRule.Name = ruleName;
            firewallRule.LocalPorts = tcpPort.ToString();
            fwPolicy.Rules.Add(firewallRule);
        }

        /// <summary>
        /// This is slightly different from <see cref="ApplyUdpRules(FirewallRule[])"/> but it 
        /// could be made equal.
        /// should be expired.
        /// </summary>
        /// <param name="rules"></param>
        public void ApplyBasicTcpRules(FirewallRule[] rules)
        {
            var ports = rules.Select(r => r.TcpPort).ToArray();
            var portRanges = PortRangeHelper.GetRangeExclusive(ports);
            //now get already existing rules.
            var allStaticTcpRule = SearchFirewallRules()
                .Where(r => r.Name.StartsWith(Constants.TcpStaticPrefix));

            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            //we need to create other rules, and be 100% sure that only new rules for udp are created
            var newRules = portRanges
                .Select(range => new
                {
                    Name = $"{Constants.TcpStaticPrefix}_{range.LowerPortInclusive}_{range.UpperPortInclusive}",
                    Range = range
                })
                .ToList();

            var rulesToRemove = allStaticTcpRule.Where(r => !newRules.Any(nr => nr.Name == r.Name)).ToList();
            foreach (var ruleToRemove in rulesToRemove)
            {
                fwPolicy.Rules.Remove(ruleToRemove.Name);
            }
            var rulesToAdd = newRules.Where(r => !allStaticTcpRule.Any(ur => ur.Name == r.Name)).ToList();
            foreach (var ruleToAdd in rulesToAdd)
            {
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.Description = "Created by StupidFirewallManager";
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.RemoteAddresses = "*"; // add more blocks comma separated
                firewallRule.LocalAddresses = "*";
                firewallRule.Name = ruleToAdd.Name;
                firewallRule.LocalPorts = $"{ruleToAdd.Range.LowerPortInclusive}-{ruleToAdd.Range.UpperPortInclusive}";
                fwPolicy.Rules.Add(firewallRule);
            }
        }

        public void CheckForExpiredRules(FirewallRule[] rules)
        {
            Log.Debug("Check for expired rules");
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            var temporaryRules = fwPolicy
                .Rules
                .OfType<INetFwRule>()
                .Where(r => r.Name.StartsWith(Constants.TcpRulePrefix))
                .ToList();

            foreach (var rule in temporaryRules)
            {
                if (DateTimeEncodingHelper.TryParse(rule.Name, out var dateTime))
                {
                    //Indeed this is a rule made by this tool, check for exipiration
                    if (DateTime.UtcNow.Subtract(dateTime).TotalSeconds > 0)
                    {
                        Log.Information("About to remove rule {name} because it is expired", rule.Name);
                        fwPolicy.Rules.Remove(rule.Name);
                    }
                }
            }

            //now check for other rules that allows traffic on that port
            var allTcpRules = fwPolicy
                 .Rules
                 .OfType<INetFwRule>()
                 .Where(r => r.InterfaceTypes == "All" || r.InterfaceTypes == "Tcp")
                 .ToList();
            var controlledTcpPorts = rules.Select(r => r.TcpPort).ToList();
            foreach (var tcpRule in allTcpRules)
            {
                if (PortRangeHelper.RangeContainsPort(tcpRule.LocalPorts, controlledTcpPorts))
                {
                    //lets only one custom port to exists
                    if (!tcpRule.Name.StartsWith(Constants.SealRulePrefix))
                    {
                        Log.Information("About to remove rule {name} because it is based on a controlled port", tcpRule.Name);
                        fwPolicy.Rules.Remove(tcpRule.Name);
                    }
                }
            }
        }
    }
}
