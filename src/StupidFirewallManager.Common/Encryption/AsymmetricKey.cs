namespace StupidFirewallManager.Common.Encryption
{
    /// <summary>
    /// Simple class to store private and public key.
    /// </summary>
    public class AsymmetricKey
    {
        public AsymmetricKey(byte[] salt, byte[] publicKey, byte[] privateKey)
        {
            Salt = salt;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        public byte[] Salt { get; private set; }
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }
    }
}
