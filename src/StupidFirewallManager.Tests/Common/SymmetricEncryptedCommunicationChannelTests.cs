using NUnit.Framework;
using StupidFirewallManager.Common;
using System;
using System.Security;

namespace StupidFirewallManager.Tests.Common
{
    [TestFixture]
    public class CommunicationChannelTests
    {
        private readonly Configuration _config = new Configuration()
        {
            Rules = new FirewallRule[]
            {
                new FirewallRule("test1", 1000, 1234, "secure password"),
                new FirewallRule("test2", 2000, 2345, "another secure password"),
            }
        };

        [Test]
        public void Verify_basic_send_capabilities()
        {
            var sut = new TestSymmetricCommunicationChannel(_config);
            sut.SendOpenPortRequest(new OpenPortRequest(1234, DateTime.Now, "10.0.0.1"), "secure password");
            Assert.That(sut.SentData, Is.Not.Null);
        }

        [Test]
        public void Verify_send_then_receive()
        {
            DateTime expectedDate = new DateTime(2010, 10, 10);
            var sut = new TestSymmetricCommunicationChannel(_config);
            sut.SendOpenPortRequest(new OpenPortRequest(1234, expectedDate, "10.0.0.1"), "secure password");
            Assert.That(sut.ReceivedRequest.EndOpeningDate, Is.EqualTo(expectedDate));
            Assert.That(sut.ReceivedRequest.PortToOpen, Is.EqualTo(1234));
        }

        [Test]
        public void Unknown_port_generates_security_exception() 
        {
            DateTime expectedDate = new DateTime(2010, 10, 10);
            var sut = new TestSymmetricCommunicationChannel(_config);
            Assert.Throws<SecurityException>(() => 
                sut.SendOpenPortRequest(new OpenPortRequest(555555, expectedDate, "10.0.0.1"), "secure password"));
        }

        [Test]
        public void Wrong_password_generates_security_exception()
        {
            DateTime expectedDate = new DateTime(2010, 10, 10);
            var sut = new TestSymmetricCommunicationChannel(_config);
            Assert.Throws<SecurityException>(() =>
                sut.SendOpenPortRequest(new OpenPortRequest(1234, expectedDate, "10.0.0.1"), "Wrong password"));
        }
    }

    internal class TestSymmetricCommunicationChannel 
    {
        private TestBaseSimmetricEncryptedCommunicationChannelSender _sender;
        private TestBaseSimmetricEncryptedCommunicationChannelReceiver _receiver;

        public Byte[] SentData { get; private set; }

        public OpenPortRequest ReceivedRequest { get; set; }

        public TestSymmetricCommunicationChannel(Configuration configuration) 
        {
            _receiver = new TestBaseSimmetricEncryptedCommunicationChannelReceiver(configuration);
            _receiver.OpenPortRequestReceived += (sender, data) => ReceivedRequest = data.Request;

            _sender = new TestBaseSimmetricEncryptedCommunicationChannelSender(_receiver, d => SentData = d); 
        }

        internal void SendOpenPortRequest(OpenPortRequest openPortRequest, string password)
        {
            _sender.SendOpenPortRequest(openPortRequest, password);
            
        }

        private class TestBaseSimmetricEncryptedCommunicationChannelSender : BaseSimmetricEncryptedCommunicationChannelSender
        {
            private readonly TestBaseSimmetricEncryptedCommunicationChannelReceiver _receiver;
            private readonly Action<byte[]> _sendFunction;

            public TestBaseSimmetricEncryptedCommunicationChannelSender(
                TestBaseSimmetricEncryptedCommunicationChannelReceiver receiver,
                Action<byte[]> sendFunction)
            {
                _receiver = receiver;
                _sendFunction = sendFunction;
            }
            protected override void OnSendRequest(byte[] data)
            {
                _sendFunction(data);
                _receiver.ReceiveData(data);
            }
        }

        private class TestBaseSimmetricEncryptedCommunicationChannelReceiver : BaseSimmetricEncryptedCommunicationChannelReceiver
        {

            public TestBaseSimmetricEncryptedCommunicationChannelReceiver(Configuration configuration) : base(configuration)
            {
            }

            public void ReceiveData(byte[] data) 
            {
                base.OnRaisePortRequestReceived(data, "Local");
            }
        }
    }
}
