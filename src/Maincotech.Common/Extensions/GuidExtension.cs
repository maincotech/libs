namespace System
{
    public static class GuidExtension
    {
        public static bool IsNullOrEmpty(this Guid id)
        {
            if (id == Guid.Empty)
            {
                return true;
            }
            return false;
        }
    }
}