using System;

namespace Gmich.Cedrus.IOC
{
    public interface IContainer : IDisposable
    {
        object Resolve(Type serviceType);
        TService Resolve<TService>();
        IContainer Scope { get; }
    }
}