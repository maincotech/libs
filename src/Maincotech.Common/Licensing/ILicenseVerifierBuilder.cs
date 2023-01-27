using Maincotech.Common.Security.Signing;

namespace Maincotech.Common.Licensing
{
    public interface ILicenseVerifierBuilder
    {
        LicenseVerifier Build();

        ILicenseVerifierBuilder WithAdditionalVerifier(ILicenseVerifyRule verifier);

        ILicenseVerifierBuilder WithSerialNumberTemplate(string template);

        ILicenseVerifierBuilder WithSigner(ISigner signer);
    }
}