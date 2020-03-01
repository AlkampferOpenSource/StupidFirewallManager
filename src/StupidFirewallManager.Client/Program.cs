using StupidFirewallManager.Common;
using System;
using System.Net.Sockets;

namespace StupidFirewallManager.Client
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Specify Address: ");
            var address = Console.ReadLine();
            Console.Write("Specify Udp port: ");
            var udpPort = Int32.Parse(Console.ReadLine());

            Console.Write("Specify what TCP port you want to open: ");
            var tcpPort = Int32.Parse(Console.ReadLine());

            Console.Write("Specify publicIp to open: ");
            var ipAddress = Console.ReadLine();

            var secret = ConsoleHelper.AskPassword("Specify secret:");

            var channel = new SymmetricEncryptedUdpCommunicationChannelSender(address, udpPort);
            channel.SendOpenPortRequest(new OpenPortRequest(tcpPort, DateTime.UtcNow.AddHours(1), ipAddress), secret);

            Console.ReadKey();
        }
    }
}
