using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensionsValidators
    {
        #region [IsAlpha]

        private static readonly Regex _isAlphaRegex = new Regex(RegexPattern.Alpha, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified eval string contains only alpha characters.
        /// </summary>
        /// <param name="evalString">The eval string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified eval string is alpha; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlpha(this string evalString)
        {
            return _isAlphaRegex.IsMatch(evalString);
        }

        #endregion [IsAlpha]

        #region [IsAlphaNumeric]

        private static readonly Regex _isAlphaNumericRegex = new Regex(RegexPattern.AlphaNumeric, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified eval string contains only alphanumeric characters
        /// </summary>
        /// <param name="evalString">The eval string.</param>
        /// <returns>
        /// 	<c>true</c> if the string is alphanumeric; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAlphaNumeric(this string evalString)
        {
            return _isAlphaNumericRegex.IsMatch(evalString);
        }

        #endregion [IsAlphaNumeric]

        #region [IsNumeric]

        private static readonly Regex _isNumericRegex = new Regex(RegexPattern.Numeric, RegexOptions.Compiled);

        public static bool IsNumeric(this string inputString)
        {
            var m = _isNumericRegex.Match(inputString);
            return m.Success;
        }

        #endregion [IsNumeric]

        #region IsAbsolutePhysicalPath

        private static bool IsDirectorySeparatorChar(char ch)
        {
            if (ch != '\\')
            {
                return (ch == '/');
            }
            return true;
        }

        internal static bool IsUncSharePath(string path)
        {
            return (((path.Length > 2) && IsDirectorySeparatorChar(path[0])) && IsDirectorySeparatorChar(path[1]));
        }

        public static bool IsAbsolutePhysicalPath(this string path)
        {
            if ((path == null) || (path.Length < 3))
            {
                return false;
            }
            return (((path[1] == ':') && IsDirectorySeparatorChar(path[2])) || IsUncSharePath(path));
        }

        #endregion IsAbsolutePhysicalPath

        #region IsAppRelativePath

        public static bool IsAppRelativePath(this string path)
        {
            if (path == null)
            {
                return false;
            }
            var length = path.Length;
            if (length == 0)
            {
                return false;
            }
            if (path[0] != '~')
            {
                return false;
            }
            if ((length != 1) && (path[1] != '\\'))
            {
                return (path[1] == '/');
            }
            return true;
        }

        #endregion IsAppRelativePath

        #region [IsEmailAddress]

        private static readonly Regex _isValidEmailRegex = new Regex(RegexPattern.Email, RegexOptions.Compiled);

        /// <summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmailAddress(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;

            return _isValidEmailRegex.IsMatch(s);
        }

        #endregion [IsEmailAddress]

        #region [IsGuid]

        private static readonly Regex _isGuidRegex = new Regex(RegexPattern.Guid, RegexOptions.Compiled);

        public static bool IsGuid(this string candidate)
        {
            var isValid = false;

            if (!string.IsNullOrEmpty(candidate))
            {
                isValid = _isGuidRegex.IsMatch(candidate);
            }
            return isValid;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="candidate"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public static bool IsGuid(this string candidate, out Guid output)
        {
            var isValid = false;
            output = Guid.Empty;
            if (candidate.IsGuid())
            {
                isValid = true;
                output = new Guid(candidate);
            }
            return isValid;
        }

        #endregion [IsGuid]

        #region [IsIPAddress]

        private static readonly Regex _isIpAddressRegex = new Regex(RegexPattern.IpAddress, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified string is a valid IP address.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <returns>
        /// 	<c>true</c> if valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIpAddress(this string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return false;
            return _isIpAddressRegex.IsMatch(ipAddress);
        }

        #endregion [IsIPAddress]

        #region [IsLowerCase]

        private static readonly Regex _isLowerCaseRegex = new Regex(RegexPattern.LowerCase, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified string is lower case.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is lower case; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLowerCase(this string inputString)
        {
            return _isLowerCaseRegex.IsMatch(inputString);
        }

        #endregion [IsLowerCase]

        #region [IsUpperCase]

        private static readonly Regex _isUpperRegex = new Regex(RegexPattern.UpperCase, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified string is upper case.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is upper case; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUpperCase(this string inputString)
        {
            return _isUpperRegex.IsMatch(inputString);
        }

        #endregion [IsUpperCase]

        #region [IsUrl]

        private static readonly Regex _isUrlRegex = new Regex(RegexPattern.Url, RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified string is url.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <returns>
        /// 	<c>true</c> if the specified string is url; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUrl(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return false;
            return _isUrlRegex.IsMatch(inputString);
        }

        #endregion [IsUrl]

        #region [IsHasChinese]

        private static readonly Regex _isHasChineseRegex = new Regex(RegexPattern.HasChinese, RegexOptions.Compiled);

        public static bool IsHasChinese(this string inputString)
        {
            var m = _isHasChineseRegex.Match(inputString);
            return m.Success;
        }

        #endregion [IsHasChinese]

        #region [IsChineseLetter]

        /// <summary>
        /// 在unicode 字符串中，中文的范围是在4E00..9FFF:CJK Unified Ideographs。通过对字符的unicode编码进行判断来确定字符是否为中文。
        /// </summary>
        /// <remarks>http://blog.csdn.net/qiujiahao/archive/2007/08/09/1733169.aspx</remarks>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsChineseLetter(this string input, int index)
        {
            var code = 0;
            var chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
            var chend = Convert.ToInt32("9fff", 16);
            if (input != "")
            {
                code = char.ConvertToUtf32(input, index);    //获得字符串input中指定索引index处字符unicode编码

                if (code >= chfrom && code <= chend)
                {
                    return true;     //当code在中文范围内返回true
                }
                return false;    //当code不在中文范围内返回false
            }
            return false;
        } //

        #endregion [IsChineseLetter]

        #region [IsGBCode]

        /// <summary>
        /// 判断一个word是否为GB2312编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsGbCode(this string word)
        {
            var bytes = Encoding.GetEncoding("GB2312").GetBytes(word);
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code or other code
            {
                return false;
            }
            var byte1 = bytes[0];
            var byte2 = bytes[1];
            if (byte1 >= 176 && byte1 <= 247 && byte2 >= 160 && byte2 <= 254)    //判断是否是GB2312
            {
                return true;
            }
            return false;
        }

        #endregion [IsGBCode]

        #region [IsGBKCode]

        /// <summary>
        /// 判断一个word是否为GBK编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsGbkCode(this string word)
        {
            var bytes = Encoding.GetEncoding("GBK").GetBytes(word);
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code
            {
                return false;
            }
            var byte1 = bytes[0];
            var byte2 = bytes[1];
            if (byte1 >= 129 && byte1 <= 254 && byte2 >= 64 && byte2 <= 254)     //判断是否是GBK编码
            {
                return true;
            }
            return false;
        }

        #endregion [IsGBKCode]

        #region [IsBig5Code]

        /// <summary>
        /// 判断一个word是否为GBK编码的汉字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsBig5Code(this string word)
        {
            var bytes = Encoding.GetEncoding("Big5").GetBytes(word);
            if (bytes.Length <= 1)  // if there is only one byte, it is ASCII code
            {
                return false;
            }
            var byte1 = bytes[0];
            var byte2 = bytes[1];
            if ((byte1 >= 129 && byte1 <= 254) && ((byte2 >= 64 && byte2 <= 126) || (byte2 >= 161 && byte2 <= 254)))     //判断是否是Big5编码
            {
                return true;
            }
            return false;
        }

        #endregion [IsBig5Code]

        #region [IsOnlyContainsChinese]

        /// <summary>
        /// 给定一个字符串，判断其是否只包含有汉字
        /// </summary>
        /// <param name="testStr"></param>
        /// <returns></returns>
        public static bool IsOnlyContainsChinese(this string testStr)
        {
            var words = testStr.ToCharArray();
            foreach (var word in words)
            {
                if (IsGbCode(word.ToString()) || IsGbkCode(word.ToString()))  // it is a GB2312 or GBK chinese word
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        #endregion [IsOnlyContainsChinese]
    }
}