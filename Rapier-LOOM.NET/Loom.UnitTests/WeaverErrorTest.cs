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



namespace Loom.UnitTests.WeaverErrorTest
{
    public class WeaverErrorsTestBase:TestBase
    {
        public class ErrorMessageAspect2 : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void aspect1([JPContext] object Parameter)
            {
            }
        }

        public class ErrorMessageAspect4 : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void aspect1()
            {
            }
        }

        public class ErrorMessageAspect5 : TestAspectBase
        {
            [Error("Test")]
            public void Test() { }
        }

        public class ErrorMessageAspect6 : TestAspectBase
        {
            [Error(TargetTypes.Create, "Test")]
            public void Create() { }
        }

        public class ErrorMessageAspect7 : TestAspectBase
        {
            [Error("Test")]
            public void Test(int i) { }
        }

        public class ErrorMessageAspect8 : TestAspectBase
        {
            [Error(TargetTypes.Create, "Test")]
            public void Create(int i) { }
        }

        [ErrorMessageAspect9]
        public class ErrorTest
        {
            public ErrorTest(int i, int j)
            {
            }
        }

        public class ErrorMessageAspect9 : TestAspectBase
        {
            [Error(TargetTypes.Create, "constructors with parameters are not allowed")]
            [IncludeAll]
            public void Error<T>(T first, params object[] following)
            {
            }
        }


        [TestClass]
        public class ErrorMessageTest_ERR0002 : ErrorMessageAspect2
        {
            [TestMethod]
            [ExpectedException(typeof(AspectWeaverException))]
            public void test1()
            {
                ErrorMessageAspect2 asp = new ErrorMessageAspect2();
                A a = Weaver.Create<A>(asp, null);
            }

        }

        [TestClass]
        public class ErrorMessageTest_ERR0004 : ErrorMessageAspect2
        {
            [TestMethod]
            public void test1()
            {
                ErrorMessageAspect4 asp = new ErrorMessageAspect4();
                A a = Weaver.Create<A>(asp, null);

            }
        }

        
        [TestClass]
        public class ErrorMessageTest_User : ErrorMessageAspect2
        {
            [TestMethod]
            [ExpectedException(typeof(AspectWeaverException))]
            public void test1()
            {
                ErrorMessageAspect5 asp = new ErrorMessageAspect5();
                A a = Weaver.Create<A>(asp, null);
            }

            [TestMethod]
            [ExpectedException(typeof(AspectWeaverException))]
            public void test2()
            {
                ErrorMessageAspect6 asp = new ErrorMessageAspect6();
                A a = Weaver.Create<A>(asp, null);
            }

            [TestMethod]
            public void test3()
            {
                ErrorMessageAspect8 asp = new ErrorMessageAspect8();
                A a = Weaver.Create<A>(asp, null);
            }

            [TestMethod]
            public void test4()
            {
                ErrorMessageAspect7 asp = new ErrorMessageAspect7();
                A a = Weaver.Create<A>(asp, null);
            }
 
            [TestMethod]
            [ExpectedException(typeof(AspectWeaverException))]
            public void test5()
            {
                ErrorTest a = Weaver.Create<ErrorTest>(1,2);
            }
        }


    }
}
