using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.IOC
{
    public class Container : IContainer
    {
        private readonly Dictionary<Type, Func<object>> registrations = new Dictionary<Type, Func<object>>();

        public void Register<TService, TImpl>() where TImpl : TService
        {
            registrations.Add(typeof(TService), () => this.Resolve(typeof(TImpl)));
        }

        public TService Resolve<TService>() 
        {
            return (TService)Resolve(typeof(TService));
        }

        public object Resolve(Type serviceType)
        {
            Func<object> creator;
            if (registrations.TryGetValue(serviceType, out creator)) return creator();
            else if (!serviceType.IsAbstract) return this.CreateInstance(serviceType);
            else throw new InvalidOperationException("No registration for " + serviceType);
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().Single();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(t => this.Resolve(t)).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
