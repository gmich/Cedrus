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
        protected readonly Dictionary<Keyed, Entry> keyedRegistrations;
        private IocContainer child;

        internal IocContainer ActiveContainer => child?.ActiveContainer ?? this;

        public class Keyed : Tuple<Type, object>
        {
            public Keyed(Type item1, object item2) : base(item1, item2)
            {
            }
        }

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
        public IocContainer(Dictionary<Type, Entry> registrations, Dictionary<Keyed, Entry> keyedRegistrations)
        {
            this.registrations = registrations;
            this.keyedRegistrations = keyedRegistrations;
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

        public TService ResolveWithId<TService>(object id)
        {
            return (TService)ResolveWithId(typeof(TService), id);
        }

        public object ResolveWithId(Type serviceType, object id)
        {
            var key = new Keyed(serviceType, id);
            if (keyedRegistrations.ContainsKey(key))
            {
                return keyedRegistrations[key].Resolve();
            }
            throw new CendrusIocException($"No registration for {serviceType.FullName} and id {id.ToString()}");
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
                child = new IocContainer(registrations,keyedRegistrations);
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