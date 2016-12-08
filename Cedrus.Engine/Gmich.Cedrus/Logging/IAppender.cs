using System;

namespace Gmich.Cedrus.Logging
{
    public interface IAppender 
    {
        IAppender this[Type logger] { get; }

        IAppender this[string logger] { get; }

        LoggingContext LoggingContext { get; }

        void Trace(string message);
        void Trace(string message, params object[] args);

        void Info(string message);
        void Info(string message, params object[] args);

        void Debug(string message);
        void Debug(string message, params object[] args);

        void Warn(string message);
        void Warn(string message, params object[] args);

        void Error(string message);
        void Error(string message, params object[] args);

        void Fatal(string message);
        void Fatal(string message, params object[] args);
    }
}
