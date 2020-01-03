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
            var port = Int32.Parse(Console.ReadLine());
            Console.Write("Specify secret: ");
            var secret = Console.ReadLine();

            using (var client = new UdpClient(address, port))
            {
                var message = System.Text.Encoding.UTF8.GetBytes(secret);
                client.Send(message, message.Length);
                Console.ReadKey();
            }
        }
    }
}
