namespace Maincotech.Reflection.Fast
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IFastReflectionCache<TKey, TValue>
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Get(TKey key);
    }
}