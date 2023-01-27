using System.Security.Cryptography;
using System.Text;

namespace Maincotech.Common.Security
{
    public static class CheckSumGenerator
    {
        private static readonly char[] DefaultSupportedCharacters = "0123456789ABCDEF".ToArray();

        public static string GetByFixedLength(string data, int length, char[]? supportedCharacters = null)
        {
            Check.Require(data, nameof(data), Check.NotNullOrEmpty);
            if (supportedCharacters == null)
            {
                supportedCharacters = DefaultSupportedCharacters;
            }

            var checkSum = new CheckSum(supportedCharacters, length).Create(Encoding.UTF8.GetBytes(data));
            return checkSum;
        }

        public static byte[] GetByFixedLength(byte[] data, int length, char[]? supportedCharacters = null)
        {
            Check.Require(data, nameof(data), Check.NotNullOrEmpty);
            if (supportedCharacters == null)
            {
                supportedCharacters = DefaultSupportedCharacters;
            }
            var checkSum = new CheckSum(supportedCharacters, length).Create(data);
            return Encoding.UTF8.GetBytes(checkSum);
        }

        public static string GetByMD5(string text)
        {
            if (text == null)
            {
                return string.Empty;
            }

            byte[] message = Encoding.UTF8.GetBytes(text);
            byte[] hashValue = GetByMD5(message);

            string hashString = string.Empty;
            foreach (byte x in hashValue)
            {
                hashString += string.Format("{0:x2}", x);
            }

            return hashString;
        }

        public static byte[] GetByMD5(byte[] message)
        {
            MD5 hashString = MD5.Create();
            return hashString.ComputeHash(message);
        }

        internal class CheckSum
        {
            private readonly char[] _ValidCharacters;

            /// <summary>
            /// Creates a new instance of the class.
            /// </summary>
            /// <param name="supportedCharacters">
            /// List of supported characters for the check sum.
            /// </param>
            /// <param name="length">
            /// The length of the check sum.
            /// </param>
            public CheckSum(char[] supportedCharacters, int length)
            {
                Check.Require(supportedCharacters, nameof(supportedCharacters), Check.NotNullOrEmpty);
                Check.Require(length, nameof(length), Check.GreaterThan(0));
                _ValidCharacters = supportedCharacters.Where(x => x != ' ').Distinct().ToArray();
                Check.Require(_ValidCharacters, nameof(supportedCharacters), Check.NotNullOrEmpty);
                Length = length;
            }

            /// <summary>
            /// Gets the length of the check sum.
            /// </summary>
            public int Length { get; }

            /// <summary>
            /// Creates the check sum for the specified byte array.
            /// </summary>
            /// <param name="bytes">
            /// The byte array to create the check sum for.
            /// </param>
            /// <returns>
            /// A string that represents the check sum of the specified byte array.
            /// </returns>
            public string Create(byte[] bytes)
            {
                bytes = AdjustSize(bytes, Length);

                var checkSum = bytes.Select(ToCheckSumChar).ToArray();

                return new string(checkSum);
            }

            private static byte[] AdjustSize(byte[] bytes, int length)
            {
                if (bytes.Length > length)
                    return ReduceSize(bytes, length);
                if (bytes.Length < length)
                    return InflateSize(bytes, length);
                return bytes;
            }

            private static byte[] InflateSize(byte[] bytes, int length)
            {
                var result = new byte[length];
                for (int i = 0; i < length; i++)
                    result[i] ^= bytes[i % bytes.Length];
                return result;
            }

            private static byte[] ReduceSize(byte[] bytes, int length)
            {
                var result = new byte[length];
                for (int i = 0; i < bytes.Length; i++)
                    result[i % length] ^= bytes[i];
                return result;
            }

            private char ToCheckSumChar(byte b)
            {
                return _ValidCharacters[b % _ValidCharacters.Length];
            }
        }
    }
}