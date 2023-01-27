using System.Security.Cryptography;

namespace Maincotech.Common.Security.Signing
{
    public static class SigningKeyPairGenerator
    {
        public static SigningKeyPair Generate()
        {
            var cp = new RSACryptoServiceProvider();
            var privateKey = Convert.ToBase64String(cp.ExportCspBlob(true));
            var publicKey = Convert.ToBase64String(cp.ExportCspBlob(false));
            return new SigningKeyPair(publicKey, privateKey);
        }
    }
}