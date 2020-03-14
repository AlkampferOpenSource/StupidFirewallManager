using Serilog;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Security;
using System.Security.Cryptography;

namespace StupidFirewallManager.Common.Encryption
{
    public static class Encryptor
    {
        /// <summary>
        /// Encrypt with AES using a key generated from a pre-shared password and a salt
        /// that is different for each request, to avoid key reusing.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Byte[] SimmetricEncrypt(string password, byte[] salt, byte[] data)
        {
            using var aes = Aes.Create();
            using var encryptor = aes.GetEncryptorFromPassword(password, salt);
            using MemoryStream msEncrypt = new MemoryStream();

            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            {
                csEncrypt.Write(data, 0, data.Length);
            }

            // important, dispose CryptoStream before accessing the array
            return msEncrypt.ToArray();
        }

        public static Byte[] SimmetricDecrypt(string password, byte[] salt, byte[] data)
        {
            try
            {
                using var aes = Aes.Create();
                using var encryptor = aes.GetDecryptorFromPassword(password, salt);
                using MemoryStream msDecrypt = new MemoryStream();
                using MemoryStream msOriginalData = new MemoryStream(data);

                using (CryptoStream csDecrypt = new CryptoStream(msOriginalData, encryptor, CryptoStreamMode.Read))
                {
                    csDecrypt.CopyTo(msDecrypt);
                }

                // important, dispose CryptoStream before accessing the array
                return msDecrypt.ToArray();
            }
            catch (CryptographicException cex)
            {
                Log.Error(cex, "Error decrypting message");
                //Do not disclose anything to the caller.
                throw new SecurityException("Error in decrypting");
            }
        }

        public static AsymmetricKey GenerateAsymmetricKeys(string password)
        {
            using (var rsa = RSA.Create(2048))
            {
                var salt = EncryptionUtils.GenerateRandomSalt();
                var encryptedPrivateKey = SimmetricEncrypt(password, salt, rsa.ExportRSAPrivateKey());
                return new AsymmetricKey(salt, rsa.ExportRSAPublicKey(), encryptedPrivateKey);
            }
        }

        public static byte[] Sign(byte[] arrayToSign, AsymmetricKey key, string password)
        {
            using var rsa = RSA.Create();
            var privateKey = SimmetricDecrypt(password, key.Salt, key.PrivateKey);
            rsa.ImportRSAPublicKey(key.PublicKey, out _);
            rsa.ImportRSAPrivateKey(privateKey, out _);
            return rsa.SignData(arrayToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        public static void Verify(byte[] arrayToVerify, byte[] signature, byte[] publicKey)
        {
            using var rsa = RSA.Create();

            rsa.ImportRSAPublicKey(publicKey, out _);
            var verificationResult = rsa.VerifyData(
                arrayToVerify,
                signature,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            if (!verificationResult)
            {
                throw new SecurityException("Invaid signature");
            }
        }
    }
}


