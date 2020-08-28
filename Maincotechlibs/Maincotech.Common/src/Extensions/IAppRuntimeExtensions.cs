using Maincotech.Logging;

namespace Maincotech
{
    public static class IAppRuntimeExtensions
    {
        public static T Resolve<T>(this IAppRuntime @this)
        {
            return (T)@this.ServiceProvider?.GetService(typeof(T));
        }

        public static TTarget Adapt<TTarget>(this IAppRuntime @this, object source)
            where TTarget : class, new()
        {
            var adpter = @this.TypeAdapterFactory.Create();
            return adpter.Adapt<TTarget>(source);
        }

        public static TTarget To<TTarget>(this object @this) where TTarget : class, new()
        {
            return AppRuntimeContext.Current.Adapt<TTarget>(@this);
        }

        public static ILogger GetLogger<T>(this IAppRuntime @this)
        {
            var type = typeof(T);
            return @this.LoggerFactory.CreateLog(type);
        }
    }
}