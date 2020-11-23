using System;
using System.Collections.Generic;
using System.Text;

namespace Maincotech.Data
{
    /// <summary>
    /// 将检索规则 翻译成 where sql 语句,并生成相应的参数列表
    /// 如果遇到{CurrentUserID}这种，翻译成对应的参数
    /// </summary>
    public class FilterTranslator
    {
        //几个前缀/后缀
        /// <summary>
        /// 左中括号[(用于表示数据库实体前的标识)
        /// </summary>
        protected char LeftToken = '[';

        /// <summary>
        /// 用于可变参替换的标志
        /// </summary>
        protected char ParamPrefixToken = '@';

        /// <summary>
        /// 右中括号(用于表示数据库实体前的标识)
        /// </summary>
        protected char RightToken = ']';

        /// <summary>
        /// 组条件括号
        /// </summary>
        protected char GroupLeftToken = '(';

        /// <summary>
        /// 右条件括号
        /// </summary>
        protected char GroupRightToken = ')';

        /// <summary>
        /// 模糊查询符号
        /// </summary>
        protected char LikeToken = '%';

        /// <summary>
        /// 参数计数器
        /// </summary>
        private int _paramCounter;

        //几个主要的属性
        public FilterGroup Group { get; set; }

        /// <summary>
        /// 最终的Where语句(包括可变参占位符)
        /// </summary>
        public string CommandText { get; private set; }

        /// <summary>
        /// 查询语句可变参数数组
        /// </summary>
        public IList<FilterParameter> Parms { get; }

        /// <summary>
        /// 是否为Entity To Sql 生成where翻译语句(Entity To Sql就需要在实体前面加it,例如it.ID=@ID and it.Name-@Name)
        /// 否则为普通的SQL语句可变参拼接
        /// </summary>
        public bool IsEntityToSql { get; set; }

        public FilterTranslator()
            : this(null)
        {
            IsEntityToSql = false;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="group"></param>
        public FilterTranslator(FilterGroup group)
        {
            Group = group;
            Parms = new List<FilterParameter>();
        }

        /// <summary>
        /// 翻译语句成sql的where查询条件
        /// </summary>
        public void Translate()
        {
            CommandText = TranslateGroup(Group);
        }

        /// <summary>
        /// 对多组规则进行翻译解析
        /// </summary>
        /// <param name="group">规则数组</param>
        /// <returns></returns>
        public string TranslateGroup(FilterGroup group)
        {
            var bulider = new StringBuilder();
            if (group == null) return " 1=1 ";
            var appended = false;
            bulider.Append(GroupLeftToken);
            if (group.Count > 0)
            {
                foreach (var rule in group)
                {
                    if (appended)
                    {
                        bulider.Append(GetOperatorQueryText(rule.Conjunction));
                    }
                    bulider.Append(TranslateRule(rule));
                    appended = true;
                }
            }
            if (group.Groups != null)
            {
                foreach (var subgroup in group.Groups)
                {
                    if (appended)
                        bulider.Append(GetOperatorQueryText(group.Conjunction));
                    bulider.Append(TranslateGroup(subgroup));
                    appended = true;
                }
            }
            bulider.Append(GroupRightToken);
            if (appended == false) return " 1=1 ";
            return bulider.ToString();
        }

        /// <summary>
        /// 注册用户匹配管理，当不方便修改ligerRM.dll时，可以通过这种方式，在外部注册
        ///  currentParmMatch.Add("{CurrentUserID}",()=>UserID);
        /// currentParmMatch.Add("{CurrentRoleID}",()=>UserRoles.Split(',')[0].ObjToInt());
        /// </summary>
        /// <param name="match"></param>
        public static void RegCurrentParmMatch(string key, Func<int> fn)
        {
            if (!_currentParmMatch.ContainsKey(key))
                _currentParmMatch.Add(key, fn);
        }

        /// <summary>
        /// 匹配当前用户信息，都是int类型
        /// 对于CurrentRoleID，只返回第一个角色
        /// 注意这里是用来定义隐藏规则,比如,用户只能自己访问等等,
        /// </summary>
        private static readonly Dictionary<string, Func<int>> _currentParmMatch = new Dictionary<string, Func<int>>();

        /// <summary>
        /// 翻译规则
        /// </summary>
        /// <param name="rule">规则</param>
        /// <returns></returns>
        public string TranslateRule(FilterRule rule)
        {
            var bulider = new StringBuilder();
            if (rule == null) return " 1=1 ";

            //如果字段名采用了 用户信息参数
            if (_currentParmMatch.ContainsKey(rule.Field))
            {
                var field = _currentParmMatch[rule.Field]();
                bulider.Append(ParamPrefixToken + CreateFilterParam(field, "int"));
            }
            else //这里实现了数据库实体条件的拼接,[ID]=xxx的形式
            {
                //如果是EF To Sql
                if (IsEntityToSql)
                {
                    bulider.Append(" it." + rule.Field + " ");
                }
                else
                {
                    bulider.Append(LeftToken + rule.Field + RightToken);
                }
            }
            //操作符
            bulider.Append(GetOperatorQueryText(rule.Operation));

            var op = rule.Operation.ToLower();
            if (op == "like" || op == "endwith")
            {
                var value = rule.Value.ToString();
                if (!value.StartsWith(LikeToken.ToString()))
                {
                    rule.Value = LikeToken + value;
                }
            }
            if (op == "like" || op == "startwith")
            {
                var value = rule.Value.ToString();
                if (!value.EndsWith(LikeToken.ToString()))
                {
                    rule.Value = value + LikeToken;
                }
            }
            if (op == "in" || op == "notin")
            {
                var values = rule.Value.ToString().Split(',');
                var appended = false;
                bulider.Append("(");
                foreach (var value in values)
                {
                    if (appended) bulider.Append(",");
                    //如果值使用了 用户信息参数 比如： in ({CurrentRoleID},4)
                    if (_currentParmMatch.ContainsKey(value))
                    {
                        var val = _currentParmMatch[value]();
                        bulider.Append(ParamPrefixToken + CreateFilterParam(val, "int"));
                    }
                    else
                    {
                        bulider.Append(ParamPrefixToken + CreateFilterParam(value, rule.Type));
                    }
                    appended = true;
                }
                bulider.Append(")");
            }
            //is null 和 is not null 不需要值
            else if (op != "isnull" && op != "isnotnull")
            {
                //如果值使用了 用户信息参数 比如 [EmptID] = {CurrentEmptID}
                if (rule.Value != null && _currentParmMatch.ContainsKey(rule.Value.SafeToString()))
                {
                    var value = _currentParmMatch[rule.Value.SafeToString()]();
                    bulider.Append(ParamPrefixToken + CreateFilterParam(value, "int"));
                }
                else
                {
                    bulider.Append(ParamPrefixToken + CreateFilterParam(rule.Value, rule.Type));
                }
            }
            return bulider.ToString();
        }

        /// <summary>
        /// 创建过滤规则参数数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateFilterParam(object value, string type)
        {
            var paramName = "p" + ++_paramCounter;
            var val = value;

            ////原版在这里要验证类型
            //if (type.Equals("int", StringComparison.OrdinalIgnoreCase) || type.Equals("digits", StringComparison.OrdinalIgnoreCase))
            //    val = val.ObjToInt ();
            //if (type.Equals("float", StringComparison.OrdinalIgnoreCase) || type.Equals("number", StringComparison.OrdinalIgnoreCase))
            //    val = type.ObjToDecimal();

            var param = new FilterParameter(paramName, val);
            Parms.Add(param);
            return paramName;
        }

        /// <summary>
        /// 获取解析的参数
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var bulider = new StringBuilder();
            bulider.Append("CommandText:");
            bulider.Append(CommandText);
            bulider.AppendLine();
            bulider.AppendLine("Parms:");
            foreach (var parm in Parms)
            {
                bulider.AppendLine(string.Format("{0}:{1}", parm.Name, parm.Value));
            }
            return bulider.ToString();
        }

        #region 公共工具方法

        /// <summary>
        /// 获取操作符的SQL Text
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string GetOperatorQueryText(string op)
        {
            switch (op.ToLower())
            {
                case "add":
                    return " + ";

                case "bitwiseand":
                    return " & ";

                case "bitwisenot":
                    return " ~ ";

                case "bitwiseor":
                    return " | ";

                case "bitwisexor":
                    return " ^ ";

                case "divide":
                    return " / ";

                case "equal":
                    return " = ";

                case "greater":
                    return " > ";

                case "greaterorequal":
                    return " >= ";

                case "isnull":
                    return " is null ";

                case "isnotnull":
                    return " is not null ";

                case "less":
                    return " < ";

                case "lessorequal":
                    return " <= ";

                case "like":
                    return " like ";

                case "startwith":
                    return " like ";

                case "endwith":
                    return " like ";

                case "modulo":
                    return " % ";

                case "multiply":
                    return " * ";

                case "notequal":
                    return " <> ";

                case "subtract":
                    return " - ";

                case "and":
                    return " and ";

                case "or":
                    return " or ";

                case "in":
                    return " in ";

                case "notin":
                    return " not in ";

                default:
                    return " = ";
            }
        }

        #endregion 公共工具方法
    }
}