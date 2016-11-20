using Autofac;
using System;

namespace Gmich.Cedrus.IOC
{
    public class AutofacContainer : IContainer
    {

        private readonly Autofac.IContainer container;

        public AutofacContainer(Autofac.IContainer container)
        {
            this.container = container;
        }

        public object Resolve(Type serviceType) => container.Resolve(serviceType);
        public TService Resolve<TService>() => container.Resolve<TService>();

    }
}
