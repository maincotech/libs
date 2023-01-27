using Maincotech.Common.Security.Signing;

namespace Maincotech.Common.Licensing
{
    public sealed class LicenseVerifier
    {
        private readonly List<ILicenseVerifyRule> _AdditionalRules;
        private readonly string _SerialNumberTemplate;
        private readonly ISigner _Signer;

        internal LicenseVerifier(ISigner signer, string serialNumberTemplate, IEnumerable<ILicenseVerifyRule> rules)
        {
            _Signer = signer;
            _SerialNumberTemplate = serialNumberTemplate;
            _AdditionalRules = new List<ILicenseVerifyRule>(rules);
        }

        public SignedLicense LoadAndVerify(string licenseString)
        {
            var license = SignedLicense.Deserialize(licenseString);
            // verify signature
            license!.VerifySignature(_Signer);

            // verify expiration date
            if (license.HasExpirationDate && license.ExpirationDate < DateTime.UtcNow)
            {
                throw new SignedLicenseException
                            ($"License has been expired since '{license.ExpirationDate}'.");
            }
            // verify serial number
            if (license.HasSerialNumber && _SerialNumberTemplate.IsNotNullOrEmpty())
            {
                SerialNumberGenerator.Verify(_SerialNumberTemplate, license.SerialNumber);
            }
            return license;
        }
    }
}