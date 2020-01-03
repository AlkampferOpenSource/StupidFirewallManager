
using Serilog;
using StupidFirewallManager.Core;
using System;

namespace StupidFirewallManager
{
    static class Program
    {
        private static Sealer Sealer { get; set; }

        private static UdpListener UdpListener { get; set; }

        static void Main(string[] args)
        {
            Bootstrapper.Initialize();
            Sealer = new Sealer(Bootstrapper.Configuration);
            Sealer.Seal();

            UdpListener = new UdpListener();
            UdpListener.MessageReceived += UdpMessageReceived;
            foreach (var rule in Bootstrapper.Configuration.Rules)
            {
                UdpListener.StartListeningOnPort(rule.UdpPort);
            }

            Console.WriteLine("Press a key to close.");
            Console.ReadKey();

            UdpListener.Dispose();
        }

        private static void UdpMessageReceived(object sender, UdpListener.UdpMessageReceivedEventArgs e)
        {
            var rule = Bootstrapper.Configuration.GetRuleFromUdpPort(e.Port);
            if (rule == null) 
            {
                Log.Error("Received message from unknown port {port} - Check firewall because it should be closed received from ip {ip}", e.Port, e.Endpoint);
                return;
            }
            Log.Information("Checking udp message in port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Port, rule.TcpPort, e.Endpoint);
            if (rule.Secret != e.Message)
            {
                Log.Error("Received wrong secret in port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Port, rule.TcpPort, e.Endpoint);
                return;
            }

            Log.Information("CORRECT! udp message in port {port} bound to rule for tcp port {tcpport} received from ip {ip}", e.Port, rule.TcpPort, e.Endpoint);
            Sealer.TemporaryOpen(rule.TcpPort, e.Endpoint, DateTime.Now.AddMinutes(100));
        }
    }
}
