using Maincotech.Common.Security.Signing;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace Maincotech.Common.Licensing
{
    /// <summary>
    /// Class that encapsulates some license related information and a signature for verifying it
    /// </summary>
    public sealed class SignedLicense
    {
        private const char InvalidChar = ':';

        private SignedLicense(string serialNumber, DateTime issueDate, DateTime expirationDate, IDictionary<string, string> properties, string signature)
        {
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            SerialNumber = serialNumber ?? "";
            var dict = properties ?? new Dictionary<string, string>();
            if (dict.Keys.Any(key => key.Contains(InvalidChar)))
                throw new FormatException($"Character '{InvalidChar}' is not allowed in property key.");
            Signature = signature;
            Properties = new ReadOnlyDictionary<string, string>(dict);
        }
        internal SignedLicense(string serialNumber, DateTime issueDate, DateTime expirationDate, IDictionary<string, string> properties)
            : this(serialNumber, issueDate, expirationDate, properties, string.Empty)
        {
         
        }

        //  Properties
        // ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Date of the expiration (may be <see cref="DateTime.MaxValue"/> for licenses without expiration)
        /// </summary>
        public DateTime ExpirationDate { get; }

        /// <summary>
        /// Gets a value that indicates if the license expires.
        /// </summary>
        public bool HasExpirationDate => ExpirationDate != DateTime.MaxValue;

        /// <summary>
        /// Gets a value that indicates if the license has a serial number.
        /// </summary>
        public bool HasSerialNumber => SerialNumber.IsNotNullOrEmpty() && SerialNumber != Maincotech.Common.Licensing.SerialNumberGenerator.NoSerialNumber;

        /// <summary>
        /// Date of the issuing (when the license was created)
        /// </summary>
        public DateTime IssueDate { get; }

        /// <summary>
        /// List of custom key value pairs that are part of the license.
        /// </summary>
        public IDictionary<string, string> Properties { get; }

        /// <summary>
        /// Optional: A serial number (See also <see cref="SerialNumber"/>)
        /// </summary>
        public string SerialNumber { get; }

        private string Signature { get; set; }
        //  Methods
        // ////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Serializes the license as encrypted base64 encoded text.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return CryptoHelper.EncryptRijndaelManaged(SerializeAsPlainText());
        }
        internal void Sign(ISigner signer)
        {
            var sb = new StringBuilder();

            WriteLicenseProperties(sb);

            Signature = signer.Sign(sb.ToString());
        }

        internal void VerifySignature(ISigner signer)
        {
            var sb = new StringBuilder();

            WriteLicenseProperties(sb);

            if (!signer.Verify(sb.ToString(), Signature))
            {
                ThrowInvalidSignatureException();
            }
        }
        // - Load
        internal static SignedLicense? Deserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException($"'{nameof(content)}' must not null or empty.");
            }
            content = content.Unwrap();
            content = CryptoHelper.DecryptRijndaelManaged(content);
            var lines = (content ?? "").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length < 4)
            {
                ThrowInvalidFormatException();
            }
            return ReadLicenseFile(lines);
        }

        /// <summary>
        /// Serializes the license as plain text (license information are readable by humans).
        /// </summary>
        /// <returns></returns>
        internal string SerializeAsPlainText()
        {
            var sb = new StringBuilder();
            WriteLicenseProperties(sb);
            WriteSignature(sb);
            return sb.ToString();
        }

        private static KeyValuePair<string, string> GetKeyValuePair(string line)
        {
            var index = line.IndexOf(':');
            if (index < 0)
            {
                ThrowInvalidFormatException();
            }
            var key = line.Substring(0, index);
            var value = line.Substring(index + 1);
            return new KeyValuePair<string, string>(key, value);
        }

        private static SignedLicense? ReadLicenseFile(string[] lines)
        {
            try
            {
                var index = 0;
                var serialNumber = lines[index++];
                var issueDate = DateTime.Parse(lines[index++], CultureInfo.InvariantCulture);
                var expirationDate = DateTime.Parse(lines[index++], CultureInfo.InvariantCulture);
                var signature = lines.Last();

                var properties = new Dictionary<string, string>();
                foreach (var line in lines.Skip(index).Take(lines.Length - (index + 1)))
                {
                    var pair = GetKeyValuePair(line);
                    properties.Add(pair.Key, pair.Value);
                }

                return new SignedLicense(serialNumber, issueDate, expirationDate, properties, signature);
            }
            catch
            {
                ThrowInvalidFormatException();
                return null;
            }
        }

        // - Throw helper
        private static void ThrowInvalidFormatException()
        {
            var msg = "License file has not a valid format.";
            throw new SignedLicenseException(msg);
        }

        private static void ThrowInvalidSignatureException()
        {
            var msg = "Signature of license file is not valid.";
            throw new SignedLicenseException(msg);
        }

        private static void ThrowNotSignedException()
        {
            var msg = "License file is not signed.";
            throw new SignedLicenseException(msg);
        }

        private void WriteLicenseProperties(StringBuilder sb)
        {
            sb.AppendLine(SerialNumber);
            sb.AppendLine(IssueDate.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine(ExpirationDate.ToString(CultureInfo.InvariantCulture));
            foreach (var property in Properties)
            {
                sb.AppendLine(property.Key + ":" + property.Value);
            }
        }

        private void WriteSignature(StringBuilder sb)
        {
            if (string.IsNullOrEmpty(Signature))
            {
                ThrowNotSignedException();
            }
            sb.Append(Signature); // note: Append because it is the last line
        }
    }
}