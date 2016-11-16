using Autofac;

namespace Gmich.Cedrus
{
    internal class TimeModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TimeManager>().AsSelf();
        }
    }
}
