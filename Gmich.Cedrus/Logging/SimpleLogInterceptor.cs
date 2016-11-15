using Castle.DynamicProxy;
using System;

namespace Gmich.Cedrus.Logging
{

    public class SimpleLogInterceptor : IInterceptor
    {
        private readonly IAppender appender;

        public SimpleLogInterceptor(IAppender appender)
        {
            this.appender = appender["Gmich.Cedrus.Interceptor"];
        }

        public void Intercept(IInvocation invocation)
        {
            appender.Trace($"Entering {GetMethodInformation(invocation)}");
            invocation.Proceed();
            appender.Trace($"Exiting {GetMethodInformation(invocation)}");

        }

        private string GetMethodInformation(IInvocation invocation)
        {
            return String.Format(
              "{0}.{1} ",
              invocation.Method.DeclaringType,
              invocation.Method.Name);
        }

    }
}