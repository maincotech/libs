using System;
using System.Collections.Generic;

namespace Maincotech.Data
{
    /// <summary>
    /// 对应前台 ligerFilter 的检索规则数据
    /// </summary>
    [Serializable]
    public class FilterGroup : List<FilterRule>
    {
        /// <summary>
        /// Group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 规则
        /// </summary>
        public IList<FilterRule> Rules { get; set; }

        /// <summary>
        /// 逻辑连词 AND OR
        /// </summary>
        public string Conjunction { get; set; }

        /// <summary>
        /// 条件组
        /// </summary>
        public IList<FilterGroup> Groups { get; set; }

        public LogicalOperator LogicalOperator { get; set; }
    }
}