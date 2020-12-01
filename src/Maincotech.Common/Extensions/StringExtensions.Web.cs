namespace System
{
    public static partial class StringExtensions
    {
        public static string FixLineBreakForWeb(this string str)
        {
            return str.Replace(Environment.NewLine, "<br/>");
        }

        public static string FixTabsForWeb(this string str)
        {
            return str.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
        }

        public static string FixSpaceForWeb(this string str)
        {
            return str.Replace(" ", "&nbsp;");
        }
    }
}
