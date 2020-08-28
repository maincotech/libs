namespace Maincotech.Threading
{
    /// <summary>
    /// Reader/Writer locker type
    /// </summary>
    public enum ReaderWriterLockType
    {
        Read,
        Write,
        UpgradeableRead
    }
}