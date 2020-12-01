using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensionsMatch
    {
        public static string GetSplit(this string source, string regex, int index)
        {
            if (source.IsNullOrEmpty())
                return string.Empty;

            var matches = Regex.Split(source, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return matches[index];
        }

        public static string GetMatch(this string source, string regex, int groupIndex)
        {
            if (source.IsNullOrEmpty())
                return string.Empty;

            var titleMatch = Regex.Match(source, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (titleMatch.Success)

                return titleMatch.Groups[groupIndex].Value;

            return string.Empty;
        }

        public static IList<string[]> GetMatches(this string source, string pattern)
        {
            return source.GetMatches(pattern, 0);
        }

        public static IList<string[]> GetMatches(this string source, string pattern, int limit)
        {
            var rgClass = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var matches = rgClass.Matches(source);

            var outputCount = limit <= 0 ? matches.Count : (matches.Count >= limit ? limit : matches.Count);

            var res = new List<string[]>(outputCount);

            for (var i = 0; i < outputCount; i++)
            {
                var match = matches[i];

                if (match.Success)
                {
                    var arr = new List<string>(match.Groups.Count);

                    for (var j = 0; j < match.Groups.Count; j++)
                    {
                        arr.Add(match.Groups[j].Value);
                    }

                    res.Add(arr.ToArray());
                }
                else
                {
                    outputCount++;
                }
            }

            return res;
        }

        public static string GetFirstMatch(this string source, string pattern)
        {
            return GetMatch(source, pattern, 1);
        }

        public static string GetFirstMatch(this string source, string pattern, int groupIndex)
        {
            return GetMatch(source, pattern, groupIndex);
        }

        public static string GetValueUseRegex(this string text, string pattern)
        {
            return GetValueUseRegex(text, pattern, "data");
        }

        public static string GetValueUseRegex(this string text, string pattern, string groupName)
        {
            var rgClass = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var match = rgClass.Match(text);

            if (match.Success)
                return match.Groups[groupName].Value;

            return string.Empty;
        }

        public static string GetFirstMatch(this string source, string start, string end)
        {
            return source.GetFirstMatch(start, end, true, true);
        }

        public static string GetFirstMatch(this string source, string start, string end, bool appendStart, bool appendEnd)
        {
            if (source.IsNullOrEmpty())
                return string.Empty;

            var sidx = source.IndexOf(start);

            if (sidx == -1)
                return string.Empty;

            var eidx = source.Substring(sidx + start.Length).IndexOf(end) + sidx + start.Length;

            if (eidx == -1)
                return string.Empty;

            var contentlength = eidx - sidx - start.Length;

            var startIndex = appendStart ? sidx : sidx + start.Length;

            var length = contentlength + (appendStart ? start.Length : 0) + (appendEnd ? end.Length : 0);

            //Console.WriteLine(_source);
            //Console.WriteLine("start = " + start);
            //Console.WriteLine("end = " + end);
            //Console.WriteLine("sidx = " + sidx);
            //Console.WriteLine("eidx = " + eidx);
            //Console.WriteLine("contentlength = " + contentlength);

            //Console.WriteLine("_source.Length = " + _source.Length);
            //Console.WriteLine("startIndex = " + startIndex);
            //Console.WriteLine("length = " + length);

            return source.Substring(startIndex, length);
        }

        #region WipeScript

        private static readonly Regex _regex1 = new Regex(@"<script[\s\s]+</script *>", RegexOptions.IgnoreCase);
        private static readonly Regex _regex2 = new Regex(@" href *= *[\s\s]*script *:", RegexOptions.IgnoreCase);
        private static readonly Regex _regex3 = new Regex(@" on[\s\s]*=", RegexOptions.IgnoreCase);
        private static readonly Regex _regex4 = new Regex(@"<iframe[\s\s]+</iframe *>", RegexOptions.IgnoreCase);
        private static readonly Regex _regex5 = new Regex(@"<frameset[\s\s]+</frameset *>", RegexOptions.IgnoreCase);

        /// <summary>
        /// 清楚脚本、iframe、on事件
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string WipeScript(this string html)
        {
            html = _regex1.Replace(html, string.Empty); //过滤<script></script>标记
            html = _regex2.Replace(html, string.Empty); //过滤href=javascript: (<a>) 属性
            html = _regex3.Replace(html, " _disibledevent="); //过滤其它控件的on...事件
            html = _regex4.Replace(html, string.Empty); //过滤iframe
            html = _regex5.Replace(html, string.Empty); //过滤frameset
            return html;
        }

        #endregion WipeScript

        /// <summary>
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/791963c8-9e20-4e9e-b184-f0e592b943b0
        /// </summary>
        /// <returns>Ex: casedWordHTTPWriter becomes "Cased Word HTTP Writer", HotMomma becomes "Hot Momma"</returns>
        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        /// <remarks>This was written pre-linq</remarks>
        public static string ToDelimitedString(this List<string> list, string separator = ":", bool insertSpaces = false, string delimiter = "")
        {
            var result = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                string initialStr = list[i];
                var currentString = (delimiter == string.Empty) ? initialStr : string.Format("{1}{0}{1}", initialStr, delimiter);
                if (i < list.Count - 1)
                {
                    currentString += separator;
                    if (insertSpaces)
                    {
                        currentString += ' ';
                    }
                }
                result.Append(currentString);
            }
            return result.ToString();
        }
    }
}