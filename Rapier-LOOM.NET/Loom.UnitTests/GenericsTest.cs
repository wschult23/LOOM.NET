// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom;
using Loom.JoinPoints; 

namespace Loom.UnitTests.GenericsTest
{
    public class WhereClause : TestBase
    {
        public interface IBaseBase
        {
        }

        public interface IBase:IBaseBase
        {
            void call();
        }

        public class MyType : IBase
        {
            public void call()
            {

            }
        }

        public class MyTypeDerived : MyType
        {
        }

        public class TargetClass : ClassBase
        {
            public virtual IBase myfoo(MyType type)
            {
                testCall = 1;
                return type;
            }
            public virtual IBaseBase mybar(MyTypeDerived type)
            {
                testCall = 1;
                return type;
            }
            public virtual MyType mybaz(MyTypeDerived type)
            {
                testCall = 1;
                return type;
            }
            public virtual IBase myfoobar(IBaseBase type)
            {
                testCall = 1;
                return (IBase)type;
            }

        }

        public interface ITest
        {
            void Remove();
        }


        public class MyClass1
        {
            public virtual void Add(object item)
            {
            }
        }

        public class MyClass2
        {
            public virtual void Add(T item)
            {
            }
        }

        public class MyClass3
        {
            public virtual void Add(string s)
            {
            }

            public virtual int Add(int i)
            {
                return 42;
            }
        }

        public class MyTestAspect : TestAspectBase
        {
            public int counter = 0;

            [Call(Advice.Before)]
            [Include("Add")]
            public void Add<T>([JPTarget] object target, T item)
            {
                counter++;  
            }
        }	

        public class BeforeAspect : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void Test<T>(T t) where T : IBase
            {
                RegisterCall(ref aspectTestBefore, "BeforeAspect.Test()");
            }
        }

        public class AfterAspect : TestAspectBase
        {
            [Call(Advice.After)]
            [IncludeAll]
            public void Test<T>(T t) where T : IBase
            {
                RegisterCall(ref aspectTestAfter, "AfterAspect.Test()");
            }
        }

        public class AroundAspect : TestAspectBase
        {
            [Call(Advice.Around)]
            [IncludeAll]
            public T Test<T>(T t) where T : IBase
            {
                RegisterCall(ref aspectTestInstead, "AroundAspect.Test()");
                return (T)t;
            }
        }

        public class AfterReturningAspect : TestAspectBase
        {
            [Call(Advice.AfterReturning)]
            [IncludeAll]
            public void Test<T>([JPRetVal]T retval, T t) where T : IBase
            {
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.Test()");
            }
        }

        public class VoidMethodAspect : TestAspectBase
        {
            [Call(Advice.Around)]
            [IncludeAll]
            public T Test<T,J>(J t) where T : _Void
            {
                RegisterCall(ref aspectTestInstead, "AroundAspect.Test()");
                return default(T);
            }
        }
    }

    [TestClass]
    public class AdditionalTests : WhereClause
    {
        [TestMethod]
        [TestCategory("Unresolved")]
        public void GenericTest()
        {
            MyTestAspect aspect = new MyTestAspect();

            MyClass1 myclass1 = Weaver.Create<MyClass1>(aspect);
            myclass1.Add(null);
            Assert.AreEqual(1, aspect.counter);

            MyClass2 myclass2 = Weaver.Create<MyClass2>(aspect);
            myclass2.Add(null);
            Assert.AreEqual(2, aspect.counter);

            System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
            list.Add(null);

            System.Collections.IList l = Weaver.Create<System.Collections.Generic.List<object>>(aspect);
            l.Add(null);
            Assert.AreEqual(3, aspect.counter);

        }
    }

    [TestClass]
    public class MyTypeTest : WhereClause
    {
        [TestMethod]
        public void GenericsBeforeAspect()
        {
            BeforeAspect ta = new BeforeAspect();
            callorder = 1;

            TargetClass t = Weaver.Create<TargetClass>(ta);
            t.myfoo(new MyType());

            Assert.AreEqual(t.testCall, ta.aspectTestBefore, "Before Aspect was not called.");
        }

        [TestMethod]
        public void GenericsAfterAspect()
        {
            AfterAspect ta = new AfterAspect();
            callorder = 1;

            TargetClass t = Weaver.Create<TargetClass>(ta);
            t.myfoo(new MyType());

            Assert.AreEqual(t.testCall, ta.aspectTestAfter, "After Aspect was not called.");
        }

        [TestMethod]
        public void GenericsAroundAspect()
        {
            AroundAspect ta = new AroundAspect();
            callorder = 1;

            TargetClass t = Weaver.Create<TargetClass>(ta);
           
            IBaseBase retval=t.myfoo(new MyType());
            Assert.IsInstanceOfType(retval,typeof(MyType));
            Assert.AreEqual(1, ta.aspectTestInstead, "Around Aspect was not called.");

            ta.aspectTestInstead = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.mybar(new MyTypeDerived());
            Assert.IsInstanceOfType(retval,typeof(MyTypeDerived));
            Assert.AreEqual(1, ta.aspectTestInstead, "Around Aspect was not called.");

            ta.aspectTestInstead = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.mybaz(new MyTypeDerived());
            Assert.IsInstanceOfType(retval,typeof(MyTypeDerived));
            Assert.AreEqual(0, ta.aspectTestInstead, "Around Aspect was called.");

            ta.aspectTestInstead = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.myfoobar(new MyType());
            Assert.IsInstanceOfType(retval,typeof(MyType));
            Assert.AreEqual(0, ta.aspectTestInstead, "Around Aspect was called.");
        }


        [TestMethod]
        public void GenericsAfterReturningAspect()
        {
            AfterReturningAspect ta = new AfterReturningAspect();
            callorder = 1;

            TargetClass t = Weaver.Create<TargetClass>(ta);
            IBaseBase retval=t.myfoo(new MyType());
            Assert.IsInstanceOfType(retval,typeof(MyType));
            Assert.AreEqual(1, t.testCall);
            Assert.AreEqual(1, ta.aspectTestAfterReturning, "AfterReturning Aspect was not called.");

            ta.aspectTestAfterReturning = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.mybar(new MyTypeDerived());
            Assert.IsInstanceOfType(retval,typeof(MyTypeDerived));
            Assert.AreEqual(1, t.testCall);
            Assert.AreEqual(1, ta.aspectTestAfterReturning, "AfterReturning Aspect was not called.");

            ta.aspectTestAfterReturning = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.mybaz(new MyTypeDerived());
            Assert.IsInstanceOfType(retval,typeof(MyTypeDerived));
            Assert.AreEqual(1, t.testCall);
            Assert.AreEqual(0, ta.aspectTestAfterReturning, "AfterReturning Aspect was called.");

            ta.aspectTestAfterReturning = 0;
            callorder = 1;
            t.testCall = 0;
            retval = t.myfoobar(new MyType());
            Assert.IsInstanceOfType(retval,typeof(MyType));
            Assert.AreEqual(1, t.testCall);
            Assert.AreEqual(0, ta.aspectTestAfterReturning, "AfterReturning Aspect was called.");
        }

        [TestMethod]
        public void GenericsVoidAspect()
        {
            var ta = new VoidMethodAspect();
            callorder = 1;

            var t = Weaver.Create<MyClass3>(ta);

            t.Add(42);
            Assert.AreEqual(0, ta.aspectTestInstead, "Around Advicewas called.");

            t.Add("Test");
            Assert.AreEqual(1, ta.aspectTestInstead, "AfterReturning Aspect was not called.");
        }

    }
}
