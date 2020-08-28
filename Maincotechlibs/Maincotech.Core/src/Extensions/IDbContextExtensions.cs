namespace Maincotech.EF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IDbContextExtensions
    {
        public static IEnumerable<string> GetTableNames(this IDbContext context)
        {
            var result = new List<string>();
            string script = context.GenerateCreateScript();
            if (!string.IsNullOrEmpty(script))
            {
                try
                {
                    var split = script.Split(new[] { "CREATE TABLE " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string sql in split)
                    {
                        var tableName = sql.Substring(0, sql.IndexOf("(", StringComparison.OrdinalIgnoreCase));
                        tableName = tableName.Split('.').Last();
                        tableName = tableName.Trim().TrimStart('[').TrimEnd(']');
                        result.Add(tableName);
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            return result;
        }
    }
}