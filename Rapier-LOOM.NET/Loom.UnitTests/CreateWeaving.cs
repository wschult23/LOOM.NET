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

namespace Loom.UnitTests.CreateWeaving
{
	/// <summary>
	/// CreateWeavingTestBase contains all aspects for testing interweaving with create.
	/// </summary>
	public class CreateWeavingTestBase:TestBase
	{
		/// <summary>
		/// AspectAttribute for interweaving an empty constructor with Advice.Before.
		/// </summary>
		public class BeforeCreateAspect:TestAspectBase
		{
			[Create(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeCreateAspect.Before");
			}
		}
		/// <summary>
		/// AspectAttribute for interwaaving a constructor with Advice.Before and wildcard.
		/// </summary>
		public class BeforeCreateAspect2:TestAspectBase
		{
			[Create(Advice.Before)]
            [IncludeAll]
			public void Before2(params object[] args)
			{
				RegisterCall(ref aspectTestBefore, "BeforeCreateAspect.Before2");
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving an empty constructor with Advice.After.
		/// </summary>
		public class AfterCreateAspect:TestAspectBase
		{
			[Create(Advice.After)]
			public void After()
			{
				RegisterCall(ref aspectTestAfter, "AfterCreateAspect.After");
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a constructor with Advice.After and wildcard.
		/// </summary>
		public class AfterCreateAspect2:TestAspectBase
		{
			[Create(Advice.After)]
            [IncludeAll]
			public void After2(params object[] args)
			{
				RegisterCall(ref aspectTestAfter, "AfterCreateAspect.After2");
			}
		}
		/// <summary>
		/// AspectAttribute for intereweaving an empty constructor with Advice.Instead.
		/// </summary>
		public class SimpleInsteadCreateAspect:TestAspectBase
		{
			[Create(Advice.Around)]
            public T Instead<T>([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestInstead, "SimpleInsteadCreateAspect.Instead");
				object o=Context.Call();
				return (T)o;
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a constructor with Advice.Instead and wildcard.
		/// </summary>
		public class SimpleInsteadCreateAspect2:TestAspectBase
		{
			[Create(Advice.Around)]
            [IncludeAll]
            public T Instead2<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead, "SimpleInsteadCreateAspect.Instead2");
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a constructor with Advice.Instead and wildcard.
		/// </summary>
		public class InsteadCreateAspect:TestAspectBase
		{
			///Geündert W.S. siehe unten...
			//			[Loom.Create(Advice.Instead)]
			//			public object Instead1(System.Type param)
			//			{
			//				RegisterCall(ref aspectTestInstead, "InsteadCreateAspect.Instead1");
			//				object o=Context.Invoke();
			//				return o;
			//			}
			[Create(Advice.Around)]
            [IncludeAll]
            public T Instead2<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead, "InsteadCreateAspect.Instead2");
				object o=Context.Invoke(args);
				return (T)o;
			}
			/// Geündert W.S.: Diese Methode kollidiert mit der vorhergehenden:
			/// der Weber weiss nicht, welche der beiden Methoden er anstelle der 
			/// originalen Methode einweben soll.
			//			[Loom.Create(Advice.Instead)]
			//			public object Instead3(System.Type param, object[] args)
			//			{
			//				RegisterCall(ref aspectTestInstead, "InsteadCreateAspect.Instead3");
			//				object o=Context.Invoke(args);
			//				return o;
			//			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a constructor with Advice.Instead and wildcard.
		/// </summary>
		public class InsteadCreateAspect2:TestAspectBase
		{
			[Create(Advice.Around)]
            [IncludeAll]
            public T InsteadCresteAspect2<T>([JPContext] Context Context, params  object[] args)
			{
				RegisterCall(ref aspectTestInstead, "InsteadCreateAspect.InsteadCresteAspect2");
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving an empty constructor with Advice.Instead.
		/// </summary>
		public class CreateInsteadSameParams:TestAspectBase
		{
			[Create(Advice.Around)]
            public T Create<T>([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestInstead,"CreateInsteadSameParams.Create");
				return (T)Context.Call();
			}
		}
		public class InsteadAspect:TestAspectBase
		{
			[Create(Advice.Around)]
            [IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspect.f");
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		public class BeforeAspectForRefInt:TestAspectBase
		{
			[Create(Advice.Around)]
			public T f<T>([JPContext] Context ctx, ref int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForRefObject.f");
                object[] param=new object[1];
                param[0]=i;
                object res=ctx.Invoke(param);
                i = (int)param[0];
                return (T)res;
			}
		}
		public class BeforeAspectForOutInt:TestAspectBase
		{
			[Create(Advice.Around)]
			public T f<T>([JPContext] Context ctx, out int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutObject.f");
                object[] param = new object[1];
                param[0] = default(int);
                object res = ctx.Invoke(param);
                i = (int)param[0];
                return (T)res;
            }
		}
		[BeforeAspect]
		public class TestClass1:ClassBase
		{
			public TestClass1()
			{
				RegisterCall(ref testCall,"TestClass.TestClass()");
			}
		}
		[BeforeAspectWithWildcard]
		public class TestClass2:ClassBase
		{
			public TestClass2(int i)
			{
				RegisterCall(ref testCall,"TestClass.TestClass(int i)");
			}
			public TestClass2(string s)
			{
				RegisterCall(ref testCall,"TestClass.TestClass(string s)");
			}
		}
		[AttributeUsage(AttributeTargets.Class)]
		public class BeforeAspect:TestAspectBase
		{
			[Create(Advice.Before)]
			public void Before()
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspect.Before");
			}
		}
		[AttributeUsage(AttributeTargets.Class)]
		public class BeforeAspectWithWildcard:TestAspectBase
		{
            public BeforeAspectWithWildcard()
            {
            }

			[Create(Advice.Before)]
            [IncludeAll]
			public void Before(params object[] args)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectWithWildcard.Before");
			}
		}
	}

    
    /// <summary>
	/// InvokeBefore contains all tests for interweaving with create and Advice.Before.
	/// </summary>
	[TestClass]
	public class InvokeBefore:CreateWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>Test with empty constructor.</summary>
		[TestMethod]
		public void Before()
		{
			BeforeCreateAspect bca=new BeforeCreateAspect();
			J j=(J)Weaver.CreateInstance(typeof(J),null,bca);
			Assert.AreEqual(1,bca.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with empty constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void BeforeEmptyConstructor()
		{
			BeforeCreateAspect2 bca2=new BeforeCreateAspect2();
			J j=(J)Weaver.CreateInstance(typeof(J),null,bca2);
			Assert.AreEqual("empty",j.o.ToString(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,bca2.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with integer constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void BeforeIntegerConstructor()
		{
			BeforeCreateAspect2 bca2=new BeforeCreateAspect2();
			object[] o=new object[1];
			int i=1;
			o[0]=i;
			J j=(J)Weaver.CreateInstance(typeof(J),o,bca2);
			Assert.AreEqual(typeof(int),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,bca2.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with string constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void BeforeStringConstructor()
		{
			BeforeCreateAspect2 bca2=new BeforeCreateAspect2();
			object[] o=new object[1];
			string s="a";
			o[0]=s;
			J j=(J)Weaver.CreateInstance(typeof(J),o,bca2);
			Assert.AreEqual(typeof(string),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,bca2.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with empty constructors and 2 aspects.</summary>
		[TestMethod]
		public void Interweave2AspectBefore()
		{
			BeforeCreateAspect bca=new BeforeCreateAspect();
			BeforeCreateAspect bca2=new BeforeCreateAspect();
			Loom.AspectAttribute[] args=new AspectAttribute[2];
			args[0]=bca;
			args[1]=bca2;
			J j=(J)Weaver.CreateInstance(typeof(J),null,args);
			Assert.AreEqual(1,bca2.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,bca.aspectTestBefore,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(3,j.testCall,"object not created");
		}

        [TestMethod]
        public void BeforeConstructor2()
        {
            BeforeCreateAspect2 bca = new BeforeCreateAspect2();
            J2 j1 = Weaver.Create<J2>(bca, 1);
            Assert.AreEqual(1,bca.aspectTestBefore, "Aspect not jpinterwoven at object creation");
            Assert.AreEqual(j1.o, 1);
        }
        [TestMethod]
        public void BeforeConstructor3()
        {
            BeforeCreateAspect2 bca2 = new BeforeCreateAspect2();

            J2 j2 = Weaver.Create<J2>(bca2, _T<J2>.Null);
            Assert.AreEqual(1, bca2.aspectTestBefore, "Aspect not jpinterwoven at object creation");

            Assert.IsNull(j2.o);
        }

        [TestMethod]
        public void BeforeConstructor4()
        {
            BeforeCreateAspect2 bca2 = new BeforeCreateAspect2();

            J2 j2 = Weaver.Create<J2>(bca2, _T<object>.Null);
            Assert.AreEqual(1, bca2.aspectTestBefore, "Aspect not jpinterwoven at object creation");

            Assert.IsNull(j2.o);
        }

        [TestMethod]
        public void BeforeConstructor5()
        {
            BeforeCreateAspect2 bca2 = new BeforeCreateAspect2();

            J2 j2 = Weaver.Create<J2>(bca2, _T<B>.Null);
            Assert.AreEqual(1, bca2.aspectTestBefore, "Aspect not jpinterwoven at object creation");

            Assert.IsNull(j2.o);
        }

        [TestMethod]
        public void BeforeConstructor6()
        {
            BeforeCreateAspect2 bca2 = new BeforeCreateAspect2();

            A a = new A();

            J2 j2 = Weaver.Create<J2>(bca2, _T<object>.Value(a));
            Assert.AreEqual(1, bca2.aspectTestBefore, "Aspect not jpinterwoven at object creation");

            Assert.AreEqual(a, j2.o);
        }



	}	
	/// <summary>
	/// InvokeAfter contains all tests for interweaving with create and Advice.After.
	/// </summary>
	[TestClass]
	public class InvokeAfter:CreateWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>Test with empty constructor.</summary>
		[TestMethod]
		public void After()
		{
			AfterCreateAspect aca=new AfterCreateAspect();
			J j=(J)Weaver.CreateInstance(typeof(J),null,aca);
			Assert.AreEqual(1,j.testCall,"object not created");
			Assert.AreEqual(2,aca.aspectTestAfter,"Aspect not jpinterwoven at object creation");
		}
		/// <summary>Test with empty constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void AfterEmptyConstructor()
		{
			AfterCreateAspect2 aca2=new AfterCreateAspect2();
			J j=(J)Weaver.CreateInstance(typeof(J),null,aca2);
			Assert.AreEqual("empty",j.o.ToString(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,j.testCall,"object not created");
			Assert.AreEqual(2,aca2.aspectTestAfter,"Aspect not jpinterwoven at object creation");
		}
		/// <summary>Test with integer constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void AfterIntegerConstructor()
		{
			AfterCreateAspect2 aca2=new AfterCreateAspect2();
			object[] o=new object[1];
			int i=1;
			o[0]=i;
			J j=(J)Weaver.CreateInstance(typeof(J),o,aca2);
			Assert.AreEqual(typeof(int),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,j.testCall,"object not created");
			Assert.AreEqual(2,aca2.aspectTestAfter,"Aspect not jpinterwoven at object creation");
		}
		/// <summary>Test with string constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void AfterStringConstructor()
		{
			AfterCreateAspect2 aca2=new AfterCreateAspect2();
			object[] o=new object[1];
			string s="a";
			o[0]=s;
			J j=(J)Weaver.CreateInstance(typeof(J),o,aca2);
			Assert.AreEqual(typeof(string),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,j.testCall,"object not created");
			Assert.AreEqual(2,aca2.aspectTestAfter,"Aspect not jpinterwoven at object creation");
		}
		/// <summary>Test with empty constructor and 2 aspects.</summary>
		[TestMethod]
		public void Interweave2AspectsAfter()
		{
			AfterCreateAspect aca=new AfterCreateAspect();
			AfterCreateAspect aca2=new AfterCreateAspect();
			Loom.AspectAttribute[] args=new AspectAttribute[2];
			args[0]=aca;
			args[1]=aca2;
			J j=(J)Weaver.CreateInstance(typeof(J),null,args);
			Assert.AreEqual(1,j.testCall,"object not created");			
			Assert.AreEqual(2,aca.aspectTestAfter,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(3,aca2.aspectTestAfter,"Aspect not jpinterwoven at object creation");
		}
	}
	/// <summary>
	/// InvokeInstead contains all tests for interweaving with create and Advice.Instead.
	/// </summary>
	[TestClass]
	public class InvokeInstead:CreateWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>Test with empty constructor.</summary>
		[TestMethod]
		public void SimpleInstead()
		{
			SimpleInsteadCreateAspect sica=new SimpleInsteadCreateAspect();
			J j=(J)Weaver.CreateInstance(typeof(J),null,sica);
			Assert.AreEqual(1,sica.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with empty constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void SimpleInsteadEmptyConstructor()
		{
			SimpleInsteadCreateAspect2 sica2=new SimpleInsteadCreateAspect2();
			J j=(J)Weaver.CreateInstance(typeof(J),null,sica2);
			Assert.AreEqual("empty",j.o.ToString(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,sica2.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with integer constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void SimpleInsteadIntegerConstructor()
		{
			SimpleInsteadCreateAspect2 sica2=new SimpleInsteadCreateAspect2();
			object[] o=new object[1];
			int i=1;
			o[0]=i;
			J j=(J)Weaver.CreateInstance(typeof(J),o,sica2);
			Assert.AreEqual(typeof(int),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,sica2.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with string constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void SimpleInsteadStringConstructor()
		{
			SimpleInsteadCreateAspect2 sica2=new SimpleInsteadCreateAspect2();
			object[] o=new object[1];
			string s="a";
			o[0]=s;
			J j=(J)Weaver.CreateInstance(typeof(J),o,sica2);
			Assert.AreEqual(typeof(string),j.o.GetType(),"Aspect not jpinterwoven with proper constructor");
			Assert.AreEqual(1,sica2.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with empty constructor.</summary>
		[TestMethod]
		public void Instead1()
		{
			InsteadCreateAspect ica=new InsteadCreateAspect();
			J j=(J)Weaver.CreateInstance(typeof(J),null,ica);
			Assert.AreEqual(1,ica.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with integer constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void Instead2()
		{
			InsteadCreateAspect ica=new InsteadCreateAspect();
			object[] args=new object[1];
			args[0]=1;
			J j=(J)Weaver.CreateInstance(typeof(J),args,ica);
			Assert.AreEqual(1,ica.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}
		/// <summary>Test with string constructor and Aspect with wildcard.</summary>
		[TestMethod]
		public void Instead3()
		{
			InsteadCreateAspect ica=new InsteadCreateAspect();
			object[] args=new object[1];
			args[0]="a";
			J j=(J)Weaver.CreateInstance(typeof(J),args,ica);
			Assert.AreEqual(1,ica.aspectTestInstead,"Aspect not jpinterwoven at object creation");
			Assert.AreEqual(2,j.testCall,"object not created");
		}

		/// <summary>Test with empty constructor and 2 aspects.</summary>
		[TestMethod]
		public void Interweave2AspectsInstead()
		{
			InsteadCreateAspect a=new InsteadCreateAspect();
			InsteadCreateAspect a2=new InsteadCreateAspect();
			Loom.AspectAttribute[] args=new AspectAttribute[2];
			args[0]=a;
			args[1]=a2;
			J j=(J)Weaver.CreateInstance(typeof(J),null,args);
			Assert.AreEqual(1,a2.aspectTestInstead,"aspects not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,j.testCall,"object not created");
		}
		/// <summary>Test with empty constructor.</summary>
		[TestMethod]
		public void SameParameter()
		{
			CreateInsteadSameParams asp=new CreateInsteadSameParams();
			A a=Weaver.CreateInstance(typeof(A),null,asp) as A;
			Assert.IsNotNull(a,"no object created");
			Assert.AreEqual(1,asp.aspectTestInstead,"Aspect not called");
		}
	}
	[TestClass]
	public class RefAndOutParameterWeaving:CreateWeavingTestBase
	{
		[TestMethod]
		public void InsteadRefInt()
		{
			callorder=1;
			InsteadAspect a=new InsteadAspect();
			object[] o=new object[1];
			int i=0;
			o[0]=i;
            R r = Weaver.Create<R>(a, o);
			i=(int)o[0];
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,r.testCall,"object not called");
			Assert.AreEqual(1,i,"ref parameter not changed");
		}
		[TestMethod]
		public void InsteadOutInt()
		{
			callorder=1;
			InsteadAspect a=new InsteadAspect();
			object[] o=new object[1];
			int i=0;
			o[0]=i;
            Q q = Weaver.Create<Q>(a, o);
			i=(int)o[0];
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,q.testCall,"object not called");
			Assert.AreEqual(1,i,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeRefInt()
		{
			callorder=1;
			BeforeAspectForRefInt a=new BeforeAspectForRefInt();
			object[] o=new object[1];
			int i=0;
			o[0]=i;
            R r = Weaver.Create<R>(a, o);
            i=(int)o[0];
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,r.testCall,"object not called");
			Assert.AreEqual(1,i,"ref parameter not changed");
		}
		[TestMethod]
		public void BeforeOutInt()
		{
			try
			{
				callorder=1;
				BeforeAspectForOutInt a=new BeforeAspectForOutInt();
				object[] o=new object[1];
				int i=0;
				o[0]=i;
                Q q = Weaver.Create<Q>(a, o);
				i=(int)o[0];
				Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
				Assert.AreEqual(2,q.testCall,"object not called");
				Assert.AreEqual(1,i,"out parameter not changed");
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Fehler: "+e.Message.ToString());
			}
		}
	}
	[TestClass]
	public class StaticWeaving:CreateWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void BeforeEmptyConstructor()
		{
			TestClass1 o = (TestClass1)Weaver.CreateInstance(typeof(TestClass1));
			BeforeAspect a = (BeforeAspect)Weaver.GetAspects(o)[0];
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not jpinterwoven");
		}
		[TestMethod]
		public void BeforeIntegerConstructor()
		{
			object[] args = new object[1];
			int i = 1;
			args[0] = i;
			TestClass2 o = (TestClass2)Weaver.CreateInstance(typeof(TestClass2),args);
			BeforeAspectWithWildcard a = (BeforeAspectWithWildcard)Weaver.GetAspects(o)[0];
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not jpinterwoven");
		}
		[TestMethod]
		public void BeforeStringConstructor()
		{
			object[] args = new object[1];
			string s = "a";
			args[0] = s;
			TestClass2 o = (TestClass2)Weaver.CreateInstance(typeof(TestClass2),args);
			BeforeAspectWithWildcard a = (BeforeAspectWithWildcard)Weaver.GetAspects(o)[0];
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o.testCall,"object not jpinterwoven");
		}
	}
}
