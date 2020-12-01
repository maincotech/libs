namespace Maincotech.Storage
{
    public class Quota
    {
        public long Deleted { get; set; }
        public long Remaining { get; set; }
        public string State { get; set; }
        public long Total { get; set; }
        public long Used { get; set; }

        public PropertyBag AdditionalData { get; set; }
    }
}