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

            public Func<object> Resolved { get; internal set; }
            public Tag RegistrationTag { get; }

            public RegistrationItem(Tag tag, Func<object> lambda)
            {
                Lambda = lambda;
                RegistrationTag = tag;
            }

            public enum Tag
            {
                Default,
                Lambda,
                Singleton,
            }
        }

        public IocBuilder Register<TService, TImpl>()
            where TImpl : TService
        {
            return AddRegistration<TService>(RegistrationItem.Tag.Default, () => CreateInstance(typeof(TImpl)));
        }

        public IocBuilder Register<TService>(Func<IContainer, TService> resolver)
        {
            return AddRegistration<TService>(RegistrationItem.Tag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterSingleton<TService, TImpl>()
            where TImpl : TService
        {
            var lazy = new Lazy<object>(() => CreateInstance(typeof(TImpl)));
            return AddRegistration<TService>(RegistrationItem.Tag.Singleton, () => lazy.Value);
        }

        public IocBuilder RegisterSingleton<TService>(Func<IContainer, TService> instanceCreator)
        {
            var lazy = new Lazy<object>(() => instanceCreator(container));
            return AddRegistration<TService>(RegistrationItem.Tag.Lambda, () => lazy.Value);
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
                return NormalizeLambda(registrations[serviceType]);
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

        private Func<object> NormalizeLambda(RegistrationItem item)
        {
            if (item.Resolved != null)
            {
                return item.Resolved;
            }
            switch (item.RegistrationTag)
            {
                case RegistrationItem.Tag.Default:
                    return () => item.Lambda();
                case RegistrationItem.Tag.Singleton:
                    var resolve = (Func<object>)item.Lambda();
                    var lazy = new Lazy<object>(resolve);
                    item.Resolved = () => new Func<object>(() => lazy.Value);
                    return item.Resolved;
                default:
                    return () => item.Lambda;
            }
        }

        public IContainer Build()
        {
            var containerRegistrations = new Dictionary<Type, Func<object>>();

            foreach (var registration in registrations)
            {
                switch (registration.Value.RegistrationTag)
                {
                    case RegistrationItem.Tag.Default:
                        containerRegistrations.Add(registration.Key, (Func<object>)registration.Value.Lambda());
                        break;
                    case RegistrationItem.Tag.Lambda:
                        containerRegistrations.Add(registration.Key, registration.Value.Lambda);
                        break;
                    case RegistrationItem.Tag.Singleton:
                        if (registration.Value.Resolved == null)
                        {
                            var resolve = registration.Value.Lambda();
                            Lazy<object> lazy = new Lazy<object>((Func<object>)resolve);
                            containerRegistrations.Add(registration.Key, () => lazy.Value);
                        }
                        else
                        {
                            containerRegistrations.Add(registration.Key, (Func<object>)registration.Value.Resolved());
                        }
                        break;
                }
            }
            container = new IocContainer(containerRegistrations);
            return container;
        }
    }

}
