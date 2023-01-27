namespace Maincotech.Common.Licensing
{
    /// <summary>
    /// Entry point of fluent API for working with licenses.
    /// </summary>
    public static class LicenseManager
    {
        public static ILicenseBuilder LicenseBuilder => new DefaultLicenseBuilder();
        public static ILicenseVerifierBuilder LicenseVerifierBuilder => new DefaultLicenseVerifierBuilder();
    }
}