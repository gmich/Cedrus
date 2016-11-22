using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gmich.Cedrus.IOC;

namespace Gmich.Cedrus.UnitTests.IOC
{
    [TestClass]
    public class IocTests
    {

        public interface IA { }
        public interface IB { }
        public interface IC { }

        public class A : IA { }
        public class B : IB { }
        public class C : IC { public C(IA a, IB b, IC c) { } }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void SimpleRegisterResovle()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();

            var container = builder.Build();

            var a = container.Resolve<IA>();

        }
    }
}
