using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maincotech.Data
{
    public interface IHierarchyData : IEquatable<IHierarchyData>
    {
        object Data { get; }
        string FullPath { get; }
        bool IsExpanded { get; set; }
        bool IsRoot { get; }
        bool IsSelected { get; set; }
        IHierarchyData Parent { get; set; }
        string PathSeperator { get; }
        string RelativePath { get; }
        IHierarchyData SelectedChild { get; set; }
        Task<IHierarchyData> FindChild(string path);
        Task<IEnumerable<IHierarchyData>> FindChildrenStartWith(string filter);
        Task<IHierarchyData> FindDescendant(string path);
        Task<IEnumerable<IHierarchyData>> GetChildren();
        IEnumerable<IHierarchyData> GetHierarchy();
        bool IsDescendantOf(IHierarchyData hierarchyData);
        IHierarchyData FindSameParentWith(IHierarchyData hierarchyData);
        IHierarchyData Root { get; }
    }

    public static class HierarchyDataExtensions
    {
        public static IHierarchyData FindSameParent(this IHierarchyData one, IHierarchyData another)
        {
            if (one == null || another == null)
            {
                return null;
            }
            return one.FindSameParentWith(another);
        }

        public static void ClearSelectionTo(this IHierarchyData from, IHierarchyData to)
        {
            if (from == null)
            {
                return;
            }
            from.IsSelected = false;
            var parent = from.Parent;
            while (parent != null && parent != to)
            {
                parent.IsSelected = false;
                parent.SelectedChild = null;
                parent = parent.Parent;
            }
        }

        public static void SelecteTo(this IHierarchyData from, IHierarchyData to)
        {
            if (from == null)
            {
                return;
            }
            from.IsSelected = true;
            var hierarchy = from.GetHierarchy().ToArray();
            var flag = to == null || hierarchy[0].Equals(to);
            for (var i = 0; i < hierarchy.Length - 1; i++)
            {
                if (flag)
                {
                    var current = hierarchy[i];
                    current.SelectedChild = hierarchy[i + 1];
                    //current.IsSelected = true;
                    current.IsExpanded = true;
                }
                else
                {
                    flag = hierarchy[i] == to;
                }
            }
        }

        public static async Task<IHierarchyData> Find(this IEnumerable<IHierarchyData> roots, string path)
        {
            var hierarchyDatas = roots as IHierarchyData[] ?? roots.ToArray();
            if (roots == null || !hierarchyDatas.Any() || string.IsNullOrEmpty(path))
            {
                return null;
            }
            IHierarchyData found = null;

            foreach (
                var root in
                    hierarchyDatas.Where(root => path.StartsWith(root.RelativePath, StringComparison.OrdinalIgnoreCase))
                )
            {
                if (root.RelativePath == path)
                {
                    found = root;
                    break;
                }
                found = await root.FindDescendant(path);
                if (found != null)
                {
                    break;
                }
            }
            return found;
        }

        public static async Task<IEnumerable<IHierarchyData>> Filter(this IEnumerable<IHierarchyData> roots,
            string pathFilter)
        {
            var hierarchyDatas = roots as IHierarchyData[] ?? roots.ToArray();
            if (roots == null || !hierarchyDatas.Any())
            {
                return null;
            }
            if (string.IsNullOrEmpty(pathFilter))
            {
                return roots;
            }

            var current = hierarchyDatas[0];
            var pathIndex = pathFilter.LastIndexOf(current.PathSeperator, StringComparison.OrdinalIgnoreCase);
            if (pathIndex <= 0)
            {
                return
                    hierarchyDatas.Where(
                        item => item.RelativePath.StartsWith(pathFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }
            var path = pathFilter.Substring(0, pathIndex);
            var filter = pathIndex + 1 < pathFilter.Length ? pathFilter.Substring(pathIndex + 1) : string.Empty;
            var found = await hierarchyDatas.Find(path);
            return found != null ? (await found.FindChildrenStartWith(filter)) : null;
        }


    }
}