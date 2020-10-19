namespace Maincotech.Storage
{
    public class BlobContainer
    {
        public IBlobStorage BlobStorage { get; set; }
        public string Identifier { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }
        public BlobContainer ParentContainer { get; set; }

        public string Uri { get; set; }

        public PropertyBag Metadata { get; set; }
    }
}