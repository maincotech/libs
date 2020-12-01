using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static partial class StringExtensions
    {
        #region [Center]

        /// <summary>
        /// Centers the specified string.
        /// </summary>
        /// <param name="str">The string to center.</param>
        /// <param name="ch">The char to pad on eather side.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Center(this string str, char ch, int length)
        {
            var result = str;

            var strLength = str.Length;
            if (strLength < length)
            {
                var diff = length - strLength;

                result = result.PadLeft((diff / 2) + strLength, ch);
                result = result.PadRight(diff + strLength, ch);
            }

            return result;
        }

        #endregion [Center]

        #region [Contains]

        /// <summary>
        /// Returns true if the the specified container string contains the
        /// contained string.
        /// </summary>
        /// <param name="container">Container.</param>
        /// <param name="contained">Contained.</param>
        /// <param name="comparison">Case sensitivity.</param>
        /// <returns></returns>
        public static bool Contains(this string container, string contained, StringComparison comparison)
        {
            return container.IndexOf(contained, comparison) >= 0;
        }

        #endregion [Contains]

        #region [EnsureTrailingSlash]

        /// <summary>
        /// Ensure that the given string has a trailing slash.
        /// </summary>
        /// <param name="stringThatNeedsTrailingSlash"></param>
        /// <returns></returns>
        public static string EnsureTrailingSlash(this string stringThatNeedsTrailingSlash)
        {
            if (!stringThatNeedsTrailingSlash.EndsWith("/"))
            {
                return stringThatNeedsTrailingSlash + "/";
            }
            return stringThatNeedsTrailingSlash;
        }

        #endregion [EnsureTrailingSlash]

        #region [RemoveDoubleCharacter]

        /// <summary>
        /// Removes any double instances of the specified character.
        /// So "--" becomes "-" if the character is '-'.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        public static string RemoveDoubleCharacter(this string text, char character)
        {
            Check.Require(text, "text", Check.NotNull);

            if (character == char.MinValue)
                return text;

            var newString = new char[text.Length];
            var i = 0;

            var lastCharIsOurChar = false;
            foreach (var c in text)
            {
                if (c != character || !lastCharIsOurChar)
                {
                    newString[i] = c;
                    i++;
                }
                lastCharIsOurChar = (c == character);
            }

            return new string(newString, 0, i);
        }

        #endregion [RemoveDoubleCharacter]

        #region SafeTrim

        public static string SafeTrim(this string obj, params char[] trimChars)
        {
            return obj == null ? string.Empty : obj.Trim(trimChars);
        }

        public static string TrimAll(this string @this)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return @this;
            }
            return Regex.Replace(@this, @"\s+", "");
        }

        #endregion SafeTrim

        #region [SplitUppercase]

        /// <summary>
        /// Parses a camel cased or pascal cased string and returns an array
        /// of the words within the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string[] SplitUppercase(this string source)
        {
            if (source == null)
                return new string[] { }; //Return empty array.

            if (source.Length == 0)
                return new[] { "" };

            var words = new StringCollection();
            var wordStartIndex = 0;

            var letters = source.ToCharArray();
            // Skip the first letter. we don't care what case it is.
            for (var i = 1; i < letters.Length; i++)
            {
                if (char.IsUpper(letters[i]))
                {
                    //Grab everything before the current index.
                    words.Add(new string(letters, wordStartIndex, i - wordStartIndex));
                    wordStartIndex = i;
                }
            }
            //We need to have the last word.
            words.Add(new string(letters, wordStartIndex, letters.Length - wordStartIndex));

            //Copy to a string array.
            var wordArray = new string[words.Count];
            words.CopyTo(wordArray, 0);
            return wordArray;
        }

        #endregion [SplitUppercase]

        #region [SplitUppercaseToString]

        /// <summary>
        /// Parses a camel cased or pascal cased string and returns a new
        /// string with spaces between the words in the string.
        /// </summary>
        /// <example>
        /// The string "PascalCasing" will return an array with two
        /// elements, "Pascal" and "Casing".
        /// </example>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SplitUppercaseToString(this string source)
        {
            return string.Join(" ", SplitUppercase(source));
        }

        #endregion [SplitUppercaseToString]

        #region [PascalCase]

        /// <summary>
        /// Converts text to pascal case...
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string PascalCase(this string text)
        {
            Check.Require(text, "text", Check.NotNull);

            if (text.Length == 0)
                return text;

            var words = text.Split(' ');
            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    var word = words[i];
                    var firstChar = char.ToUpper(word[0]);
                    words[i] = firstChar + word.Substring(1);
                }
            }
            return string.Join(string.Empty, words);
        }

        #endregion [PascalCase]

        #region [Left]

        /// <summary>
        /// Returns a string containing a specified number of characters from the left side of a string.
        /// </summary>
        /// <param name="str">Required. String expression from which the leftmost characters are returned.</param>
        /// <param name="length">Required. Integer greater than 0. Numeric expression
        /// indicating how many characters to return. If 0, a zero-length string ("")
        /// is returned. If greater than or equal to the number of characters in Str,
        /// the entire string is returned. If str is null, this returns null.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if length is less than 0</exception>
        /// <exception cref="ArgumentNullException">Thrown if str is null.</exception>
        public static string Left(this string str, int length)
        {
            if (str.IsNullOrEmpty())
                return string.Empty;

            if (length >= str.Length)
                return str;

            return str.Substring(0, length);
        }

        #endregion [Left]

        #region [LeftBefore]

        /// <summary>
        /// Returns a string containing every character within a string before the
        /// first occurrence of another string.
        /// </summary>
        /// <param name="str">Required. String expression from which the leftmost characters are returned.</param>
        /// <param name="search">The string where the beginning of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string LeftBefore(this string str, string search)
        {
            return LeftBefore(str, search, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Returns a string containing every character within a string before the
        /// first occurrence of another string.
        /// </summary>
        /// <param name="original">Required. String expression from which the leftmost characters are returned.</param>
        /// <param name="search">The string where the beginning of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <param name="comparisonType">Determines whether or not to use case sensitive search.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string LeftBefore(this string original, string search, StringComparison comparisonType)
        {
            Check.Require(original, "original", Check.NotNull);
            Check.Require(search, "search", Check.NotNull);

            //Shortcut.
            if (search.Length > original.Length || search.Length == 0)
                return original;

            var searchIndex = original.IndexOf(search, 0, comparisonType);

            if (searchIndex < 0)
                return original;

            return Left(original, searchIndex);
        }

        #endregion [LeftBefore]

        #region [Right]

        /// <summary>
        /// Returns a string containing a specified number of characters from the right side of a string.
        /// </summary>
        /// <param name="original">Required. String expression from which the rightmost characters are returned.</param>
        /// <param name="length">Required. Integer greater than 0. Numeric expression
        /// indicating how many characters to return. If 0, a zero-length string ("")
        /// is returned. If greater than or equal to the number of characters in Str,
        /// the entire string is returned. If str is null, this returns null.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if length is less than 0</exception>
        /// <exception cref="ArgumentNullException">Thrown if str is null.</exception>
        public static string Right(this string original, int length)
        {
            Check.Require(original, "original", Check.NotNull);
            Check.Require(length, "length", Check.GreaterThanOrEqual(0));

            if (original.Length == 0 || length == 0)
                return string.Empty;

            if (length >= original.Length)
                return original;

            return original.Substring(original.Length - length);
        }

        #endregion [Right]

        #region [RightAfter]

        /// <summary>
        /// Returns a string containing every character within a string after the
        /// first occurrence of another string.
        /// </summary>
        /// <param name="original">Required. String expression from which the rightmost characters are returned.</param>
        /// <param name="search">The string where the end of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string RightAfter(this string original, string search)
        {
            return RightAfter(original, search, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Returns a string containing every character within a string after the
        /// first occurrence of another string.
        /// </summary>
        /// <param name="original">Required. String expression from which the rightmost characters are returned.</param>
        /// <param name="search">The string where the end of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <param name="comparisonType">Determines whether or not to use case sensitive search.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string RightAfter(this string original, string search, StringComparison comparisonType)
        {
            Check.Require(original, "original", Check.NotNull);
            Check.Require(search, "search", Check.NotNull);

            //Shortcut.
            if (search.Length > original.Length || search.Length == 0)
                return original;

            var searchIndex = original.IndexOf(search, 0, comparisonType);

            if (searchIndex < 0)
                return original;

            return Right(original, original.Length - (searchIndex + search.Length));
        }

        /// <summary>
        /// Returns a string containing every character within a string after the
        /// last occurrence of another string.
        /// </summary>
        /// <param name="original">Required. String expression from which the rightmost characters are returned.</param>
        /// <param name="search">The string where the end of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string RightAfterLast(this string original, string search)
        {
            return RightAfterLast(original, search, original.Length - 1, StringComparison.InvariantCulture);
        }

        /// <summary>
        /// Returns a string containing every character within a string after the
        /// last occurrence of another string.
        /// </summary>
        /// <param name="original">Required. String expression from which the rightmost characters are returned.</param>
        /// <param name="search">The string where the end of it marks the
        /// characters to return.  If the string is not found, the whole string is
        /// returned.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="comparisonType">Determines whether or not to use case sensitive search.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if str or searchstring is null.</exception>
        public static string RightAfterLast(this string original, string search, int startIndex, StringComparison comparisonType)
        {
            Check.Require(original, "original", Check.NotNull);
            Check.Require(search, "search", Check.NotNull);

            //Shortcut.
            if (search.Length > original.Length || search.Length == 0)
                return original;

            var searchIndex = original.LastIndexOf(search, startIndex, comparisonType);

            if (searchIndex < 0)
                return original;

            return Right(original, original.Length - (searchIndex + search.Length));
        }

        #endregion [RightAfter]

        #region RightOf

        public static string RightOf(this string src, char c)
        {
            var index = src.IndexOf(c);
            if (index == -1)
            {
                return "";
            }
            return src.Substring(index + 1);
        }

        public static string RightOf(this string src, string text)
        {
            var index = src.IndexOf(text);
            if (index == -1)
            {
                return "";
            }
            return src.Substring(index + text.Length);
        }

        public static string RightOf(this string src, char c, int n)
        {
            var index = -1;
            while (n != 0)
            {
                index = src.IndexOf(c, index + 1);
                if (index == -1)
                {
                    return "";
                }
                n--;
            }
            return src.Substring(index + 1);
        }

        #endregion RightOf

        #region [ReplaceFirst]

        /// <summary>
        /// Replace the first occurrence of <paramref name="find"/> (case sensitive) with
        /// <paramref name="replace"/>.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string str, string find, string replace)
        {
            return str.ReplaceFirst(find, replace, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Replace the first occurrence of <paramref name="find"/> with
        /// <paramref name="replace"/>.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="find">The find.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="findComparison">The find comparison.</param>
        /// <returns></returns>
        public static string ReplaceFirst(this string str, string find, string replace, StringComparison findComparison)
        {
            Check.Require(str, "str", Check.NotNullOrEmpty);
            Check.Require(find, "find", Check.NotNullOrEmpty);
            Check.Require(replace, "replace", Check.NotNullOrEmpty);

            var firstIndex = str.IndexOf(find, findComparison);

            if (firstIndex != -1)
            {
                if (firstIndex == 0)
                {
                    str = replace + str.Substring(find.Length);
                }
                else if (firstIndex == (str.Length - find.Length))
                {
                    str = str.Substring(0, firstIndex) + replace;
                }
                else
                {
                    str = str.Substring(0, firstIndex) + replace + str.Substring(firstIndex + find.Length);
                }
            }
            return str;
        }

        #endregion [ReplaceFirst]

        #region [With]

        /// <summary>
        /// replacement for String.Format
        /// </summary>
        public static string With(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        #endregion [With]

        #region ToCharacterSeparatedFileName

        /// <summary>
        /// 将name转换成带'_'分隔符的字符串并加上指定的后缀(extension)
        /// </summary>
        public static string ToCharacterSeparatedFileName(this string name, char separator, string extension)
        {
            /*
            MatchCollection matchs = Regex.Matches(name, @"(\P{Lu}+)|(\p{Lu}+\P{Lu}*)");
            string str = "";
            for (int i = 0; i < matchs.Count; i++)
            {
                if (i != 0)
                {
                    str = str + separator;
                }
                str = str + matchs[i].ToString().ToLower();
            }
            string format = string.IsNullOrEmpty(extension) ? "{0}{1}" : "{0}.{1}";
            return string.Format(format, str, extension);
            */
            var matchs = Regex.Matches(name, @"([A-Z]+)[a-z]*|\d{1,}[a-z]{0,}");
            var str = "";
            for (var i = 0; i < matchs.Count; i++)
            {
                if (i != 0)
                {
                    str = str + separator;
                }
                str = str + matchs[i].ToString().ToLower();
            }
            var format = string.IsNullOrEmpty(extension) ? "{0}{1}" : "{0}.{1}";
            return string.Format(format, str, extension);
        }

        #endregion ToCharacterSeparatedFileName

        #region ToLowerCamelCase

        /// <summary>
        /// Convert string "HelloWorld" To "helloWorld"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToLowerCamelCase(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (name.Length == 1)
                return name.ToLower(CultureInfo.InvariantCulture);

            return name.Substring(0, 1).ToLower(CultureInfo.InvariantCulture) + name.Substring(1);
        }

        #endregion ToLowerCamelCase

        #region ToHtml

        /// <summary>
        /// Texts to HTML.
        /// </summary>
        /// <param name="txtStr">The TXT STR.</param>
        /// <returns>The formated str.</returns>
        public static string ToHtml(this string txtStr)
        {
            return txtStr.Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;").
                Replace("<", "&lt;").Replace(">", "&gt;").Replace("\r", "").Replace("\n", "<br />");
        }

        #endregion ToHtml

        #region CheckForUrl

        /// <summary>
        /// Checks the text and prepends "http://" if it doesn't have it already.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <returns></returns>
        public static Uri CheckForUrl(this string text)
        {
            if (text == null)
                return null;

            text = text.Trim();

            if (string.IsNullOrEmpty(text))
                return null;

            if (!text.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                text = "http://" + text;
            }

            return new Uri(text);
        }

        #endregion CheckForUrl

        #region ReplaceHost

        /// <summary>
        /// Replaces the host in the given url with the new host.
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <param name="newHost"></param>
        /// <returns></returns>
        public static string ReplaceHost(this string originalUrl, string newHost)
        {
            return Regex.Replace(originalUrl, @"(https?://).*?((:\d+)?/.*)?$", "$1" + newHost + "$2", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        #endregion ReplaceHost

        #region HasIllegalContent

        /// <summary>
        /// Tests the specified string looking for illegal characters
        /// or html tags.
        /// </summary>
        /// <param name="s">S.</param>
        /// <returns></returns>
        public static bool HasIllegalContent(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            if (s.IndexOf("<script", StringComparison.InvariantCultureIgnoreCase) > -1
                || s.IndexOf("&#60script", StringComparison.InvariantCultureIgnoreCase) > -1
                || s.IndexOf("&60script", StringComparison.InvariantCultureIgnoreCase) > -1
                || s.IndexOf("%60script", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                return true;
            }
            return false;
        }

        #endregion HasIllegalContent

        #region TruncateText

        private static readonly Regex _stripHtmlRegex = new Regex("<[^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Truncate a given text to the given number of characters.
        /// Also any embedded html is stripped.
        /// </summary>
        public static string TruncateText(this string fullText, int numberOfCharacters)
        {
            string text;
            if (fullText.Length > numberOfCharacters)
            {
                var spacePos = fullText.IndexOf(" ", numberOfCharacters);
                if (spacePos > -1)
                {
                    text = fullText.Substring(0, spacePos) + "...";
                }
                else
                {
                    text = fullText;
                }
            }
            else
            {
                text = fullText;
            }
            text = _stripHtmlRegex.Replace(text, " ");
            return text;
        }

        #endregion TruncateText

        public static string Reverse(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="absolutePath">The path to compress</param>
        /// <param name="limit">The maximum length</param>
        /// <param name="delimiter">The character(s) to use to imply incompleteness</param>
        /// <returns></returns>
        public static string ShrinkPath(this string absolutePath, int limit, string delimiter = "…")
        {
            //no path provided
            if (string.IsNullOrEmpty(absolutePath))
            {
                return string.Empty;
            }

            var name = IO.Path.GetFileName(absolutePath);
            var nameLength = name.Length;
            var pathLength = absolutePath.Length;
            var dir = absolutePath.Substring(0, pathLength - nameLength);

            var delimiterLength = delimiter.Length;
            var idealMinLength = nameLength + delimiterLength;

            var slash = (absolutePath.IndexOf("/", StringComparison.Ordinal) > -1 ? "/" : "\\");

            //less than the minimum amt
            if (limit < ((2 * delimiterLength) + 1))
            {
                return "";
            }

            //fullpath
            if (limit >= pathLength)
            {
                return absolutePath;
            }

            //file name condensing
            if (limit < idealMinLength)
            {
                return delimiter + name.Substring(0, (limit - (2 * delimiterLength))) + delimiter;
            }

            //whole name only, no folder structure shown
            if (limit == idealMinLength)
            {
                return delimiter + name;
            }

            return dir.Substring(0, (limit - (idealMinLength + 1))) + delimiter + slash + name;
        }


        /// <summary>
        /// Remove Diacritics from a string
        /// This converts accented characters to nonaccented, which means it is
        /// easier to search for matching data with or without such accents.
        /// Respaced and converted to an Extension Method
        /// <example>
        ///    aàáâãäåçc
        /// is converted to
        ///    aaaaaaacc
        /// </example>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string WithoutDiacritics(this string str)
        {
            var normalizedString = str.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

    }
}