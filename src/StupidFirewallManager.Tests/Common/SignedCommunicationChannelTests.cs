using NUnit.Framework;
using StupidFirewallManager.Common;
using StupidFirewallManager.Common.Encryption;
using System;
using System.IO;
using System.Security;

namespace StupidFirewallManager.Tests.Common
{
    [TestFixture]
    public class SignedCommunicationChannelTests
    {
        public const string ClientName = "Test Client";

        private readonly Configuration _config = new Configuration()
        {
            Rules = new FirewallRule[]
            {
                new FirewallRule("test1", 1000, 1234, "secure password"),
                new FirewallRule("test2", 2000, 2345, "another secure password"),
            },
            CertificatesBaseFolder = Path.GetTempPath(),
        };

        private AsymmetricKey _key;

        [OneTimeSetUp]
        public void OneTimeSetUp() 
        {
            _key = Encryptor.GenerateAsymmetricKeys("SignPassword");
            _config.SaveKey(ClientName, _key);
        }

        [Test]
        public void Verify_basic_send_capabilities()
        {
            var sut = new TestSignedCommunicationChannel(_config);
            sut.SendOpenPortRequest(new OpenPortRequest(1234, DateTime.Now, "10.0.0.1"), "SignPassword");
            Assert.That(sut.SentData, Is.Not.Null);
        }

        [Test]
        public void Verify_send_then_receive()
        {
            DateTime expectedDate = new DateTime(2010, 10, 10);
            var sut = new TestSignedCommunicationChannel(_config);
            sut.SendOpenPortRequest(new OpenPortRequest(1234, expectedDate, "10.0.0.1"), "SignPassword");
            Assert.That(sut.ReceivedRequest.EndOpeningDate, Is.EqualTo(expectedDate));
            Assert.That(sut.ReceivedRequest.PortToOpen, Is.EqualTo(1234));
        }

        [Test]
        public void Unknown_port_generates_security_exception()
        {
            DateTime expectedDate = new DateTime(2010, 10, 10);
            var sut = new TestSignedCommunicationChannel(_config);
            Assert.Throws<SecurityException>(() =>
                sut.SendOpenPortRequest(new OpenPortRequest(555555, expectedDate, "10.0.0.1"), "SignPassword"));
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

    internal class TestSignedCommunicationChannel : BaseSignedCommunicationChannel
    {
        public Byte[] SentData { get; private set; }

        public OpenPortRequest ReceivedRequest { get; set; }

        public TestSignedCommunicationChannel(
            Configuration configuration) :
            base(configuration, SignedCommunicationChannelTests.ClientName)
        {
            base.OpenPortRequestReceived += DataReceived;
        }

        private void DataReceived(object sender, OpenPortRequestEventArgs e)
        {
            ReceivedRequest = e.Request;
        }

        protected override void OnSendRequest(byte[] data)
        {
            SentData = data;
            //immediately short circuit received data for test.S
            base.OnRaisePortRequestReceived(data, "Local");
        }
    }
}
