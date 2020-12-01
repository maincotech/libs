using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace System
{
    public class CryptoHelper
    {
        protected RSACryptoServiceProvider RsaProvider = new RSACryptoServiceProvider();

        public string RsaPublicKey
        {
            get { return RsaProvider.ToXmlString(false); }
        }

        public string RsaPrivateKey
        {
            get { return RsaProvider.ToXmlString(true); }
        }

        public string Encrypt(string clearTextString)
        {
            byte[] encryptedStr;
            encryptedStr = RsaProvider.Encrypt(Encoding.ASCII.GetBytes(clearTextString), false);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i <= encryptedStr.Length - 1; i++)
            {
                if (i != encryptedStr.Length - 1)
                {
                    stringBuilder.Append(encryptedStr[i] + "~");
                }
                else
                {
                    stringBuilder.Append(encryptedStr[i]);
                }
            }
            return stringBuilder.ToString();
        }

        public string Decrypt(string encryptedString)
        {
            var decryptedStr = RsaProvider.Decrypt(StringToByteArray(encryptedString.Trim()), false);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i <= decryptedStr.Length - 1; i++)
            {
                stringBuilder.Append(Convert.ToChar(decryptedStr[i]));
            }
            return stringBuilder.ToString();
        }

        public byte[] StringToByteArray(string inputString)
        {
            string[] s;
            s = inputString.Trim().Split('~');
            var b = new byte[s.Length];

            for (var i = 0; i <= s.Length - 1; i++)
            {
                b[i] = Convert.ToByte(s[i]);
            }
            return b;
        }

        public static string Hash(string cleanString)
        {
            if (cleanString != null)
            {
                var clearBytes = new UnicodeEncoding().GetBytes(cleanString);

                var hashedBytes
                    = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);

                return BitConverter.ToString(hashedBytes);
            }
            return string.Empty;
        }

        //

        // TODO: move to config, should not be hard coded
        private static readonly byte[] _key192 = {10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10};

        private static readonly byte[] _iv128 = {10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
                10, 10, 10, 10};

        public static string EncryptRijndaelManaged(string value)
        {
            if (value == string.Empty) return string.Empty;

            var crypto = new RijndaelManaged();
            var memoryStream = new MemoryStream();

            var cryptoStream = new CryptoStream(
                memoryStream,
                crypto.CreateEncryptor(_key192, _iv128),
                CryptoStreamMode.Write);

            var streamWriter = new StreamWriter(cryptoStream);

            streamWriter.Write(value);
            streamWriter.Flush();
            cryptoStream.FlushFinalBlock();
            memoryStream.Flush();

            return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public static string DecryptRijndaelManaged(string value)
        {
            if (value == string.Empty) return string.Empty;

            var crypto = new RijndaelManaged();
            var memoryStream = new MemoryStream(Convert.FromBase64String(value));

            var cryptoStream = new CryptoStream(
                memoryStream,
                crypto.CreateDecryptor(_key192, _iv128),
                CryptoStreamMode.Read);

            var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }

        public string SignAndSecureData(string value)
        {
            return SignAndSecureData(new[] { value });
        }

        public string SignAndSecureData(string[] values)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<x></x>");

            for (var i = 0; i < values.Length; i++)
            {
                AddNode(xmlDoc, "v" + i, values[i]);
            }

            var signature = RsaProvider.SignData(Encoding.ASCII.GetBytes(xmlDoc.InnerXml),
                "SHA1");

            AddNode(xmlDoc, "s", Convert.ToBase64String(signature, 0, signature.Length));
            return EncryptRijndaelManaged(xmlDoc.InnerXml);
        }

        public bool DecryptAndVerifyData(string input, out string[] values)
        {
            var xml = DecryptRijndaelManaged(input);

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            values = null;

            var node = xmlDoc.GetElementsByTagName("s")[0];
            node.ParentNode.RemoveChild(node);

            var signature = Convert.FromBase64String(node.InnerText);

            var data = Encoding.ASCII.GetBytes(xmlDoc.InnerXml);
            if (!RsaProvider.VerifyData(data, "SHA1", signature))
                return false;

            int count;
            for (count = 0; count < 100; count++)
            {
                if (xmlDoc.GetElementsByTagName("v" + count)[0] == null)
                    break;
            }

            values = new string[count];

            for (var i = 0; i < count; i++)
                values[i] = xmlDoc.GetElementsByTagName("v" + i)[0].InnerText;

            return true;
        }

        private static void AddNode(XmlDocument xmlDoc, string name, string content)
        {
            var elem = xmlDoc.CreateElement(name);
            var text = xmlDoc.CreateTextNode(content);
            xmlDoc.DocumentElement.AppendChild(elem);
            xmlDoc.DocumentElement.LastChild.AppendChild(text);
        }
    }
}