using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StupidFirewallManager.Common
{
    public class SymmetricEncryptedUdpCommunicationChannelReceiver : BaseSimmetricEncryptedCommunicationChannelReceiver, IDisposable
    {
        public SymmetricEncryptedUdpCommunicationChannelReceiver(Configuration configuration) : base(configuration)
        {
        }

        private readonly Dictionary<Int32, UdpClient> _listeners = new Dictionary<int, UdpClient>();
        private bool _stopped = false;

        public void StartListeningOnPort(Int32 port)
        {
            var client = new UdpClient(port);
            var thread = new Thread(Listen);
            _listeners.Add(port, client);
            thread.Start(new UdpData { Client = client, Port = port });
        }

        private void Listen(object state)
        {
            UdpData udpData = (UdpData)state;
            while (true)
            {
                if (_stopped)
                    break;

                try
                {
                    IPEndPoint ipe = null;
                    byte[] sent = udpData.Client.Receive(ref ipe);
                    base.OnRaisePortRequestReceived(sent, ipe.Address.ToString());
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e, "Error receiving udp message");
                }
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(Boolean dispose)
        {
            if (disposed)
                return;

            if (dispose)
            {
                _stopped = true;

                foreach (var udpClient in _listeners.Values)
                {
                    udpClient.Close();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private struct UdpData
        {
            public Int32 Port { get; set; }

            public UdpClient Client { get; set; }
        }
    }

    public class SymmetricEncryptedUdpCommunicationChannelSender : BaseSimmetricEncryptedCommunicationChannelSender
    {
        private readonly string _serverAddress;
        private readonly int _port;
        
        public SymmetricEncryptedUdpCommunicationChannelSender(String serverAddress, Int32 port) : base()
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        protected override void OnSendRequest(byte[] data)
        {
            using var client = new UdpClient(_serverAddress, _port);
            client.Send(data, data.Length);
        }
    }
}
