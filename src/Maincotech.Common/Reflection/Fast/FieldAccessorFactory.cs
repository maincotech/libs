using System.Reflection;

namespace MaincoTech.Reflection.Fast
{
    public class FieldAccessorFactory : IFastReflectionFactory<FieldInfo, IFieldAccessor>
    {
        public IFieldAccessor Create(FieldInfo key)
        {
            return new FieldAccessor(key);
        }

        #region IFastReflectionFactory<FieldInfo,IFieldAccessor> Members

        IFieldAccessor IFastReflectionFactory<FieldInfo, IFieldAccessor>.Create(FieldInfo key)
        {
            return Create(key);
        }

        #endregion IFastReflectionFactory<FieldInfo,IFieldAccessor> Members
    }
}