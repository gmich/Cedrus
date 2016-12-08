using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{
    public class IocContainer : CleanedupEntity, IContainer
    {
        protected readonly Dictionary<Type, List<object>> resolved = new Dictionary<Type, List<object>>();
        protected readonly Dictionary<Type, Entry> registrations;

        public class Entry
        {
            internal Entry(RegistrationTag tag, Func<object> resolve)
            {
                Tag = tag;
                Resolve = resolve;
            }
            internal RegistrationTag Tag { get; }
            public Func<object> Resolve { get; }

        }
        public IocContainer(Dictionary<Type, Entry> registrations)
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
                var obj = InternalResolve(serviceType);
                AddResolved(serviceType, obj);
                return obj;
            }
            throw new CendrusIocException($"No registration for {serviceType.FullName}");
        }

        private void AddResolved(Type serviceType, object obj)
        {
            if (resolved.ContainsKey(serviceType))
            {
                resolved[serviceType].Add(obj);
            }
            else
            {
                resolved[serviceType] = new List<object>();
                resolved[serviceType].Add(obj);
            }
        }

        protected virtual object InternalResolve(Type serviceType) => registrations[serviceType].Resolve();

        public IContainer Scope => new IocScope(registrations);

        protected override void OnDisposal()
        {
            foreach (var obj in resolved.SelectMany(c => c.Value))
            {
                (obj as IDisposable)?.Dispose();
            }
            resolved.Clear();
        }

        private class IocScope : IocContainer
        {
            public IocScope(Dictionary<Type, Entry> registrations) : base(registrations)
            {
            }

            protected override object InternalResolve(Type serviceType)
            {
                if (registrations[serviceType].Tag.HasFlag(RegistrationTag.Scope))
                {
                    if (resolved.ContainsKey(serviceType))
                    {
                        return resolved[serviceType].Single();
                    }
                }
                return base.InternalResolve(serviceType);
            }
        }


    }


}
