using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Maincotech.Storage
{
    public class Blob
    {
        public IBlobStorage BlobStorage { get; set; }
        public string Identifier { get; set; }
        public BlobContainer Parent { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public PropertyBag Metadata { get; set; }
        public long Size { get; set; }
    }
}
