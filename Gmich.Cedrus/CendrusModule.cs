using Autofac;
using System;
using System.Linq;
using System.Reflection;

namespace Gmich.Cedrus
{
    public class CendrusModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            RegisterModules(builder, assembly,
                module =>
                    module.Name.Equals("Module"));

        }

        private void RegisterModules(ContainerBuilder builder, Assembly assembly, Predicate<Type> rule)
        {
            var modules = assembly
            .GetTypes()
            .Where(type =>
                type.IsAssignableTo<Autofac.Module>()
                && rule(type))
            .Select(type =>
                (Autofac.Module)Activator.CreateInstance(type));

            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }
        }
    }
}
