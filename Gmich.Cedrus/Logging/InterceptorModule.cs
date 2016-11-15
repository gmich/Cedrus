using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.Logging
{
    public class InterceptorModule : Module
    {
        private const string InterceptorsPropertyName = "Autofac.Extras.DynamicProxy2.RegistrationExtensions.InterceptorsPropertyName";
        private readonly Type[] interceptorTypes = new[] { typeof(SimpleLogInterceptor) };

        protected override void Load(ContainerBuilder builder)
        {
#if DEBUG
            foreach (var type in interceptorTypes)
            {
                builder.RegisterType(type);
            }
#endif
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var interceptorServices = interceptorTypes
                .Select(type => new TypedService(type))
                .ToArray<Service>();

            object existing;
            if (registration.Metadata.TryGetValue(InterceptorsPropertyName, out existing))
            {
                registration.Metadata[InterceptorsPropertyName] =
                  ((IEnumerable<Service>)existing).Concat(interceptorServices).Distinct();
            }
            else
            {
                registration.Metadata.Add(InterceptorsPropertyName, interceptorServices);
            }
        }
    }
}
