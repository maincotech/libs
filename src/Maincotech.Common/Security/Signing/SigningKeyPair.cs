namespace Maincotech.Common.Security.Signing
{
    /// <summary>
    /// Represents a public/private key pair.
    /// </summary>
    public class SigningKeyPair
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="publicKey">
        /// The public key.
        /// </param>
        /// <param name="privateKey">
        /// The private key.
        /// </param>
        public SigningKeyPair(string publicKey, string privateKey)
        {
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }

        /// <summary>
        /// Gets the private key.
        /// </summary>
        public string PrivateKey { get; }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        public string PublicKey { get; }
    }
}