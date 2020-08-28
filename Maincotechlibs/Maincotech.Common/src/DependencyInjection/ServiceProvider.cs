using Autofac;
using System;

namespace Maincotech.DependencyInjection
{
    public class ServiceProvider : DisposableObject, IServiceProvider
    {
        private IContainer _Container;

        public ServiceProvider(IContainer container)
        {
            _Container = container;
        }

        public object GetService(Type serviceType)
        {
            return _Container.ResolveOptional(serviceType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _Container.Dispose();
            }
        }
    }
}