using NUnit.Framework;
using StupidFirewallManager.Common.Encryption;
using System.Security;
using System.Text;

namespace StupidFirewallManager.Tests.Common.Encryption
{
    [TestFixture]
    public class SimmetricEncryptorTests
    {
        [Test]
        public void Verify_encryption_then_encryption()
        {
            const string testString = "This is a test string";
            var data = Encoding.UTF8.GetBytes(testString);
            var password = "an extremely secure passwrd";
            var salt = EncryptionUtils.GenerateRandomSalt();
            var encrypted = Encryptor.SimmetricEncrypt(password, salt, data);

            var decrypted = Encryptor.SimmetricDecrypt(password, salt, encrypted);
            var decryptedString = Encoding.UTF8.GetString(decrypted);
            Assert.That(decryptedString, Is.EqualTo(testString));
        }

        [Test]
        public void Bad_key_generates_exception()
        {
            const string testString = "This is a test string";
            var data = Encoding.UTF8.GetBytes(testString);
            var password = "an extremely secure passwrd";
            var salt = EncryptionUtils.GenerateRandomSalt();
            var encrypted = Encryptor.SimmetricEncrypt(password, salt, data);

            Assert.Throws<SecurityException>(() => Encryptor.SimmetricDecrypt(password + "hello", salt, encrypted));
        }

        [Test]
        public void Bad_salt_generates_exception()
        {
            const string testString = "This is a test string";
            var data = Encoding.UTF8.GetBytes(testString);
            var password = "an extremely secure passwrd";
            var salt = EncryptionUtils.GenerateRandomSalt();
            var wrongSalt = EncryptionUtils.GenerateRandomSalt();
            var encrypted = Encryptor.SimmetricEncrypt(password, salt, data);

            Assert.Throws<SecurityException>(() => Encryptor.SimmetricDecrypt(password + "hello", wrongSalt, encrypted));
        }
    }
}
