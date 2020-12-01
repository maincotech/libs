using System;
using System.Collections.Generic;

namespace Maincotech.Data
{
    /// <summary>
    /// 过滤规则
    /// </summary>
    [Serializable]
    public class FilterRule
    {
        /// <summary>
        /// 过滤规则
        /// </summary>
        public FilterRule()
        {
        }

        /// <summary>
        /// 过滤规则
        /// </summary>
        /// <param name="field">参数</param>
        /// <param name="value">值</param>
        public FilterRule(string field, object value)
            : this(field, value, "equal")
        {
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="field">参数</param>
        /// <param name="value">值</param>
        /// <param name="op">操作</param>
        public FilterRule(string field, object value, string op)
        {
            Field = field;
            Value = value;
            Operation = op;
        }

        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Obsolete]
        public object Value { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 逻辑连词 AND OR
        /// </summary>
        public string Conjunction { get; set; }

        public FilterOperator FilterOperator { get; set; }

        public List<object> PropertyValues { get; set; }

        public LogicalOperator LogicalOperator { get; set; }
    }
}