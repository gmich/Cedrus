using System;

namespace Gmich.Cedrus.IOC
{
    public interface IContainer
    {
        object Resolve(Type serviceType);
        TService Resolve<TService>();
    }
}