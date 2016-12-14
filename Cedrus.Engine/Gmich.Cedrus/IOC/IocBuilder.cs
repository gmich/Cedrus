using Gmich.Cedrus.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Gmich.Cedrus.IOC
{

    [Flags]
    internal enum RegistrationTag
    {
        Default = 1,
        Lambda = 2,
        Singleton = 4,
        Scope = 8,
        Enumerable = 16
    }

    public class IocBuilder
    {
        private Dictionary<RegistrationKey, RegistrationItem> registrations = new Dictionary<RegistrationKey, RegistrationItem>();
        private IocContainer container;
        private RegistrationItem lastItem;

        public EventHandler<IContainer> OnBuild { get; set; }

        private class RegistrationKey
        {
            public RegistrationKey(Type type)
            {
                Type = type;
            }
            public Type Type { get; }
        }
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


        public IocBuilder Match<TAbstract>() => Match(typeof(TAbstract));

        public IocBuilder Match(Type type)
        {
            if (lastItem == null)
            {
                throw new CendrusIocException("No registration found for match");
            }

            registrations.Add(new RegistrationKey(type), lastItem);
            return this;
        }


        public IocBuilder RegisterModule<Module>(Module module)
              where Module : CendrusModule
        {
            module.Register(this);
            return this;
        }
        public IocBuilder RegisterModule<Module>()
            where Module : CendrusModule, new() => RegisterModule(new Module());

        public IocBuilder RegisterModules(Assembly assembly, Predicate<Type> rule = null)
        {
            var picker = rule ?? new Predicate<Type>(t => true);

            var modules = assembly
            .GetTypes()
            .Where(type =>
                type.IsSubclassOf(typeof(CendrusModule))
                && !type.IsAbstract
                && picker(type))
            .Select(type =>
                (CendrusModule)Activator.CreateInstance(type));

            foreach (var module in modules)
            {
                RegisterModule(module);
            }

            OnBuild += (sender, container) =>
            {
                var appender = container.Resolve<IAppender>()["Gmich.Cedrus.IOC"];
                foreach (var module in modules)
                {
                    appender.Debug($"Registered module {module.GetType().FullName}");
                }
            };

            return this;
        }

        public IocBuilder RegisterSingletonSubclassesOf(Assembly assembly, Type classType)
        {
            var method = GetType().GetMethods().Where(m => m.Name == "RegisterSingleton" && m.IsGenericMethodDefinition && m.GetGenericArguments().Count() == 2).First();
            var match = GetType().GetMethods().Where(m => m.Name == "Match" && !m.IsGenericMethodDefinition).First();

            foreach (var type in assembly
            .GetTypes()
            .Where(type =>
                type.IsSubclassOf(classType)
                && !type.IsAbstract))
            {
                var generic = method.MakeGenericMethod(classType, type);
                match.Invoke(generic.Invoke(this, new object[] { }), new[] { type });
            }

            return this;
        }

        public IocBuilder Register<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            return AddRegistration<TAbstract>(RegistrationTag.Default, () => CreateInstance(typeof(TAbstract), typeof(TImpl)));
        }

        public IocBuilder Register<TAbstract>(Func<IContainer, TAbstract> resolver)
        {
            return AddRegistration<TAbstract>(RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterPerScope<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            return AddRegistration<TAbstract>(RegistrationTag.Scope | RegistrationTag.Default, () => CreateInstance(typeof(TAbstract), typeof(TImpl)));
        }

        public IocBuilder RegisterPerScope<TAbstract>(Func<IContainer, TAbstract> resolver)
        {
            return AddRegistration<TAbstract>(RegistrationTag.Scope | RegistrationTag.Lambda, () => resolver(container));
        }

        public IocBuilder RegisterSingleton<TAbstract, TImpl>()
            where TImpl : TAbstract
        {
            var lazy = new Lazy<object>(() => CreateInstance(typeof(TAbstract), typeof(TImpl)));
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
            lastItem = new RegistrationItem(tag, lambda);
            registrations.Add(new RegistrationKey(type), lastItem);
            return this;
        }

        private Func<object> Resolve(Type serviceType)
        {
            var entry = registrations.FirstOrDefault(c => c.Key.Type == serviceType);

            if (entry.Value != null)
            {
                return GetNormalizedLambda(entry.Key.Type, registrations[entry.Key]);
            }
            if (!serviceType.IsAbstract)
            {
                return CreateInstance(serviceType, serviceType);
            }
            throw new CendrusIocException($"Unable to resolve abstract type {serviceType}. Component is not registered");
        }

        private Func<object> CreateInstance(Type abstractType, Type implementationType)
        {
            var ctor = implementationType.GetConstructors().FirstOrDefault();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType).ToArray();

            if (parameterTypes.Length == 0)
            {
                return () =>
                {
                    return Activator.CreateInstance(implementationType);
                };
            }

            var dependencies = parameterTypes
                .Select(t => Resolve(t))
                .ToArray();

            return () =>
            {
                return Activator.CreateInstance(implementationType, dependencies.Select(d => d.Invoke()).ToArray());
            };
        }

        private Func<object> GetNormalizedLambda(Type type, RegistrationItem item)
        {
            if (item.Resolved == null)
            {
                item.Resolved = Cache(type, Normalize(type, item));
            }
            return item.Resolved;
        }

        private Func<object> Cache(Type type, Func<object> cached)
        {
            return () =>
            {
                var obj = cached();
                container.ActiveContainer.AddResolved(type, obj);
                return obj;
            };
        }

        private Func<object> Normalize(Type type, RegistrationItem item)
        {
            if (item.RegistrationTag.HasFlag(RegistrationTag.Scope))
            {
                var lambda = item.RegistrationTag.HasFlag(RegistrationTag.Default) ? item.Lambda() : item.Lambda;
                return new Func<object>(() =>
                {
                    if (container.ActiveContainer.resolved.ContainsKey(type))
                    {
                        return container.ActiveContainer.resolved[type].First();
                    }
                    return ((Func<object>)lambda).Invoke();
                });
            }
            else if (item.RegistrationTag.HasFlag(RegistrationTag.Default))
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
            else if (item.RegistrationTag.HasFlag(RegistrationTag.Enumerable))
            {
                var dynamicCast = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(new[] { type.GetGenericArguments().First() });
                var lambda = (IEnumerable<Func<object>>)item.Lambda();

                return () => dynamicCast.Invoke(null, new[] { lambda.Select(c => c.Invoke()).ToArray() });
            }
            else
            {
                return () => item.Lambda();
            }
        }

        public void LogRegistrations()
        {
            OnBuild += (sender, container) =>
            {
                var appender = container.Resolve<IAppender>()["Gmich.Cedrus.IOC"];
                foreach (var registration in registrations.Keys)
                {
                    appender.Debug($"Registered abstract type {registration.Type.FullName}");
                }
            };
        }

        public IContainer Build()
        {
            var groupedRegistrations = registrations
                 .GroupBy(c => c.Key.Type);

            var expandedRegistrations = new Dictionary<RegistrationKey, RegistrationItem>();
            foreach (var entry in groupedRegistrations)
            {
                if (entry.Count() == 1)
                {
                    expandedRegistrations.Add(new RegistrationKey(entry.Key), entry.LastOrDefault().Value);
                }
                if (entry.Key.IsGenericType && entry.Key.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    continue;
                }
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(entry.Key);
                var item = new RegistrationItem(RegistrationTag.Enumerable, () => entry.Select(c => GetNormalizedLambda(entry.Key, c.Value)).ToArray());
                expandedRegistrations.Add(new RegistrationKey(enumerableType), item);
            }
            registrations = expandedRegistrations;
            var registrationDictionary = new Dictionary<Type, IocContainer.Entry>();
            foreach (var entry in expandedRegistrations)
            {
                try
                {
                    registrationDictionary.Add(entry.Key.Type, new IocContainer.Entry(entry.Value.RegistrationTag, GetNormalizedLambda(entry.Key.Type, entry.Value)));
                }
                catch (Exception ex)
                {
                    throw new CendrusIocException($"Failed to build {entry.Value.RegistrationTag} dependency {entry.Key.Type.FullName}. {ex.Message}", ex);
                }
            }

            container = new IocContainer(registrationDictionary);

            OnBuild?.Invoke(this, container);
            return container;
        }


    }

}
