using System.Collections.Generic;

namespace Maincotech.ModuleModel
{
    public class ModuleInfo
    {
        public bool Enabled { get; set; }
        public string Identity { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public List<string> Dependencies { get; set; }
    }
}