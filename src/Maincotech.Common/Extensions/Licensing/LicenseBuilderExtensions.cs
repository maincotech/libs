using Maincotech.Common.Security.Signing;
using System.Security.Cryptography;

namespace Maincotech.Common.Licensing
{
    public static class LicenseBuilderExtensions
    {
        public static ILicenseBuilder ExpiresIn(this ILicenseBuilder builder, TimeSpan timeSpan)
        {
            return builder.ExpiresOn(DateTime.Now.Date + timeSpan);
        }

        public static ILicenseBuilder WithDefaultSerialNumberTemplate(this ILicenseBuilder builder)
        {
            var serialNumber = new SerialNumberGenerator();
            return builder.WithSerialNumber(serialNumber.Generate());
        }

        public static ILicenseBuilder WithRsaPrivateKey(this ILicenseBuilder builder, string base64EncodedCsbBlobKey)
        {
            var rsaSigner = new RsaSigner(base64EncodedCsbBlobKey);
            builder.WithSigner(rsaSigner);
            return builder;
        }

        public static ILicenseBuilder WithRsaPrivateKey(this ILicenseBuilder builder, byte[] csbBlobKey)
        {
            var rsaSigner = new RsaSigner(csbBlobKey);
            builder.WithSigner(rsaSigner);
            return builder;
        }

        public static ILicenseBuilder WithRsaPrivateKey(this ILicenseBuilder builder, RSAParameters rsaParameters)
        {
            var rsaSigner = new RsaSigner(rsaParameters);
            builder.WithSigner(rsaSigner);
            return builder;
        }

        public static ILicenseBuilder WithSerialNumberTemplate(this ILicenseBuilder builder, string template)
        {
            var serialNumber = new SerialNumberGenerator(template);
            return builder.WithSerialNumber(serialNumber.Generate());
        }
    }
}