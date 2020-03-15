using Newtonsoft.Json;
using StupidFirewallManager.Common.Encryption;
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace StupidFirewallManager.Common
{
    /// <summary>
    /// A simmetric encryptor can easily implement both sender bot receiver
    /// in this simple first version.
    ///
    /// TODO: Split this class in two, one for receive, the other one to send.
    /// </summary>
    public abstract class BaseSimmetricEncryptedCommunicationChannelReceiver :
        ICommunicationChannelReceiver
    {
        protected readonly Configuration _configuration;

        protected BaseSimmetricEncryptedCommunicationChannelReceiver(Configuration configuration)
        {
            _configuration = configuration;
        }

        public event EventHandler<OpenPortRequestEventArgs> OpenPortRequestReceived;

        /// <summary>
        /// This is to be called from the concrete communication channel.S
        /// </summary>
        /// <param name="receivedData"></param>
        protected void OnRaisePortRequestReceived(byte[] receivedData, string sender)
        {
            Int32 port = 0;
            try
            {
                var ms = new MemoryStream(receivedData);
                using BinaryReader sr = new BinaryReader(ms);

                var salt = sr.ReadBytes(EncryptionUtils.saltSize);
                port = sr.ReadInt32();
                var encryptedText = sr.ReadBytes(1000);

                //now we should try to decrypt the data.
                var portRule = _configuration.GetRuleFromTcpPort(port);
                if (portRule == null)
                {
                    throw new SecurityException($"Unable to find rule for requested port {port}");
                }
                var decrypted = Encryptor.SimmetricDecrypt(portRule.Secret, salt, encryptedText);
                var decryptedDeserializedText = Encoding.UTF8.GetString(decrypted);
                var request = JsonConvert.DeserializeObject<OpenPortRequest>(decryptedDeserializedText);
                OpenPortRequestReceived?.Invoke(this, new OpenPortRequestEventArgs(request));
            }
            catch (CryptographicException)
            {
                throw new SecurityException($"Wrong password received trying to access port {port} from {sender}");
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Malformed packets received from {sender} - {ex}");
            }
        }
    }

    public abstract class BaseSimmetricEncryptedCommunicationChannelSender : ICommunicationChannel
    {
        protected BaseSimmetricEncryptedCommunicationChannelSender()
        {
        }

        public bool SendOpenPortRequest(OpenPortRequest request, string password)
        {
            //ok we will send the request directly encrypting with the password
            var serializedRequest = Encoding.UTF8.GetBytes(
                JsonConvert.SerializeObject(request, Formatting.Indented));

            //need to encrypt with the password, remember also to generate a salt
            //of 8 bytes that will be added to the stream.
            var salt = EncryptionUtils.GenerateRandomSalt();
            var encrypted = Encryptor.SimmetricEncrypt(password, salt, serializedRequest);

            var ms = new MemoryStream();
            using BinaryWriter sw = new BinaryWriter(ms);

            sw.Write(salt);
            sw.Write(request.PortToOpen);
            sw.Write(encrypted);

            OnSendRequest(ms.ToArray());
            return true;
        }

        /// <summary>
        /// This is to be implemented by the concrete channel to really transmit
        /// data to the server
        /// </summary>
        /// <param name="data"></param>
        protected abstract void OnSendRequest(byte[] data);
    }
}
