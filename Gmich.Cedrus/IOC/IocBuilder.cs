using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{
    public class IocBuilder
    {
        private readonly Dictionary<Type, RegistrationItem> registrations = new Dictionary<Type, RegistrationItem>();
        private IContainer container;

        private class RegistrationItem
        {
            public Func<object> Lambda { get; }
            public Tag RegistrationTag { get; }

            public RegistrationItem(Tag tag, Func<object> lambda)
            {
                Lambda = lambda;
                RegistrationTag = tag;
            }

            [Flags]
            public enum Tag
            {
                Default = 1,
                Lambda = 2,
                Singleton = 4,
            }
        }

        public IocBuilder Register<TService, TImpl>()
            where TImpl : TService
        {
            return AddRegistration<TService>(RegistrationItem.Tag.Default, CreateInstance(typeof(TImpl)));
        }

        public IocBuilder Register<TService>(Func<IContainer, TService> resolver)
        {
            return AddRegistration<TService>(RegistrationItem.Tag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterSingleton<TService, TImpl>()
            where TImpl : TService
        {
            var lazy = new Lazy<object>(CreateInstance(typeof(TImpl)));
            return AddRegistration<TService>(RegistrationItem.Tag.Singleton, () => lazy.Value);
        }

        public IocBuilder RegisterSingleton<TService>(Func<IContainer, TService> instanceCreator)
        {
            var lazy = new Lazy<object>(() => instanceCreator(container));
            return AddRegistration<TService>(RegistrationItem.Tag.Singleton, () => lazy.Value);
        }

        private IocBuilder AddRegistration<TService>(RegistrationItem.Tag tag, Func<object> lambda)
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
                if (registrations[serviceType].RegistrationTag == RegistrationItem.Tag.Singleton)
                {
                    return () => registrations[serviceType].Lambda;
                }
                return () => registrations[serviceType].Lambda;
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
                .Select(c => c.Invoke())
                .Cast<Func<object>>()
                .ToArray();

            return () =>
                Activator.CreateInstance(implementationType, dependencies.Select(d => d.Invoke()).ToArray());
        }

        public IContainer Build()
        {
            container = new IocContainer(registrations.ToDictionary(item => item.Key, item => item.Value.Lambda));
            return container;
        }
    }

}
