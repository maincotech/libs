using System.Reflection;

namespace Maincotech.Reflection.Fast
{
    public class MethodInvokerFactory : IFastReflectionFactory<MethodInfo, IMethodInvoker>
    {
        public IMethodInvoker Create(MethodInfo key)
        {
            return new MethodInvoker(key);
        }

        #region IFastReflectionFactory<MethodInfo,IMethodInvoker> Members

        IMethodInvoker IFastReflectionFactory<MethodInfo, IMethodInvoker>.Create(MethodInfo key)
        {
            return Create(key);
        }

        #endregion IFastReflectionFactory<MethodInfo,IMethodInvoker> Members
    }
}