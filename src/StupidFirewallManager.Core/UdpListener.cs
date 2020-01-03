using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace StupidFirewallManager.Core
{
	public class UdpListener : IDisposable
	{
		private readonly Dictionary<Int32, UdpClient> _listeners = new Dictionary<int, UdpClient>();
		private bool _stopped = false;

		public void StartListeningOnPort(Int32 port)
		{
			var client = new UdpClient(port);
			var thread = new Thread(Listen);
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
					var stringValue = Encoding.UTF8.GetString(sent);
					OnReceivedMessage(ipe, stringValue, udpData.Port);
				}
				catch (Exception e)
				{
					Serilog.Log.Error(e, "Error receiving udp message");
				}
			}
		}

		private void OnReceivedMessage(IPEndPoint ipe, string stringValue, Int32 port)
		{
			MessageReceived?.Invoke(this, new UdpMessageReceivedEventArgs(ipe, stringValue, port));
		}

		public event EventHandler<UdpMessageReceivedEventArgs> MessageReceived;

		public void Dispose()
		{
			_stopped = true;

			foreach (var udpClient in _listeners.Values)
			{
				udpClient.Close();
			}
		}

		public class UdpMessageReceivedEventArgs : EventArgs 
		{
			public UdpMessageReceivedEventArgs(
				IPEndPoint endpoint, 
				String message,
				Int32 port)
			{
				Endpoint = endpoint;
				Message = message;
				Port = port;
			}

			public IPEndPoint Endpoint { get; }
			public string Message { get; }
			public int Port { get; }
		}

		private struct UdpData 
		{
			public Int32 Port { get; set; }

			public UdpClient Client { get; set; }
		}
	}
}
