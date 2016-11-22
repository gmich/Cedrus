using System;
using System.Collections.Generic;

namespace Gmich.Cedrus.IOC
{
    public class IocContainer : CleanedupEntity, IContainer
    {
        private readonly Dictionary<Type, Func<object>> registrations = new Dictionary<Type, Func<object>>();
        private readonly List<object> resolved = new List<object>();

        public IocContainer(Dictionary<Type, Func<object>> registrations)
        {
            this.registrations = registrations;
        }

        public TService Resolve<TService>()
        {
            return (TService)Resolve(typeof(TService));
        }

        public object Resolve(Type serviceType)
        {
            if (registrations.ContainsKey(serviceType))
            {
                var obj = registrations[serviceType]();
                resolved.Add(obj);
                return obj;
            }
            throw new CendrusIocException($"No registration for {serviceType.FullName}");
        }

        public IContainer Scope => new IocContainer(registrations);

        protected override void OnDisposal()
        {
            foreach (var obj in resolved)
            {
                (resolved as IDisposable)?.Dispose();
            }
            resolved.Clear();
        }

    }

}
