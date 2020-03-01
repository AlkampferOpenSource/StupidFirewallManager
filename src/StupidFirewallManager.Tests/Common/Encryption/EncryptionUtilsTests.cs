using NUnit.Framework;
using StupidFirewallManager.Common.Encryption;
using System.Security.Cryptography;

namespace StupidFirewallManager.Tests.Common.Encryption
{
    [TestFixture]
    public class EncryptionUtilsTests
    {
        [Test]
        public void Derivation_of_keys_is_stable()
        {
            var salt = EncryptionUtils.GenerateRandomSalt();
            byte[] key1, key2;
            byte[] iv1, iv2;
            using (var pdb = new PasswordDeriveBytes("test", salt))
            {
                key1 = pdb.GetBytes(32);
                iv1 = pdb.GetBytes(16);
            }

            using (var pdb = new PasswordDeriveBytes("test", salt))
            {
                key2 = pdb.GetBytes(32);
                iv2 = pdb.GetBytes(16);
            }

            Assert.That(key1, Is.EquivalentTo(key2));
            Assert.That(iv1, Is.EquivalentTo(iv2));
        }

        [Test]
        public void Different_salt_generates_different_keys()
        {
            var salt1 = EncryptionUtils.GenerateRandomSalt();
            var salt2 = EncryptionUtils.GenerateRandomSalt();
            byte[] key1, key2;
            byte[] iv1, iv2;
            using (var pdb = new PasswordDeriveBytes("test", salt1))
            {
                key1 = pdb.GetBytes(32);
                iv1 = pdb.GetBytes(16);
            }

            using (var pdb = new PasswordDeriveBytes("test", salt2))
            {
                key2 = pdb.GetBytes(32);
                iv2 = pdb.GetBytes(16);
            }

            Assert.That(key1, Is.Not.EquivalentTo(key2));
            Assert.That(iv1, Is.Not.EquivalentTo(iv2));
        }

        [Test]
        public void Salt_is_random()
        {
            var salt1 = EncryptionUtils.GenerateRandomSalt();
            var salt2 = EncryptionUtils.GenerateRandomSalt();

            Assert.That(salt1, Is.Not.EquivalentTo(salt2));
        }
    }
}