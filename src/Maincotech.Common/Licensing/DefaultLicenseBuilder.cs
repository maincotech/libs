using Maincotech.Common.Security.Signing;

namespace Maincotech.Common.Licensing
{
    internal class DefaultLicenseBuilder : ILicenseBuilder
    {
        private DateTime _ExpirationDate = DateTime.MaxValue;
        private Dictionary<string, string> _Properties = new Dictionary<string, string>();
        private string _SerialNumber = SerialNumberGenerator.NoSerialNumber;
        private ISigner? _Signer;

        public SignedLicense Build()
        {
            if (_Signer == null)
            {
                throw new LicenseBuilderException("The signer has not been specified, please specify it with WithSigner method.");
            }
            var license = new SignedLicense(_SerialNumber, DateTime.UtcNow.Date, _ExpirationDate, _Properties);
            license.Sign(_Signer);
            return license;
        }

        public ILicenseBuilder ExpiresOn(DateTime dateTime)
        {
            _ExpirationDate = dateTime;
            return this;
        }

        public ILicenseBuilder WithProperty(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (key.Contains(":"))
                throw new ArgumentException("Character ':' is not allowed in property key.");
            _Properties.Add(key, value);
            return this;
        }

        public ILicenseBuilder WithSerialNumber(string serialNumber)
        {
            _SerialNumber = serialNumber;
            return this;
        }

        public ILicenseBuilder WithSigner(ISigner signer)
        {
            Check.AssertNotNull(signer, nameof(signer));
            _Signer = signer;
            return this;
        }
    }
}