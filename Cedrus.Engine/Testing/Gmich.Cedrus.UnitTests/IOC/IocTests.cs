using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gmich.Cedrus.IOC;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Gmich.Cedrus.UnitTests.IOC
{
    [TestClass]
    public class IocTests
    {
        public interface IA { }
        public interface IB { }
        public interface IC { IB B { get; } }
        public interface ID
        {
            IA A { get; }
            IB B { get; }
        }
        public interface IE : IDisposable { }

        public class A : IA { }
        public class B : IB { }
        public class C : IC
        {
            public C(IA a, IB b)
            {
                B = b;
            }
            public IB B { get; }
        }

        public class D : ID
        {
            public D(IA a, IB b, IC c) { A = a; B = b; }

            public IA A { get; }

            public IB B { get; }
        }
        public class E : IE
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public interface IF
        {
            IEnumerable<ID> Ds { get; }
        }
        public class F : IF
        {
            public F(IEnumerable<ID> ds)
            {
                Ds = ds;
            }

            public IEnumerable<ID> Ds { get; }

        }


        [TestMethod]
        [TestCategory(Category.IOC)]
        public void SimpleResolve()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();

            var container = builder.Build();

            var a = container.Resolve<IA>();
            var b = container.Resolve<IA>();

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveMultipleOfSameType()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IA, A>();
            var container = builder.Build();

            var a = container.Resolve<IEnumerable<IA>>().ToArray();

            Assert.AreEqual(2, a.Count());
            Assert.AreNotEqual(a[0], a[1]);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveMultipleOfSameTypeAsParameter()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            builder.Register<ID, D>();
            builder.Register<IF, F>();
            var container = builder.Build();

            var f1 = container.Resolve<IF>();

        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveMultipleWithSingleton()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            builder.Register<ID, D>();
            builder.Register<IF, F>();
            var container = builder.Build();

            var f1 = container.Resolve<IF>();
            var f2 = container.Resolve<IF>();

            Assert.AreNotEqual(f1, f2);
            Assert.AreNotEqual(f1.Ds.FirstOrDefault().B, f2.Ds.FirstOrDefault().B);

            Assert.AreEqual(f1.Ds.FirstOrDefault().A, f2.Ds.FirstOrDefault().A);
            CollectionAssert.AreEqual(f1.Ds.Select(c => c.A).ToArray(), f2.Ds.Select(c => c.A).ToArray());
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveMultipleWithSingletonAndMatch()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>().Match<A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            builder.Register<ID, D>();
            builder.RegisterSingleton<ID, D>().Match<D>();
            builder.Register<IF, F>();
            var container = builder.Build();

            var f1 = container.Resolve<IF>();
            var f2 = container.Resolve<IF>();
            var a = container.Resolve<A>();
            var d = container.Resolve<D>();
            Assert.AreNotEqual(f1, f2);
            Assert.AreNotEqual(f1.Ds.FirstOrDefault().B, f2.Ds.FirstOrDefault().B);

            Assert.AreEqual(f1.Ds.FirstOrDefault().A, f2.Ds.FirstOrDefault().A);
            CollectionAssert.AreEqual(f1.Ds.Select(c => c.A).ToArray(), f2.Ds.Select(c => c.A).ToArray());
            Assert.AreEqual(f1.Ds.FirstOrDefault().A, a);
            Assert.AreEqual(f1.Ds.LastOrDefault(), d);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveMultipelWithScope()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>();
            builder.Register<IB, B>();
            builder.Register<IC, C>();
            builder.Register<ID, D>();
            builder.Register<ID, D>();
            builder.RegisterPerScope<ID, D>();


            builder.Register<IF, F>();
            var container = builder.Build();

            IEnumerable<ID> d1;
            using (var scope = container.Scope)
            {
                d1 = scope.Resolve<IEnumerable<ID>>();
                var d2 = scope.Resolve<IEnumerable<ID>>();
                //Assert.AreNotEqual(d1.FirstOrDefault().GetHashCode(), d2.LastOrDefault().GetHashCode());   
                Assert.AreEqual(d1.LastOrDefault(), d2.LastOrDefault());
            }
            var d3 = container.Resolve<IEnumerable<ID>>();
            Assert.AreNotEqual(d1.FirstOrDefault(), d3.LastOrDefault());
            Assert.AreNotEqual(d1.LastOrDefault(), d3.LastOrDefault());
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
            var b = container.Resolve<IC>();

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void AdvancedResolveWithSingleton()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IC, C>();
            builder.RegisterSingleton<IB, B>();
            var container = builder.Build();

            var a = container.Resolve<IC>();
            var b = container.Resolve<IC>();

            Assert.AreNotEqual(a, b);
            Assert.AreEqual(a.B, b.B);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void AdvancedResolveWithSingletonLambda()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.RegisterSingleton<IB>(c => new B());
            builder.Register<IC, C>();
            var container = builder.Build();

            var a = container.Resolve<IC>();
            var b = container.Resolve<IC>();

            Assert.AreEqual(a.B, b.B);
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
            var e = container.Resolve<ID>();

            Assert.AreNotEqual(d, e);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void DeepObjectGraphWithWrongOrderResolve()
        {
            var builder = new IocBuilder();

            builder.Register<ID, D>();
            builder.Register<IA, A>();
            builder.Register<IC, C>();
            builder.Register<IB, B>();
            var container = builder.Build();

            var d = container.Resolve<ID>();
            var e = container.Resolve<ID>();

            Assert.AreNotEqual(d, e);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void DeepObjectGraphWithWrongOrderAndSingletonResolve()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<ID, D>();
            builder.Register<IA, A>();
            builder.Register<IC, C>();
            builder.Register<IB, B>();
            var container = builder.Build();

            var d1 = container.Resolve<ID>();
            var d2 = container.Resolve<ID>();

            Assert.AreEqual(d1, d2);
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
            var b = container.Resolve<A>();

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveLambda()
        {
            var builder = new IocBuilder();

            builder.Register<IA, A>();
            builder.Register<IC>(c => new C(c.Resolve<IA>(), c.Resolve<IB>()));
            builder.Register<IB, B>();
            var container = builder.Build();

            var ic = container.Resolve<IC>();
            var id = container.Resolve<IC>();

            Assert.AreNotEqual(ic, id);
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
        public void ResolveSingletonWithMatch()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA, A>().Match<A>();
            var container = builder.Build();

            var a1 = container.Resolve<IA>();
            var a2 = container.Resolve<IA>();
            var a3 = container.Resolve<A>();
            Assert.AreEqual(a1, a2);
            Assert.AreEqual(a2, a3);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveSingletonWithDependencies()
        {
            var container = new IocBuilder()
            .Register<IA, A>()
            .Register<IB, B>()
            .Register<IC, C>()
            .RegisterSingleton<ID, D>()
            .Build();

            var d1 = container.Resolve<ID>();
            var d2 = container.Resolve<ID>();

            Assert.AreEqual(d1, d2);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ResolveSingletonLambda()
        {
            var builder = new IocBuilder();

            builder.RegisterSingleton<IA>(c => new A());
            var container = builder.Build();

            var a1 = container.Resolve<IA>();
            var a2 = container.Resolve<IA>();

            Assert.AreEqual(a1, a2);
        }



        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ScopeDisposesResolvedComponents()
        {
            var builder = new IocBuilder();

            builder.Register<E, E>();

            var container = builder.Build();

            E a = null;
            using (var scope = container.Scope)
            {
                a = scope.Resolve<E>();
                Assert.IsFalse(a.IsDisposed);
            }
            Assert.IsTrue(a.IsDisposed);
        }


        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ScopeResolvesAndDisposesCorrectly()
        {
            var container = new IocBuilder()
            .Register<IA, A>()
            .RegisterPerScope<IB, B>()
            .Build();

            IA a1 = null;
            IA a2 = null;
            IB b1 = null;
            IB b2 = null;
            using (var scope = container.Scope)
            {
                a1 = scope.Resolve<IA>();
                a2 = scope.Resolve<IA>();
                b1 = scope.Resolve<IB>();
                b2 = scope.Resolve<IB>();
            }
            Assert.AreNotEqual(a1, a2);
            Assert.AreEqual(b1, b2);

            b1 = container.Resolve<IB>();

            Assert.AreNotEqual(b1, b2);
        }

        [TestMethod]
        [TestCategory(Category.IOC)]
        public void ScopeResolvesAndDisposesLambdaCorrectly()
        {
            var container = new IocBuilder()
            .Register<IA, A>()
            .RegisterPerScope<IB>(c => new B())
            .Build();

            IA a1 = null;
            IA a2 = null;
            IB b1 = null;
            IB b2 = null;
            using (var scope = container.Scope)
            {
                a1 = scope.Resolve<IA>();
                a2 = scope.Resolve<IA>();
                b1 = scope.Resolve<IB>();
                b2 = scope.Resolve<IB>();
            }

            Assert.AreNotEqual(a1, a2);
            Assert.AreEqual(b1, b2);

            b1 = container.Resolve<IB>();

            Assert.AreNotEqual(b1, b2);
        }
    }
}
