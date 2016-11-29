using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{

    [Flags]
    internal enum RegistrationTag
    {
        Default = 1,
        Lambda = 2,
        Singleton = 4,
        Scope = 8
    }

    public class IocBuilder
    {
        private readonly Dictionary<Type, RegistrationItem> registrations = new Dictionary<Type, RegistrationItem>();
        private IContainer container;

        private class RegistrationItem
        {
            public Func<object> Lambda { get; }

            public Func<object> Resolved { get; internal set; }
            public RegistrationTag RegistrationTag { get; }

            public RegistrationItem(RegistrationTag tag, Func<object> lambda)
            {
                Lambda = lambda;
                RegistrationTag = tag;
            }
        }

        public IocBuilder Register<TService, TImpl>()
            where TImpl : TService
        {
            return AddRegistration<TService>(RegistrationTag.Default, () => CreateInstance(typeof(TImpl)));
        }

        public IocBuilder Register<TService>(Func<IContainer, TService> resolver)
        {
            return AddRegistration<TService>(RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterPerScope<TService, TImpl>()
            where TImpl : TService
        {
            return AddRegistration<TService>(RegistrationTag.Scope | RegistrationTag.Default, () => CreateInstance(typeof(TImpl)));
        }

        public IocBuilder RegisterPerScope<TService>(Func<IContainer, TService> resolver)
        {
            return AddRegistration<TService>(RegistrationTag.Scope | RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterSingleton<TService, TImpl>()
            where TImpl : TService
        {
            var lazy = new Lazy<object>(() => CreateInstance(typeof(TImpl)));
            return AddRegistration<TService>(RegistrationTag.Singleton, () => lazy.Value);
        }

        public IocBuilder RegisterSingleton<TService>(Func<IContainer, TService> instanceCreator)
        {
            var lazy = new Lazy<object>(() => instanceCreator(container));
            return AddRegistration<TService>(RegistrationTag.Lambda, () => lazy.Value);
        }

        private IocBuilder AddRegistration<TService>(RegistrationTag tag, Func<object> lambda)
        {
            var type = typeof(TService);
            if (registrations.ContainsKey(type))
            {
                throw new CendrusIocException($"${type.FullName} is already registered");
            }
            registrations.Add(type, new RegistrationItem(tag, lambda));
            return this;
        }

        private Func<object> Resolve(Type serviceType)
        {
            if (registrations.ContainsKey(serviceType))
            {
                return GetNormalizedLambda(registrations[serviceType]);
            }
            if (!serviceType.IsAbstract)
            {
                return CreateInstance(serviceType);
            }
            throw new CendrusIocException($"Unable to resolve abstract type {serviceType}. Component is not registered");
        }

        private Func<object> CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().FirstOrDefault();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray();

            if (parameterTypes.Length == 0)
            {
                return () =>
                    Activator.CreateInstance(implementationType);
            }

            var dependencies = parameterTypes
                .Select(t => Resolve(t))
                .ToArray();

            return () =>
                Activator.CreateInstance(implementationType, dependencies.Select(d => d.Invoke()).ToArray());
        }

        private Func<object> GetNormalizedLambda(RegistrationItem item)
        {
            if (item.Resolved == null)
            {
                item.Resolved = Normalize(item);
            }
            return item.Resolved;
        }

        private Func<object> Normalize(RegistrationItem item)
        {
            if (item.RegistrationTag.HasFlag(RegistrationTag.Default))
            {
                return (Func<object>)item.Lambda();
            }
            else if (item.RegistrationTag.HasFlag(RegistrationTag.Lambda))
            {
                return item.Lambda;
            }
            else if (item.RegistrationTag.HasFlag(RegistrationTag.Singleton))
            {
                var resolve = (Func<object>)item.Lambda();
                var lazy = new Lazy<object>(resolve);
                return new Func<object>(() => lazy.Value);
            }
            else
            {
                return () => item.Lambda();
            }
        }

        public IContainer Build()
        {
            container = new IocContainer(registrations
                .ToDictionary(c => c.Key, c => new IocContainer.Entry(c.Value.RegistrationTag, GetNormalizedLambda(c.Value))));
            return container;
        }
    }

}
