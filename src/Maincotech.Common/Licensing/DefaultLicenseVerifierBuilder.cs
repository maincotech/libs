using Maincotech.Common.Security.Signing;

namespace Maincotech.Common.Licensing
{
    internal class DefaultLicenseVerifierBuilder : ILicenseVerifierBuilder
    {
        private List<ILicenseVerifyRule> _AdditionalRules = new List<ILicenseVerifyRule>();
        private string? _SerialNumberTemplate;
        private ISigner? _Signer;

        public LicenseVerifier Build()
        {
            Check.AssertNotNull(_Signer, nameof(_Signer));
            Check.AssertNotNull(_SerialNumberTemplate, nameof(_SerialNumberTemplate));
            return new LicenseVerifier(_Signer!, _SerialNumberTemplate!, _AdditionalRules);
        }

        public ILicenseVerifierBuilder WithAdditionalVerifier(ILicenseVerifyRule verifier)
        {
            _AdditionalRules.Add(verifier);
            return this;
        }

        public ILicenseVerifierBuilder WithSerialNumberTemplate(string template)
        {
            _SerialNumberTemplate = template;
            return this;
        }

        public ILicenseVerifierBuilder WithSigner(ISigner signer)
        {
            _Signer = signer;
            return this;
        }
    }
}