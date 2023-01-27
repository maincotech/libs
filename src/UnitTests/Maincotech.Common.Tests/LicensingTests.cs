using Maincotech.Common.Licensing;
using System.Globalization;

namespace Maincotech.Common.Tests
{
    [TestClass]
    public class LicensingTests
    {
        // Private
        internal static string PrivateKey = "BwIAAACkAABSU0EyAAQAAAEAAQA9D9kxlsh1N5KPmhOsJj/QM9iZVaho5OzEG0gyO8s0Giycx7ttegjugLE1NY7Gw5FPJvSqrlRiBp9iNCsD9/NUJIa65mwfTsShzoce+v5tRLJrd4osZZ3WFA/e4oSk9BgCJNUWIShj1HKD4Lk1YqGWtaMZnx/uNLe8QZ4FGYkKvOWDl4FaLViZBbGfLBxMoMpPGQVmSbJtlOoqjyQr0J9stuuJCs564uTzXqJU9/ytInlFYGEDOpYanlkio4x38Px5WAF4+EPvplW6IszdwsR+Sd6hkSqwu8IPzkZwU6PsyvPF2tLQgomB4LVh/6gJcDpNCXJLWXm4GG+YuHpiCFG+6fPFbd0vcDc5Y4ByAtUADFQ0q2kdI+8K5znNJBd9xeuTF9mJLFKbvENJ58F+DPCtWLEWf5tYZXicZUfTa/tnmzFQKx1lc3wHYh5DyQttkmvsN4bXrp0whYU1S8eiI22H5meA5C6CiJZKSXWEGAAA2bpSgRt5ltS1WIR+wVGZ5iFkwnrkQldBZxecWCpj5HQexyDPcpsJipSotRllkNP8K8TubP8YafQGntQjeozZ4mOz2+V3f7GuC5DX229fGIv54ULq7ftWS3sqLSYzf1DJbN//bQkZQCAP5s5UXlx3n6G68cmkTaSE4ZP2slleqF6CEAaMS99jh2deYslQu1Jp395XkYoqnkzWiAIuoIYIaUxMn5+HWdqS6gqyGVHdbnWvlncdcoest/0waxkeSC86QqZ77QFzLaKSfDnYpHZ2t1o=";

        // Public
        internal static string PublicKey = "BgIAAACkAABSU0ExAAQAAAEAAQA9D9kxlsh1N5KPmhOsJj/QM9iZVaho5OzEG0gyO8s0Giycx7ttegjugLE1NY7Gw5FPJvSqrlRiBp9iNCsD9/NUJIa65mwfTsShzoce+v5tRLJrd4osZZ3WFA/e4oSk9BgCJNUWIShj1HKD4Lk1YqGWtaMZnx/uNLe8QZ4FGYkKvA==";

        [TestMethod]
        public void TestLicenseBuilder()
        {
            var expireDate = DateTime.Parse("2100-01-01", CultureInfo.InvariantCulture);
            var license = LicenseManager.LicenseBuilder
                .WithRsaPrivateKey(PrivateKey)
                .WithSerialNumber("CDE")
                .ExpiresOn(expireDate)
                .WithProperty("Prop1", "Value1")
                .WithProperty("Prop2", "Value2")
                .Build();

            Assert.IsNotNull(license);
            Assert.AreEqual("CDE", license.SerialNumber);
            Assert.AreEqual(expireDate, license.ExpirationDate);
            Assert.AreEqual(DateTime.UtcNow.Date, license.IssueDate);
            Assert.AreEqual(2, license.Properties.Count);
            Assert.AreEqual("Value1", license.Properties["Prop1"]);
            Assert.AreEqual("Value2", license.Properties["Prop2"]);
        }

        [TestMethod]
        public void TestLicenseVerifierBuilder01()
        {
            var expireDate = DateTime.Parse("2100-01-01", CultureInfo.InvariantCulture);
            var license = LicenseManager.LicenseBuilder
                .WithRsaPrivateKey(PrivateKey)
                .WithSerialNumber("CDE")
                .ExpiresOn(expireDate)
                .WithProperty("Prop1", "Value1")
                .WithProperty("Prop2", "Value2")
                .Build();

            var licenseString = license.Serialize();
            var verifier = LicenseManager.LicenseVerifierBuilder
                .WithRsaPublicKey(PublicKey)
                .Build();
            var verifiedLicense = verifier.LoadAndVerify(licenseString);

            Assert.IsNotNull(verifiedLicense);
            Assert.AreEqual("CDE", verifiedLicense.SerialNumber);
            Assert.AreEqual(expireDate, verifiedLicense.ExpirationDate);
            Assert.AreEqual(DateTime.UtcNow.Date, verifiedLicense.IssueDate);
            Assert.AreEqual(2, verifiedLicense.Properties.Count);
            Assert.AreEqual("Value1", verifiedLicense.Properties["Prop1"]);
            Assert.AreEqual("Value2", verifiedLicense.Properties["Prop2"]);
        }

        [TestMethod]
        public void TestLicenseVerifierBuilder02()
        {
            var expireDate = DateTime.Parse("2100-01-01", CultureInfo.InvariantCulture);
            var license = LicenseManager.LicenseBuilder
                .WithRsaPrivateKey(PrivateKey)
                .WithDefaultSerialNumberTemplate()
                .ExpiresOn(expireDate)
                .WithProperty("Prop1", "Value1")
                .WithProperty("Prop2", "Value2")
                .Build();

            var licenseString = license.Serialize();
            var verifier = LicenseManager.LicenseVerifierBuilder
                .WithRsaPublicKey(PublicKey)
                .WithDefaultSerialNumberTemplate()
                .Build();
            var verifiedLicense = verifier.LoadAndVerify(licenseString);

            Assert.IsNotNull(verifiedLicense);
            Assert.IsNotNull(verifiedLicense.SerialNumber);
            Assert.AreEqual(expireDate, verifiedLicense.ExpirationDate);
            Assert.AreEqual(DateTime.UtcNow.Date, verifiedLicense.IssueDate);
            Assert.AreEqual(2, verifiedLicense.Properties.Count);
            Assert.AreEqual("Value1", verifiedLicense.Properties["Prop1"]);
            Assert.AreEqual("Value2", verifiedLicense.Properties["Prop2"]);
        }


        [TestMethod]
        [ExpectedException(typeof(SignedLicenseException))]
        public void TestLicenseVerifierBuilder03()
        {
            var expireDate = DateTime.Parse("1900-01-01", CultureInfo.InvariantCulture);
            var license = LicenseManager.LicenseBuilder
                .WithRsaPrivateKey(PrivateKey)
                .WithDefaultSerialNumberTemplate()
                .ExpiresOn(expireDate)
                .WithProperty("Prop1", "Value1")
                .WithProperty("Prop2", "Value2")
                .Build();

            var licenseString = license.Serialize();
            var verifier = LicenseManager.LicenseVerifierBuilder
                .WithRsaPublicKey(PublicKey)
                .WithDefaultSerialNumberTemplate()
                .Build();
            var verifiedLicense = verifier.LoadAndVerify(licenseString);

         
        }
    }
}