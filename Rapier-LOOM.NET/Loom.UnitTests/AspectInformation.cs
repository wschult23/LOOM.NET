// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Loom;
using Loom.JoinPoints;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Loom.UnitTests.AspectContextTest
{
	/// <summary>Contains classes and aspects for testing interweaving.</summary>
	public class AspectContextTestBase:TestBase
	{
		public class IntContext:TestAspectBase
		{
			[Call(Advice.Before)]
			[Include("Test")]
			public void TestBefore([JPContext] Context Context, int i)
			{
				Assert.IsNotNull(Context,"context is null");
				Context.Tag=i;
				RegisterCall(ref aspectTestBefore,"IntContext.Test(int)");
			}

			[Call(Advice.After)]
			[Include("Test")]
            public void TestAfter([JPContext] Context Context, int i)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestAfter,"IntContext.Test(int)");
				Assert.AreEqual(i,(int)Context.Tag,"invalid Tag value");
			}
		}
        

		public class BeforeContextRequiredAspect:TestAspectBase
		{
			[Call(Advice.Before)][Include("Test")]
			public void f([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestBefore, "BeforeContextRequireAspect.f()");
			}
		}
		public class InsteadContextRequiredAspect:TestAspectBase
		{
			[Call(Advice.Around)][Include("Test")]
			public T f<T>([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestInstead, "InsteadContextRequiredAspect.f()");
                object o = Context.Call();
				return (T)o;
			}
		}
		public class AfterContextRequiredAspect:TestAspectBase
		{
			[Call(Advice.After)][Include("Test")]
			public void f([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestAfter, "AfterContextRequiredAspect.f()");
			}
		}
		public class AfterReturningContextRequiredAspect:TestAspectBase
		{
			[Call(Advice.AfterReturning)][Include("Test")]
			public void f([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestAfterReturning, "AfterReturningContextRequiredAspect.f()");
			}
		}
		public class AfterThrowingContextRequiredAspect:TestAspectBase
		{
			[Call(Advice.AfterThrowing)][Include("Test")]
			public void f([JPContext] Context Context, Exception e)
			{
				Assert.IsNotNull(Context,"context is null");
				RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingContextRequiredAspect.f()");
			}
		}

        
		public class CheckMethodNameAspect:TestAspectBase
		{
			[Call(Advice.Around)][Include("Test")]
			public T Test<T>([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"AspectContect is null");
    			Assert.IsTrue(Context.CurrentMethod.Name.EndsWith("Test"),"method name of AspectContext is false");
				return default(T);
			}			
		}
		public class CheckDeclaringTypeAspect:TestAspectBase
		{
			[Call(Advice.Around)][Include("Test")]
			public T Test<T>([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"AspectContext is null");
				Assert.AreEqual(typeof(TestBase.A),Context.TargetClass,"DeclaringType of AspectContext is false");
				RegisterCall(ref aspectTestInstead,"CheckDeclaringTypeAspect.Test()");
				Object o = Context.Call();
				return (T)o;
			}
		}
		public class CheckInstanceAspect:TestAspectBase
		{
			[Call(Advice.Around)][Include("Test")]
			public T Test<T>([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"AspectContext is null");
				Assert.IsNotNull(Context.Instance,"instance of AspectContext is null");
				RegisterCall(ref aspectTestInstead,"CheckInstanceAspect.Test()");
				object o = Context.Call();
				return (T)o;
			}
		}

		public class CheckMethodName:TestAspectBase
		{
			[Call(Advice.Around)][Include("TestPub")]
			public T Test<T>([JPContext] Context Context)
			{
				Assert.IsNotNull(Context,"AspectContext is null");
				Assert.AreEqual("TestPub",Context.CurrentMethod.Name,"method name is false");
				return default(T);
			}
		}

		public interface IfTest
		{
			void Test();
		}
		public class ClassPub:ClassBase,IfTest
		{
			public void Test()
			{
				RegisterCall(ref testCall,"ClassPub.TestPub()");
				if(bThrowException) throw new TestException();
			}
		}
		
		public class ClassPriv:ClassBase,IfTest
		{
			void IfTest.Test()
			{
				RegisterCall(ref testCall,"ClassPriv.TestPriv()");
				if(bThrowException) throw new TestException();
			}
		}

		public class ClassVirtual:ClassBase,IfTest
		{
			public virtual void Test()
			{
				RegisterCall(ref testCall,"ClassPub.TestPub()");
				if(bThrowException) throw new TestException();
			}
		}
	}
	[TestClass]
	public class InvokeInstead:AspectContextTestBase
	{
		[TestMethod]
		public void Int()
		{
			callorder=1;
			IntContext asp=new IntContext();
			B b=Weaver.CreateInstance(typeof(B),null,asp) as B;
			Assert.IsNotNull(b,"no object created");
			b.Test(23);
			Assert.AreEqual(1,asp.aspectTestBefore,"Aspect not called");
			Assert.AreEqual(2,b.testCall,"method not called");
			Assert.AreEqual(3,asp.aspectTestAfter,"Aspect not called");
		}
	}
	
    /*
    /// <summary>Tests for ContextRequired Attribute</summary>
	[TestClass]
	public class ContextRequired:AspectContextTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
        [TestMethod]
		public void Before()
		{
			BeforeContextRequiredAspect a = new BeforeContextRequiredAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not called");
		}
		[TestMethod]
		public void Instead()
		{
			InsteadContextRequiredAspect a = new InsteadContextRequiredAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not called");
		}
		[TestMethod]
		public void After()
		{
			AfterContextRequiredAspect a = new AfterContextRequiredAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,o.testCall,"object not called");
			Assert.AreEqual(2,a.aspectTestAfter,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterReturning()
		{
			AfterReturningContextRequiredAspect a = new AfterReturningContextRequiredAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,o.testCall,"object not called");
			Assert.AreEqual(2,a.aspectTestAfterReturning,"Aspect not jpinterwoven");
		}
		[TestMethod]
		public void AfterThrowing()
		{
			AfterThrowingContextRequiredAspect a = new AfterThrowingContextRequiredAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.bThrowException = true;
			o.Test();
			Assert.AreEqual(1,o.testCall,"object not called");
			Assert.AreEqual(2,a.aspectTestAfterThrowing,"Aspect not jpinterwoven");
		}
	}*/
     
      
	/// <summary>Tests the properties of the AspectContext class.</summary>
	[TestClass]
	public class CheckAspectContextInformation:AspectContextTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void MethodNameVirtual()
		{
			CheckMethodNameAspect a = new CheckMethodNameAspect();
			ClassVirtual o = (ClassVirtual)Weaver.CreateInstance(typeof(ClassVirtual),null,a);
			o.Test();
		}
		[TestMethod]
		public void MethodNamePublic()
		{
			CheckMethodNameAspect a = new CheckMethodNameAspect();
			IfTest o = (IfTest)Weaver.CreateInstance(typeof(ClassPub),null,a);
			o.Test();
		}
		[TestMethod]
		public void MethodNamePrivate()
		{
			CheckMethodNameAspect a = new CheckMethodNameAspect();
			IfTest o = (IfTest)Weaver.CreateInstance(typeof(ClassPriv),null,a);
			o.Test();
		}
		[TestMethod]
		public void DeclaringType()
		{
			CheckDeclaringTypeAspect a = new CheckDeclaringTypeAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not jpinterwoven");
		}
		[TestMethod]
		public void Instance()
		{
			CheckInstanceAspect a = new CheckInstanceAspect();
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not jpinterwoven");
		}
	}
}
