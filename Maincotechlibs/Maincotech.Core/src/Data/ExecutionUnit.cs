using System.Data.Common;

namespace Maincotech.Data
{
    public class ExecutionUnit
    {
        public string Name { get; set; }

        public DbParameter[] Parameters { get; set; }
    }
}