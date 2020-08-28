using System.Collections.Generic;

namespace System.Collections
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>http://www.paulwheeler.com/blog/asp-net-mvc-foreach-loop-sugar</remarks>
    /// <typeparam name="T"></typeparam>
    public class ForEachItem<T>
    {
        public T Item { get; set; }

        public bool First { get; set; }

        public bool Last { get; set; }

        public int Index { get; set; }

        public bool NewGroup { get; set; }
    }

    public static class ForEachItemExtensions
    {
        public static IEnumerable<ForEachItem<T>> ToFor<T>(this IEnumerable<T> items)
        {
            return ToFor(items, 0);
        }

        public static IEnumerable<ForEachItem<T>> ToFor<T>(this IEnumerable<T> items, int group)
        {
            var list = new List<ForEachItem<T>>();
            foreach (var item in items)
            {
                var fei = new ForEachItem<T> { First = (list.Count == 0), Item = item, Index = list.Count };
                fei.NewGroup = (group > 0 && (list.Count % group == group - 1));
                list.Add(fei);
            }
            if (list.Count > 0) list[list.Count - 1].Last = true;
            return list;
        }
    }
}