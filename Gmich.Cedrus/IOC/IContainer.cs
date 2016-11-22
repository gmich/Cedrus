using System;

namespace Gmich.Cedrus.IOC
{
    public interface IContainer
    {
        IContainer ActiveContainer { get; }

        object BeginLifetime();
        object Resolve(Type serviceType);
        TService Resolve<TService>();
    }
}