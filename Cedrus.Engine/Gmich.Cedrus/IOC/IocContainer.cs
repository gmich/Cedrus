using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{
    public class IocContainer : CleanedupEntity, IContainer
    {
        internal readonly Dictionary<Type, List<object>> resolved = new Dictionary<Type, List<object>>();
        protected readonly Dictionary<Type, Entry> registrations;
        private IocContainer child;

        internal IocContainer ActiveContainer => child?.ActiveContainer ?? this;

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
                return InternalResolve(serviceType);
            }
            throw new CendrusIocException($"No registration for {serviceType.FullName}");
        }

        internal void AddResolved(Type serviceType, object obj)
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

        public IContainer Scope
        {
            get
            {
                child = new IocContainer(registrations);
                return child;
            }
        }

        protected override void OnDisposal()
        {
            foreach (var obj in resolved.SelectMany(c => c.Value))
            {
                (obj as IDisposable)?.Dispose();
            }
            resolved.Clear();
        }



    }

}