using NUnit.Framework;
using StupidFirewallManager.Common.Encryption;
using System.Security;
using System.Text;

namespace StupidFirewallManager.Tests.Common.Encryption
{
    [TestFixture]
    public class AsymmetricEncryptorTests
    {
        private const string _password = "PasswordForPrivateKey";
        private const string _badPassword = "BadPassword";

        [Test]
        public void Can_generate_key_with_private_encrypted()
        {
            var key = Encryptor.GenerateAsymmetricKeys(_password);
            Assert.DoesNotThrow(() => Encryptor.SimmetricDecrypt(_password, key.Salt, key.PrivateKey));
        }

        [Test]
        public void Asymmetric_sign_data()
        {
            byte[] arrayToSign = Encoding.UTF8.GetBytes("a simple string");
            var key = Encryptor.GenerateAsymmetricKeys(_password);
            var signature = Encryptor.Sign(arrayToSign, key, _password);
            Assert.DoesNotThrow(() => Encryptor.Verify(arrayToSign, signature, key.PublicKey));
        }

        [Test]
        public void Without_password_cannot_use_private_key()
        {
            byte[] arrayToSign = Encoding.UTF8.GetBytes("a simple string");
            var key = Encryptor.GenerateAsymmetricKeys(_password);
            Assert.Throws<SecurityException>(() => Encryptor.Sign(arrayToSign, key, _badPassword));
        }

        [Test]
        public void Tampered_data_Should_throw()
        {
            byte[] arrayToSign = Encoding.UTF8.GetBytes("a simple string");
            var key = Encryptor.GenerateAsymmetricKeys(_password);
            var signature = Encryptor.Sign(arrayToSign, key, _password);
            arrayToSign[0] = arrayToSign[1];
            Assert.Throws<SecurityException>(() => Encryptor.Verify(arrayToSign, signature, key.PublicKey));
        }
    }
}
