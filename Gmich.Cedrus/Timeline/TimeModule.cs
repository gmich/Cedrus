using Gmich.Cedrus.IOC;

namespace Gmich.Cedrus
{
    internal class TimeModule : CendrusModule
    {
        public override void Register(IocBuilder builder)
        {
            builder.Register<GameTimeline, GameTimeline>();
        }

    }
}
