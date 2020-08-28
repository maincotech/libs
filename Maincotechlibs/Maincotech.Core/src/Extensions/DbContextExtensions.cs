namespace Microsoft.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore.Internal;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public static class DbContextExtensions
    {
        public static IEnumerable<string> GetCurrentTables(this DbContext context)
        {
            var result = new List<string>();

            try
            {
                var connection = context.Database.GetDbConnection();

                bool isConnectionClosed = connection.State == ConnectionState.Closed;

                if (isConnectionClosed)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore
            }

            return result;
        }

        public static void EnsureTables(this DbContext context)
        {
            string script = context.Database.GenerateCreateScript(); // See issue #2943 for this extension method
            if (!string.IsNullOrEmpty(script))
            {
                try
                {
                    var connection = context.Database.GetDbConnection();

                    bool isConnectionClosed = connection.State == ConnectionState.Closed;

                    if (isConnectionClosed)
                    {
                        connection.Open();
                    }

                    var existingTableNames = new List<string>();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingTableNames.Add(reader.GetString(0).ToLowerInvariant());
                            }
                        }
                    }

                    var split = script.Split(new[] { "CREATE TABLE " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string sql in split)
                    {
                        var tableName = sql.Substring(0, sql.IndexOf("(", StringComparison.OrdinalIgnoreCase));
                        tableName = tableName.Split('.').Last();
                        tableName = tableName.Trim().TrimStart('[').TrimEnd(']').ToLowerInvariant();

                        if (existingTableNames.Contains(tableName))
                        {
                            continue;
                        }

                        try
                        {
                            using (var createCommand = connection.CreateCommand())
                            {
                                createCommand.CommandText = "CREATE TABLE " + sql.Substring(0, sql.LastIndexOf(";"));
                                createCommand.ExecuteNonQuery();
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore
                        }
                    }

                    if (isConnectionClosed)
                    {
                        connection.Close();
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }

        public static void DropTables(this DbContext context)
        {
            var tables = context.GetCurrentTables();
            if (tables.Any())
            {
                //DROP TABLE testing1,testing2,testing3; //retry 3 times

                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var connection = context.Database.GetDbConnection();

                        bool isConnectionClosed = connection.State == ConnectionState.Closed;

                        if (isConnectionClosed)
                        {
                            connection.Open();
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = $"DROP TABLE {string.Join(",", tables)}";
                            command.ExecuteNonQuery();
                        }
                        break;
                    }
                    catch (Exception)
                    {
                        // Ignore
                    }
                }
            }
        }
    }
}