
using Serilog;
using StupidFirewallManager.Core;
using System;

namespace StupidFirewallManager
{
    static class Program
    {
        private static UdpListener UdpListener { get; set; }

        static void Main(string[] args)
        {
            Bootstrapper.Initialize();
            Sealer sealer = new Sealer(Bootstrapper.Configuration);
            sealer.Seal();

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
            Log.Information("Received UDP message {msg} from client {ip} in port {port}", e.Message, e.Endpoint.ToString(), e.Port);
        }
    }
}
