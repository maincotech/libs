﻿namespace Maincotech.Reflection.Fast
{
    public interface IFastReflectionFactory<TKey, TValue>
    {
        TValue Create(TKey key);
    }
}