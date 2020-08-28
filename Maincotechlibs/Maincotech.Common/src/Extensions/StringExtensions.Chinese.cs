using System.Linq;
using System.Text;

namespace System
{
    public static partial class StringExtensionsChinese
    {
        /// <summary>
        /// 把汉字转换成拼音(全拼)
        /// </summary>
        /// <param name="hzString">汉字字符串</param>
        /// <returns>转换后的拼音(全拼)字符串</returns>
        public static string GetQuanPin(this string chineseStr)
        {
            var result = new StringBuilder(chineseStr.Length * 10);
            var charArray = chineseStr.ToCharArray();
            for (var j = 0; j < charArray.Length; j++)
            {
                result.Append(charArray[j].GetPinYin());
            }
            return result.ToString();
        }

        /// <summary>
        /// 获得汉字的首字母
        /// </summary>
        /// <returns></returns>
        public static string GetFirstCapitalizedLetter(this string chineseStr)
        {
            var firstChar = chineseStr.ToCharArray().First().ToString();
            return firstChar.GetQuanPin().Substring(0, 1).ToUpper();
        }
    }
}