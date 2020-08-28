using System;
using System.Collections.Generic;

namespace Maincotech.Data
{
    /// <summary>
    /// 用于存放过滤参数,比如一个是名称,一个是值,等价于sql中的Parameters
    /// </summary>
    public class FilterParameter
    {
        public FilterParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        /// <summary>
        /// 为查询语句添加参数
        /// </summary>
        /// <param name="commandText">查询命令</param>
        /// <returns></returns>
        public static string AddParameters(string commandText, IEnumerable<FilterParameter> listfilter)
        {
            foreach (var param in listfilter)
            {
                if (param.Value.IsValidInput())
                {
                    commandText = commandText.Replace("@" + param.Name, "'" + param.Value + "'");
                }
            }
            return commandText;
        }
    }
}