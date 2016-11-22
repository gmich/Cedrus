using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{

    public class CendrusIocException : Exception
    {
        public CendrusIocException(string message) : base(message)
        {
        }
    }

    public class IocContainer : IContainer
    {
        private readonly Dictionary<Type, Func<object>> registrations = new Dictionary<Type, Func<object>>();

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
                return registrations[serviceType]();
            }
            throw new CendrusIocException("No registration for " + serviceType);
        }

        public object BeginLifetime()
        {
            throw new NotImplementedException();
        }

        public IContainer ActiveContainer => this;
    }


    public class IocBuilder
    {
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

        private readonly Dictionary<Type, RegistrationItem> registrations = new Dictionary<Type, RegistrationItem>();
        private IContainer container;

        public void Register<TService, TImpl>()
            where TImpl : TService
        {
            AddRegistration<TService, TImpl>(RegistrationItem.Tag.Default, CreateInstance(typeof(TImpl)));
        }

        public void Register<TService, TImpl>(Func<IContainer, TImpl> resolver)
        {
            AddRegistration<TService, TImpl>(RegistrationItem.Tag.Lambda, () => resolver(container.ActiveContainer));
        }

        public void RegisterSingleton<TService, TImpl>()
            where TImpl : TService
        {
            AddRegistration<TService, TImpl>(RegistrationItem.Tag.Singleton, CreateInstance(typeof(TImpl)));
        }

        public void RegisterSingleton<TService, TImpl>(Func<IContainer, TImpl> instanceCreator)
            where TImpl : TService
        {
            AddRegistration<TService, TImpl>(RegistrationItem.Tag.Singleton, () => instanceCreator(container.ActiveContainer));
        }

        private void AddRegistration<TService, TImpl>(RegistrationItem.Tag tag, Func<object> lambda)
        {
            var type = typeof(TService);
            if (registrations.ContainsKey(type))
            {
                throw new CendrusIocException($"${type.FullName} is already registered");
            }
            registrations.Add(type, new RegistrationItem(tag, lambda));
        }

        private Func<object> Resolve(Type serviceType)
        {
            if (registrations.ContainsKey(serviceType))
            {
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
            var containerRegistrations = new Dictionary<Type, Func<object>>();

            foreach (var registration in registrations)
            {
                switch (registration.Value.RegistrationTag)
                {
                    case RegistrationItem.Tag.Default:
                    case RegistrationItem.Tag.Lambda:
                        containerRegistrations.Add(registration.Key, registration.Value.Lambda);
                        break;
                    case RegistrationItem.Tag.Singleton:
                        var lazy = new Lazy<object>(registration.Value.Lambda);
                        containerRegistrations.Add(registration.Key, () => lazy.Value);
                        break;
                }
            }
            container = new IocContainer(containerRegistrations);
            return container;
        }
    }
}
