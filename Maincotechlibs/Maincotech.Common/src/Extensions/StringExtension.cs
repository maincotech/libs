using System.Globalization;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// Compare two string
        /// </summary>
        /// <param name="currentValue">first string</param>
        /// <param name="compareValue">compare string</param>
        /// <returns>compare result</returns>
        public static int CompareToIngnoreCase(this string currentValue, string compareValue)
        {
            return string.Compare(currentValue, compareValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// contains method with ignore case
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="compareValue">compare string value</param>
        /// <returns>the contains result</returns>
        public static bool ContainsIgnoreCase(this string currentValue, string compareValue)
        {
            return currentValue.ToLower().Contains(compareValue.ToLower());
        }

        /// <summary>
        /// equals method with ignore case
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="endValue">end string value</param>
        /// <returns>the end with string result</returns>
        public static bool EndWithIgnoreCase(this string currentValue, string endValue)
        {
            return currentValue.EndsWith(endValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// equals method with ignore case
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="compareValue">compare string value</param>
        /// <returns>the equals result</returns>
        public static bool EqualsIgnoreCase(this string currentValue, string compareValue)
        {
            return string.Equals(currentValue, compareValue, StringComparison.OrdinalIgnoreCase);
        }

        public static bool NotEqualsIgnoreCase(this string currentValue, string compareValue)
        {
            return !string.Equals(currentValue, compareValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// format string with instance
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string InvariantCultureFormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// the string class if extension
        /// </summary>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static string If(this string value, Predicate<string> predicate, Func<string, string> function)
        {
            return predicate(value) ? function(value) : value;
        }

        /// <summary>
        /// index of method with ignore case
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="compareValue">compare string value</param>
        /// <returns>the index of result</returns>
        public static int IndexOfIgnoreCase(this string currentValue, string compareValue)
        {
            return currentValue.IndexOf(compareValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// wrap the is match method of the regex
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsMatch(this string s, string pattern)
        {
            return s != null && Regex.IsMatch(s, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// To test if the string is null or empty at instance level
        /// </summary>
        /// <param name="value">the string value</param>
        /// <returns>the test result</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// last index of method with ignore case
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="compareValue">compare string value</param>
        /// <returns>the last index of result</returns>
        public static int LastIndexOfIgnoreCase(this string currentValue, string compareValue)
        {
            return currentValue.LastIndexOf(compareValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// wrap the regex class match method
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string Match(this string s, string pattern)
        {
            return s == null ? string.Empty : Regex.Match(s, pattern).Value;
        }

        /// <summary>
        /// Replaces the first occurrence of a specified System.String in this instance, with another specified System.String.
        /// </summary>
        /// <param name="currentValue">current string value</param>
        /// <param name="oldValue">old value</param>
        /// <param name="newValue">new value</param>
        /// <returns>replace result</returns>
        public static string ReplaceFirst(this string currentValue, string oldValue, string newValue)
        {
            var offset = currentValue.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            var temp = currentValue.Remove(offset, oldValue.Length);
            return temp.Insert(offset, newValue);
        }

        /// <summary>
        /// Convert String to Enum
        /// </summary>
        /// <typeparam name="T">the enum type</typeparam>
        /// <param name="value">the enum constant string</param>
        /// <returns>converted enum value</returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// convert string to int32
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>the converted int value</returns>
        public static int ToInt32(this string value)
        {
            return Convert.ToInt32(value);
        }
    }
}