using Maincotech.Common.Security;
using System.Text;

namespace Maincotech.Common.Licensing
{
    public class SerialNumberGenerator
    {
        internal const string _DefaultTemplate = "SN-####-####-####-####";
        internal static readonly string NoSerialNumber = "NO_SERIAL_NO";
        private const char Placeholder = '#';
        private const string Seperator = "-";

        /// <summary>
        ///
        /// </summary>
        /// <param name="template">Example: "SN-####-####-####-####" the last segment will be used as checksum placeholder.</param>
        public SerialNumberGenerator(string template = _DefaultTemplate)
        {
            Template = template;
        }

        public string Template { get; set; }

        public static void Verify(string template, string serialNumber)
        {
            Check.AssertNotNullOrEmpty(template, nameof(template));
            Check.AssertNotNullOrEmpty(serialNumber, nameof(serialNumber));
            var templateFragments = template.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            var fragments = serialNumber.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);

            Check.Assert(templateFragments.Length == fragments.Length, $"The specified serialnumber '{serialNumber}' is not valid for the template '{template}'");
            for (int i = 0; i < templateFragments.Length; i++)
            {
                Check.Assert(templateFragments[i].Length == fragments[i].Length, $"The specified serialnumber '{serialNumber}' is not valid for the template '{template}'");
            }

            //Check check sum
            var expectedCheckSum = fragments.Last();
            var sn = string.Join(Seperator, fragments, 0, fragments.Length - 1);
            var checkSum = CheckSumGenerator.GetByFixedLength(sn, expectedCheckSum.Length);
            Check.Assert(expectedCheckSum == checkSum, $"The specified serialnumber '{serialNumber}' is not valid for the template '{template}'");
        }

        public string Generate()
        {
            var fragments = Template.Split(Seperator, StringSplitOptions.RemoveEmptyEntries);
            var tokensLength = new List<int>();
            var tokens = new List<string>();
            var checkSumLength = 0;
            var formateBuilder = new StringBuilder();
            var tokenIndex = 0;
            for (int i = 0; i < fragments.Length; i++)
            {
                var fragment = fragments[i];
                if (i == fragments.Length - 1)
                {
                    checkSumLength = fragment.Length;
                    break;
                }
                if (fragment.Any(x => x != Placeholder) == false)
                {
                    tokensLength.Add(fragment.Length);
                    formateBuilder.Append("{" + tokenIndex++ + "}");
                }
                else
                {
                    formateBuilder.Append(fragment);
                }
                if (i < fragments.Length - 1)
                {
                    formateBuilder.Append(Seperator);
                }
            }
            foreach (var len in tokensLength)
            {
                var randomNumber = new RandomNumber(len);
                var number = randomNumber.Get();
                tokens.Add(number.ToString().PadLeft(len, '0'));
            }

            var sn = string.Format(formateBuilder.ToString(), tokens.ToArray());
            if (checkSumLength > 0)
            {
                sn += CheckSumGenerator.GetByFixedLength(sn, checkSumLength);
            }
            return sn;
        }
    }
}