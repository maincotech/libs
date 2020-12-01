using System;
using System.Threading;

namespace Maincotech.Threading
{
    /// <summary>
    /// Provides a convenience methodology for implementing locked access to resources. 
    /// </summary>
    /// <remarks>
    /// Intended as an infrastructure class.
    /// </remarks>
    public class ReaderWriterLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;
        private readonly ReaderWriterLockType _readerWriteLockType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReaderWriterLockDisposable"/> class.
        /// </summary>
        /// <param name="rwLock">The readers–writer lock</param>
        /// <param name="readerWriteLockType">Lock type</param>
        public ReaderWriterLockDisposable(ReaderWriterLockSlim rwLock, ReaderWriterLockType readerWriteLockType = ReaderWriterLockType.Write)
        {
            _rwLock = rwLock;
            _readerWriteLockType = readerWriteLockType;

            switch (_readerWriteLockType)
            {
                case ReaderWriterLockType.Read:
                    _rwLock.EnterReadLock();
                    break;
                case ReaderWriterLockType.Write:
                    _rwLock.EnterWriteLock();
                    break;
                case ReaderWriterLockType.UpgradeableRead:
                    _rwLock.EnterUpgradeableReadLock();
                    break;
            }
        }

        void IDisposable.Dispose()
        {
            switch (_readerWriteLockType)
            {
                case ReaderWriterLockType.Read:
                    _rwLock.ExitReadLock();
                    break;
                case ReaderWriterLockType.Write:
                    _rwLock.ExitWriteLock();
                    break;
                case ReaderWriterLockType.UpgradeableRead:
                    _rwLock.ExitUpgradeableReadLock();
                    break;
            }
        }
    }
}
