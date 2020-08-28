using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty(this IEnumerable source)
        {
            if (source != null)
            {
                return !source.Cast<object>().Any();
            }
            return true;
        }
        public static string Concat(this IEnumerable items, string separator)
        {
            return Concat(items, separator, "{0}");
        }

        public static string Concat(this IEnumerable items, string separator, string template)
        {
            var builder = new StringBuilder();
            foreach (var obj2 in items)
            {
                builder.Append(separator);
                builder.Append(string.Format(template, obj2));
            }
            return builder.ToString().RightOf(separator);
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> target)
        {
            var r = new Random();

            return target.OrderBy(x => (r.Next()));
        }

        public static IEnumerable<List<T>> SplitByLength<T>(this IEnumerable<T> target, int length)
        {
            var res = new List<List<T>>();
            var skip = 0;

            var cnt = target.Count();

            while (skip < cnt)
            {
                var lst = target.Skip(skip).Take(length).ToList();
                res.Add(lst);
                skip += length;
            }

            return res;
        }
    }
}