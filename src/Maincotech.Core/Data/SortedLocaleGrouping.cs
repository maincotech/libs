using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Maincotech.Data
{
    /// <summary>
    ///     Provides a way to access basic set of sorted group headers for any locale.
    /// </summary>
    internal class SortedLocaleGrouping
    {
        private readonly CultureInfo _currentCultureInfo;
        private readonly List<string> _groupDisplayNames;

        /// <summary>
        ///     Initializes a new instance of the SortedLocaleGrouping class.
        /// </summary>
        public SortedLocaleGrouping()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the SortedLocaleGrouping class with the specified CultureInfo.
        /// </summary>
        /// <param name="culture">The specified CultureInfo.</param>
        public SortedLocaleGrouping(CultureInfo culture)
        {
            _currentCultureInfo = culture;
            if (_currentCultureInfo?.Name == "zh-CN")
            {
                _groupDisplayNames = Enumerable.Range('A', 26).Select(c => ((char)c).ToString()).ToList();
            }
            else
            {
                _groupDisplayNames = Enumerable.Range('a', 26).Select(c => ((char)c).ToString()).ToList();
            }
        }

        /// <summary>
        ///     Get the strings for all the groups for this locale.
        /// </summary>
        public IEnumerable<string> GroupDisplayNames => _groupDisplayNames;

        /// <summary>
        ///     Gets a value that indicates whether the culture supports optional phonetic grouping.
        /// </summary>
        public bool SupportsPhonetics { get; }

        /// <summary>
        ///     Returns the group index to which the value string belongs.
        /// </summary>
        /// <param name="p">The specified string.</param>
        /// <returns>The group index to which the value string belongs.</returns>
        /// <remarks>Return value matches the position in the GroupDisplayNames collection.</remarks>
        internal int GetGroupIndex(string p)
        {
            return _groupDisplayNames.IndexOf(p.Substring(0, 1));
        }
    }
}