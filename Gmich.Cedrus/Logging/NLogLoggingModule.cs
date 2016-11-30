using Gmich.Cedrus.IOC;
using NLog;
using NLog.Config;

namespace Gmich.Cedrus.Logging
{
    internal class NLogLoggingModule : CendrusModule
    {
        public override void Register(IocBuilder builder)
        {
            LogManager.Configuration = new XmlLoggingConfiguration("log.config");

            builder
                .Register<IAppender, NLogAppender>()
                .RegisterPerScope<LoggingContext, LoggingContext>();
        }
    }
}
