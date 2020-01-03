using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StupidFirewallManager.Core
{
    public class FirewallManager
    {
        public IEnumerable<INetFwRule> SearchFirewallRules()
        {
            INetFwPolicy2 fwPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            return fwPolicy
                .Rules
                .OfType<INetFwRule>()
                .Where(r => r.Name.StartsWith(Constants.SealRulePrefix))
                .ToList();

            //INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            //firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            //firewallRule.Description = "Your rule description";
            //firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // inbound
            //firewallRule.Enabled = true;
            //firewallRule.InterfaceTypes = "All";
            //firewallRule.RemoteAddresses = "1.2.3.0/24"; // add more blocks comma separated
            //firewallRule.Name = "You rule name";
            //firewallPolicy.Rules.Add(firewallRule);
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

            var ruleSample = fwPolicy.Rules.OfType<INetFwRule>().Where(r => r.Name.StartsWith("_")).ToList();

            INetFwRule r1 = ruleSample.First() as INetFwRule;
            INetFwRule2 r2 = ruleSample.First() as INetFwRule2; 
            INetFwRule3 r3 = ruleSample.First() as INetFwRule3; 

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
                firewallRule.Protocol = (int) NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
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
    }
}
