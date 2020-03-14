using System;
using System.Security.Cryptography;

namespace StupidFirewallManager.Common.Encryption
{
    public static class EncryptionUtils
    {
        public const Int32 saltSize = 8;

        public static byte[] GenerateRandomSalt()
        {
            using var csp = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltSize];
            csp.GetBytes(salt);
            return salt;
        }

        public static ICryptoTransform GetEncryptorFromPassword(
            this Aes aes,
            string password,
            byte[] salt)
        {
            using var pdb = new PasswordDeriveBytes(password, salt);
            var key = pdb.GetBytes(32);
            var IV = pdb.GetBytes(16);
            return aes.CreateEncryptor(key, IV);
        }

        public static ICryptoTransform GetDecryptorFromPassword(
            this Aes aes,
            string password,
            byte[] salt)
        {
            using var pdb = new PasswordDeriveBytes(password, salt);
            var key = pdb.GetBytes(32);
            var IV = pdb.GetBytes(16);
            return aes.CreateDecryptor(key, IV);
        }
    }
}
