using Maincotech.Common.Security.Signing;
using System.Security.Cryptography;

namespace Maincotech.Common.Licensing
{
    public static class ILicenseVerifierBuilderExtensions
    {
        public static ILicenseVerifierBuilder WithDefaultSerialNumberTemplate(this ILicenseVerifierBuilder builder)
        {
            return builder.WithSerialNumberTemplate(SerialNumberGenerator._DefaultTemplate);
        }

        /// <summary>
        /// Uses RSA algorithm for verifying the signature of a signed license with the specified private key.
        /// </summary>
        /// <param name="signer">
        /// The fluent API object to extend.
        /// </param>
        /// <param name="base64EncodedCsbBlobKey">
        /// The base64 encoded CSB BLOB key that contains the public key.
        /// </param>
        /// <returns>
        /// The next fluent API object.
        /// </returns>
        public static ILicenseVerifierBuilder WithRsaPublicKey(this ILicenseVerifierBuilder builder, string base64EncodedCsbBlobKey)
        {
            return builder.WithRsaPublicKey(Convert.FromBase64String(base64EncodedCsbBlobKey));
        }

        /// <summary>
        /// Uses RSA algorithm for verifying the signature of a signed license with the specified private key.
        /// </summary>
        /// <param name="signer">
        /// The fluent API object to extend.
        /// </param>
        /// <param name="csbBlobKey">
        /// The CSB BLOB key that contains the public key.
        /// </param>
        /// <returns>
        /// The next fluent API object.
        /// </returns>
        public static ILicenseVerifierBuilder WithRsaPublicKey(this ILicenseVerifierBuilder builder, byte[] csbBlobKey)
        {
            var rsaSigner = new RsaSigner(csbBlobKey);
            return builder.WithSigner(rsaSigner);
        }

        /// <summary>
        /// Uses RSA algorithm for verifying the signature of a signed license with the specified private key.
        /// </summary>
        /// <param name="signer">
        /// The fluent API object to extend.
        /// </param>
        /// <param name="rsaParameters">
        /// The <see cref="RSAParameters" /> object that contains the public key.
        /// </param>
        /// <returns>
        /// The next fluent API object.
        /// </returns>
        public static ILicenseVerifierBuilder WithRsaPublicKey(this ILicenseVerifierBuilder builder, RSAParameters rsaParameters)
        {
            var rsaSigner = new RsaSigner(rsaParameters);
            return builder.WithSigner(rsaSigner);
        }
    }
}