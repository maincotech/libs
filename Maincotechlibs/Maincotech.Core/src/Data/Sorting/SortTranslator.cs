using System.Text;

namespace Maincotech.Data
{
    public class SortTranslator
    {
        protected char LeftToken = '[';
        protected char RightToken = ']';
        protected string Asc = "ASC";
        protected string Desc = "DESC";

        public SortGroup Group { get; set; }

        public string CommandText { get; private set; }

        public SortTranslator()
        {
        }

        public SortTranslator(SortGroup group)
        {
            Group = group;
        }

        public void Translate()
        {
            CommandText = TranslateGroup(Group);
        }

        public string TranslateGroup(SortGroup group)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < group.SortRules.Count; i++)
            {
                var rule = group.SortRules[i];
                builder.Append(TranslateRule(rule));
                if (i != group.SortRules.Count - 1)
                {
                    builder.Append(',');
                }
            }

            return builder.ToString();
        }

        public string TranslateRule(SortRule rule)
        {
            var builder = new StringBuilder();
            builder.Append(LeftToken + rule.Field + RightToken);
            if (rule.SortOrder == SortOrder.Descending)
            {
                builder.Append(' ' + Desc + ' ');
            }
            else
            {
                builder.Append(' ' + Asc + ' ');
            }

            return builder.ToString();
        }
    }
}