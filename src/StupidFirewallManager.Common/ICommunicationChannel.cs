using System;
using System.Collections.Generic;
using System.Text;

namespace StupidFirewallManager.Common
{
    /// <summary>
    /// This is the interface needed to communicate between client and server.
    /// </summary>
    public interface ICommunicationChannel
    {
        Boolean SendOpenPortRequest(OpenPortRequest request, string password);
    }

    /// <summary>
    /// This is the receiver that can get the stream of bytes passed by a 
    /// <see cref="ICommunicationChannel"/> interface and raise 
    /// event if everyting is ok.
    /// </summary>
    public interface ICommunicationChannelReceiver 
    {
        event EventHandler<OpenPortRequestEventArgs> OpenPortRequestReceived;
    }

    public class OpenPortRequestEventArgs : EventArgs
    {
        public OpenPortRequestEventArgs(OpenPortRequest request)
        {
            Request = request;
        }

        public OpenPortRequest Request { get; private set; }
    }
}
