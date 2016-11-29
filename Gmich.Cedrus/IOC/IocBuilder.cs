using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public IocBuilder RegisterModule<Module>(Module module)
              where Module : CendrusModule
        {
            module.Register(this);
            return this;
        }
        public IocBuilder RegisterModule<Module>()
            where Module : CendrusModule, new() => RegisterModule(new Module());

        public IocBuilder RegisterModules(Assembly assembly, Predicate<Type> rule)
        {
            var modules = assembly
            .GetTypes()
            .Where(type =>
                type.IsAssignableFrom(typeof(CendrusModule))
                && rule(type))
            .Select(type =>
                (CendrusModule)Activator.CreateInstance(type));

            foreach (var module in modules)
            {
                RegisterModule(module);
            }
            return this;
        }

        public IocBuilder Register<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            return AddRegistration<TAbstract>(RegistrationTag.Default, () => CreateInstance(typeof(TImpl)));
        }

        public IocBuilder Register<TAbstract>(Func<IContainer, TAbstract> resolver)
        {
            return AddRegistration<TAbstract>(RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterPerScope<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            return AddRegistration<TAbstract>(RegistrationTag.Scope | RegistrationTag.Default, () => CreateInstance(typeof(TImpl)));
        }

        public IocBuilder RegisterPerScope<TAbstract>(Func<IContainer, TAbstract> resolver)
        {
            return AddRegistration<TAbstract>(RegistrationTag.Scope | RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterSingleton<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            var lazy = new Lazy<object>(() => CreateInstance(typeof(TImpl)));
            return AddRegistration<TAbstract>(RegistrationTag.Singleton, () => lazy.Value);
        }

        public IocBuilder RegisterSingleton<TAbstract>(Func<IContainer, TAbstract> instanceCreator)
        {
            var lazy = new Lazy<object>(() => instanceCreator(container));
            return AddRegistration<TAbstract>(RegistrationTag.Lambda, () => lazy.Value);
        }

        private IocBuilder AddRegistration<TAbstract>(RegistrationTag tag, Func<object> lambda)
        {
            var type = typeof(TAbstract);
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
