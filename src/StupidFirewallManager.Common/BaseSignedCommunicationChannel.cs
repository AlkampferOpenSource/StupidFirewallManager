using Newtonsoft.Json;
using Serilog;
using StupidFirewallManager.Common.Encryption;
using System;
using System.Security;
using System.Text;

namespace StupidFirewallManager.Common
{
    /// <summary>
    /// This is sender class, used to send data to a corresponding receiver
    /// that will need to grab the corresponding public key to verify
    /// if the message arrives from trusted sender.
    /// </summary>
    public abstract class BaseSignedCommunicationChannel :
        ICommunicationChannel,
        ICommunicationChannelReceiver
    {
        protected readonly Configuration _configuration;
        private readonly String _clientName;

        protected BaseSignedCommunicationChannel(
            Configuration configuration,
            String clientName)
        {
            _configuration = configuration;
            _clientName = clientName;
        }

        public event EventHandler<OpenPortRequestEventArgs> OpenPortRequestReceived;

        public bool SendOpenPortRequest(
            OpenPortRequest request,
            string password)
        {
            var key = _configuration.LoadPrivateKey(_clientName);
            var requestInBytes = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(request, Formatting.Indented));
            var signature = Encryptor.Sign(requestInBytes, key, password);
            var dataToTransmit = new TransmissionData(_clientName, requestInBytes, signature);

            var streamToTransmit = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(dataToTransmit, Formatting.Indented));
            OnSendRequest(streamToTransmit);
            return true;
        }

        /// <summary>
        /// This is to be implemented by the concrete channel to really transmit
        /// data to the server
        /// </summary>
        /// <param name="data"></param>
        protected abstract void OnSendRequest(byte[] data);

        /// <summary>
        /// This is to be called from the concrete communication channel.S
        /// </summary>
        /// <param name="receivedData"></param>
        protected void OnRaisePortRequestReceived(byte[] receivedData, string sender)
        {
            var transmissionData = JsonConvert.DeserializeObject<TransmissionData>(
                Encoding.UTF8.GetString(receivedData));
            var loadPublicKey = _configuration.LoadPublicKey(transmissionData.Sender);
            Encryptor.Verify(transmissionData.Request, transmissionData.Signature, loadPublicKey);

            //if we reach here, signature is ok, we can simply proceed
            var request = JsonConvert.DeserializeObject<OpenPortRequest>(
                Encoding.UTF8.GetString(transmissionData.Request));

            //port rule exists?
            var rule = _configuration.GetRuleFromTcpPort(request.PortToOpen);
            if (rule == null)
            {
                Log.Error("Received valid message to open port {port} that has no rule", request.PortToOpen);
                throw new SecurityException("Error");
            }

            OpenPortRequestReceived?.Invoke(this, new OpenPortRequestEventArgs(request));
        }

        private class TransmissionData
        {
            public TransmissionData(string sender, byte[] request, byte[] signature)
            {
                Sender = sender;
                Request = request;
                Signature = signature;
            }

            public string Sender { get; private set; }

            public byte[] Request { get; private set; }

            public byte[] Signature { get; private set; }
        }
    }
}
