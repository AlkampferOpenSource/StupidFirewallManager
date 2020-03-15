
using Serilog;
using StupidFirewallManager.Common;
using StupidFirewallManager.Core;
using System;
using System.Net;

namespace StupidFirewallManager
{
    static class Program
    {
        private static Sealer Sealer { get; set; }

        private static SymmetricEncryptedUdpCommunicationChannelReceiver Channel { get; set; }

        static void Main(string[] args)
        {
            Bootstrapper.Initialize();
            Sealer = new Sealer(Bootstrapper.Configuration);
            Sealer.Seal();

            Channel = new SymmetricEncryptedUdpCommunicationChannelReceiver(Bootstrapper.Configuration);
            Channel.OpenPortRequestReceived += Channel_OpenPortRequestReceived;
            
            foreach (var rule in Bootstrapper.Configuration.Rules)
            {
                Channel.StartListeningOnPort(rule.UdpPort);
            }

            Console.WriteLine("Press a key to close.");
            Console.ReadKey();
        }

        private static void Channel_OpenPortRequestReceived(object sender, OpenPortRequestEventArgs e)
        {
            var rule = Bootstrapper.Configuration.GetRuleFromTcpPort(e.Request.PortToOpen);
            if (rule == null)
            {
                Log.Error("Received message from unknown port {port} - Check firewall because it should be closed received from ip {ip}", e.Request.PortToOpen, e.Request.IpAddress);
                return;
            }
            Log.Information("Checking udp message in port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Request.PortToOpen, rule.TcpPort, e.Request.IpAddress);
            if (rule.TcpPort != e.Request.PortToOpen)
            {
                Log.Error("Received wrong requesto to open port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Request.PortToOpen, rule.TcpPort, e.Request.IpAddress);
                return;
            }

            Log.Information("CORRECT! udp message in port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Request.PortToOpen, rule.TcpPort, e.Request.IpAddress);
            Sealer.OpenPortWithTimeout(rule.TcpPort, IPEndPoint.Parse(e.Request.IpAddress), new DateTime(Math.Min(e.Request.EndOpeningDate.Ticks, DateTime.Now.AddHours(4).Ticks)));
        }
    }
}
