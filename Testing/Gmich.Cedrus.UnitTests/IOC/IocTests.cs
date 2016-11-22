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
        public interface ID { }

        public class A : IA { }
        public class B : IB { }
        public class C : IC { public C(IA a, IB b) { } }

        public class D : ID { public D(IA a, IB b, IC c) { } }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void SimpleResolve()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();

            var container = builder.Build();

            var a = container.Resolve<IA>();

        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void AdvancedResolve()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            var container = builder.Build();

            var a = container.Resolve<IC>();
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void DeepObjectGraphResolve()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            var container = builder.Build();

            var d = container.Resolve<ID>();
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        [ExpectedException(typeof(CendrusIocException))]
        public void MissingRegistrationThrowsExceptionOnContainerBuild()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            var container = builder.Build();
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveSelfRegistered()
        {
            var builder = new IocBuilder();

            builder.Register<A, A>();
            var container = builder.Build();

            var a = container.Resolve<A>();
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveLambda()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>(c => new C(c.Resolve<IA>(), c.Resolve<IB>()));
            var container = builder.Build();

            var ic = container.Resolve<IC>();
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveSingleton()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>();
            var container = builder.Build();

            var a1 = container.Resolve<IA>();
            var a2 = container.Resolve<IA>();

            Assert.AreEqual(a1, a2);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveSingletonLambda()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>(c => new A());
            var container = builder.Build();

            var a1 = container.Resolve<IA>();
            var a2 = container.Resolve<IA>();

            Assert.AreEqual(a1, a2);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        [ExpectedException(typeof(CendrusIocException))]
        public void DoubleRegistrationThrowsException()
        {
            var builder = new IocBuilder();

            builder.Register<A, A>();
            builder.Register<A, A>();
        }
    }
}
