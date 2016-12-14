using System;

namespace Gmich.Cedrus.IOC
{

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
    public class IocKeyAttribute : Attribute
    {
        public IocKeyAttribute(object key)
        {
            Key = key;
        }
        public object Key { get; }
    }
}
