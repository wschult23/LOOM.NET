// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;

namespace Loom.UnitTests.WildcardWeaving
{
	/// <summary>
	/// WildcardWeavingTestBase contains all aspects for testing wildcard weaving.
	/// </summary>
	public class WildcardWeavingTestBase:TestBase
	{
		public class BeforeAspect:TestAspectBase
		{
			private object expected;
			private bool check;
			public BeforeAspect(object expected)
			{
				this.expected=expected;
				this.check=true;
			}
			public BeforeAspect()
			{
				this.check=false;
			}

			[Call(Advice.Before)]
            [IncludeAll]
			public void Test(params object[] param)
			{
				Assert.IsNotNull(param,"invalid param");
				if(check)
				{
					if(param.Length==1)
					{
						Assert.AreEqual(expected, param[0],"Invalid parameter value.");
					}
					else
					{
						Assert.IsNull(expected,"invalid param firstparam");
					}
				}
				RegisterCall(ref aspectTestBefore,"BeforeAspect.Test()");
			}
		}

		public class AfterAspect:TestAspectBase
		{
			private object expected;
			public AfterAspect(object expected)
			{
				this.expected=expected;
			}

			[Call(Advice.After)] [IncludeAll]
			public void Test(params object[] param)
			{
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestAfter,"AfterAspect.Test()");
			}
		}

		public class AfterThrowingAspect:TestAspectBase
		{
			private object expected;
			public AfterThrowingAspect(object expected)
			{
				this.expected=expected;
			}

			[Call(Advice.AfterThrowing)] [IncludeAll]
			public T Test<T>([JPException] Exception e, params object[] param)
			{
				Assert.IsNotNull(e as TestException,"invalid exception");
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return default(T);
			}
		}

		public class AfterReturningAspect:TestAspectBase
		{
			private object expected;
			public AfterReturningAspect(object expected)
			{
				this.expected=expected;
			}

			[Call(Advice.AfterReturning)] [IncludeAll]
			public void Test<T,R>([JPRetVal] T o, R param)
			{
				Assert.IsNotNull(param,"invalid param");
				Assert.AreEqual(expected, param,"Invalid parameter value.");
    			Assert.AreEqual(param, o,"Invalid parameter value.");
				RegisterCall(ref aspectTestAfterReturning,"AfterReturningAspect.Test()");
			}

            [Call(Advice.AfterReturning)]
            [IncludeAll]
            public void Test3<T,R>([JPRetVal] T o, ref R param)
            {
                Assert.IsNotNull(param, "invalid param");
                Assert.AreEqual(expected, param, "Invalid parameter value.");
                Assert.AreEqual(param, o, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.Test()");
            }

            [Call(Advice.AfterReturning)]
            [IncludeAll]
            public void Test2()
            {
                Assert.IsNull(expected, "invalid param firstparam");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.Test()");
            }
		}

		public class MultiMatchAspect:TestAspectBase
		{
			private object expected;
			public MultiMatchAspect(object expected)
			{
				this.expected=expected;
			}

			[Call(Advice.Before)] [IncludeAll]
			public void TestBefore(params object[] param)
			{
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestBefore,"MultiMatchAspectInt.TestBefore()");
			}
			[Call(Advice.AfterReturning)] [IncludeAll]
			public void TestReturning<T>([JPRetVal] T retval, params object[] param)
			{
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
					Assert.AreEqual(param[0], retval,"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestAfterReturning,"MultiMatchAspectInt.TestReturning()");
			}
			[Call(Advice.AfterThrowing)] [IncludeAll]
			public T TestBAfterThrowing<T>([JPException] Exception e, params object[] param)
			{
				Assert.IsNotNull(e as TestException,"invalid exception");
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestAfterThrowing,"MultiMatchAspectInt.TestAfterThrowing()");
				return default(T);
			}
			[Call(Advice.After)] [IncludeAll]
			public void TestAfter(params object[] param)
			{
				Assert.IsNotNull(param,"invalid param");
				if(param.Length==1)
				{
					Assert.AreEqual(expected, param[0],"Invalid parameter value.");
				}
				else
				{
					Assert.IsNull(expected,"invalid param firstparam");
				}
				RegisterCall(ref aspectTestAfter,"MultiMatchAspectInt.TestAfter()");
			}
		}
	}
		
	[TestClass]
	public class Before:WildcardWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect(null);
			//A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
            A a = Weaver.Create<A>(ta);
			a.Test();
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence A");
			Assert.AreEqual(2,a.testCall,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,b.testCall,"invalid calling sequence B");
		}
		[TestMethod]
		public void Class()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new BeforeAspect(carg);
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			Assert.AreEqual(carg,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence C");
			Assert.AreEqual(2,c.testCall,"invalid calling sequence C");
		}
		[TestMethod]
		public void Reference()
		{
			callorder=1;
			TestAspectBase ta;
			D darg=new D();
			ta=new BeforeAspect(darg);
			D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
			Assert.AreEqual(darg,d.Test(ref darg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence D");
			Assert.AreEqual(2,d.testCall,"invalid calling sequence D");
		}
		[TestMethod]
		public void ValueType()
		{
			callorder=1;
			TestAspectBase ta;
			V v=new V();
			v.InitStruct();
			ta=new BeforeAspect(v);
			E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence E");
			Assert.AreEqual(2,e.testCall,"invalid calling sequence E");
		}
		[TestMethod]
		public void PrimitiveRef()
		{
			TestAspectBase ta;
			callorder=1;
			double d=2.0;
			ta=new BeforeAspect(d);
			F f=(F)Weaver.CreateInstance(typeof(F),null,ta);
			Assert.AreEqual(d,f.Test(ref d),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence H");
			Assert.AreEqual(2,f.testCall,"invalid calling sequence H");
		}
		[TestMethod]
		public void ValueTypeRef()
		{
			callorder=1;
			TestAspectBase ta;
			V v=new V();
			v.InitStruct();
			ta=new BeforeAspect(v);
			G g=(G)Weaver.CreateInstance(typeof(G),null,ta);
			V vcmp=v;
			Assert.AreEqual(vcmp,g.Test(ref v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			Assert.AreEqual(2,g.testCall,"invalid calling sequence G");
		}
		[TestMethod]
		public void Enum()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect(Numbers.One);
			H h=(H)Weaver.CreateInstance(typeof(H),null,ta);
			Assert.AreEqual(Numbers.One,h.Test(Numbers.One),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence H");
			Assert.AreEqual(2,h.testCall,"invalid calling sequence H");
		}
		[TestMethod]
		public void AllInOneClass()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect();
			AllInOne aio=(AllInOne)Weaver.CreateInstance(typeof(AllInOne),null,ta);
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			aio.Test();
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence A");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence A");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			Assert.AreEqual(23,aio.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence B");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			C carg=new C();
			Assert.AreEqual(carg,aio.Test(carg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence C");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence C");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			D darg=new D();
			Assert.AreEqual(darg,aio.Test(ref darg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence D");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence D");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			V v=new V();
			v.InitStruct();
			Assert.AreEqual(v,aio.Test(v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence E");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence E");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			Assert.AreEqual(v,aio.Test(ref v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence H");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence H");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			double d=2.0;
			Assert.AreEqual(d,aio.Test(ref d),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence H");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence H");
			callorder=1;
			aio.testCall=0;
			ta.aspectTestBefore=0;
			Assert.AreEqual(Numbers.One,aio.Test(Numbers.One),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence H");
			Assert.AreEqual(2,aio.testCall,"invalid calling sequence H");
		}
	}

	[TestClass]
	public class After:WildcardWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			callorder=1;
			TestAspectBase ta=new AfterAspect(null);
			A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
			a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(1,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence B");
		}
		[TestMethod]
		public void Class()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new AfterAspect(carg);;
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			Assert.AreEqual(carg,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence C");
		}
		[TestMethod]
		public void Reference()
		{
			TestAspectBase ta;
			callorder=1;
			D darg=new D();
			ta=new AfterAspect(darg);
			D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
			Assert.AreEqual(darg,d.Test(ref darg),"invalid return value");
			Assert.AreEqual(1,d.testCall,"invalid calling sequence D");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence D");
		}
		[TestMethod]
		public void ValueType()
		{
			TestAspectBase ta;
			callorder=1;
			V v=new V();
			v.InitStruct();
			ta=new AfterAspect(v);
			E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence E");
		}
	}

	[TestClass]
	public class AfterReturning:WildcardWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterReturningAspect(null);
            A a = Weaver.Create<A>(ta);
            a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterReturningAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(1,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence B");
		}

		[TestMethod]
		public void Class()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new AfterReturningAspect(carg);
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			Assert.AreEqual(carg,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence C");
		}
		[TestMethod]
		public void Reference()
		{
			TestAspectBase ta;
			callorder=1;
			D darg=new D();
			ta=new AfterReturningAspect(darg);
			D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
			Assert.AreEqual(darg,d.Test(ref darg),"invalid return value");
			Assert.AreEqual(1,d.testCall,"invalid calling sequence D");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence D");
		}
		[TestMethod]
		public void ValueType()
		{
			TestAspectBase ta;
			callorder=1;
			V v=new V();
			v.InitStruct();
			ta=new AfterReturningAspect(v);
			E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence E");
		}		
	}

	[TestClass]
	public class AfterThrowing:WildcardWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterThrowingAspect(null);
			A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
			a.bThrowException=true;
			a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterThrowingAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			b.bThrowException=true;
			Assert.AreEqual(0,b.Test(23),"invalid return value");
			Assert.AreEqual(1,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence B");
		}
		[TestMethod]
		public void Class()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new AfterThrowingAspect(carg);
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			c.bThrowException=true;
			Assert.AreEqual(null,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence C");
		}
		[TestMethod]
		public void Reference()
		{
			TestAspectBase ta;
			callorder=1;
			D darg=new D();
			ta=new AfterThrowingAspect(darg);
			D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
			d.bThrowException=true;
			d.Test(ref darg);
			Assert.AreEqual(1,d.testCall,"invalid calling sequence D");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence D");
		}
		[TestMethod]
		public void ValueType()
		{
			TestAspectBase ta;
			callorder=1;
			V v=new V();
			v.InitStruct();
			ta=new AfterThrowingAspect(v);
			E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
			e.bThrowException=true;
			e.Test(v);
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence E");
		}
	}
	
    [TestClass]
	public class MultiMatchDifferentInvokeOrders:WildcardWeavingTestBase
	{
		[TestMethod]
		public void CallAndReturnInt()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"invalid calling sequence B");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"invalid calling sequence B");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence B");
		}
		[TestMethod]
		public void CallAndThrowInt()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspect(23);
			B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
			b.bThrowException=true;
			Assert.AreEqual(0,b.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(0,ta.aspectTestAfterReturning,"invalid calling sequence B");
			Assert.AreEqual(3,ta.aspectTestAfterThrowing,"invalid calling sequence B");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence B");
		}
		[TestMethod]
		public void CallAndReturnClass()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new MultiMatchAspect(carg);
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			Assert.AreEqual(carg,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence C");
			Assert.AreEqual(2,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"invalid calling sequence C");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"invalid calling sequence C");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence C");
		}
		[TestMethod]
		public void CallAndThrowClass()
		{
			TestAspectBase ta;
			callorder=1;
			C carg=new C();
			ta=new MultiMatchAspect(carg);
			C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
			c.bThrowException=true;
			Assert.AreEqual(null,c.Test(carg),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence C");
			Assert.AreEqual(2,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(0,ta.aspectTestAfterReturning,"invalid calling sequence C");
			Assert.AreEqual(3,ta.aspectTestAfterThrowing,"invalid calling sequence C");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence C");
		}
		[TestMethod]
		public void CallAndReturnValue()
		{
			TestAspectBase ta;
			callorder=1;
			V v=new V();
			ta=new MultiMatchAspect(v);
			E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence E");
			Assert.AreEqual(2,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"invalid calling sequence E");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"invalid calling sequence E");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence E");
		}
		[TestMethod]
		public void CallAndReturnValueRef()
		{
			TestAspectBase ta;
			callorder=1;
			V v=new V();
			ta=new MultiMatchAspect(v);
			G g=(G)Weaver.CreateInstance(typeof(G),null,ta);
			Assert.AreEqual(v,g.Test(ref v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			Assert.AreEqual(2,g.testCall,"invalid calling sequence G");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"invalid calling sequence G");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"invalid calling sequence G");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence G");
		}
	}

	[TestClass]
	public class Primitives:WildcardWeavingTestBase
	{
		[TestMethod]
		public void TestDouble()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Double)(System.Double.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Double.MinValue},ta);
			Assert.AreEqual( (System.Double)(System.Double.MinValue), ap.TestDouble((System.Double)(System.Double.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Double)(System.Double.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Double.MaxValue},ta);
			Assert.AreEqual( (System.Double)(System.Double.MaxValue), ap.TestDouble((System.Double)(System.Double.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSingle()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Single)(System.Single.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Single.MinValue},ta);
			Assert.AreEqual( (System.Single)(System.Single.MinValue), ap.TestSingle((System.Single)(System.Single.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Single)(System.Single.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Single.MaxValue},ta);
			Assert.AreEqual( (System.Single)(System.Single.MaxValue), ap.TestSingle((System.Single)(System.Single.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestDecimal()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Decimal)(System.Decimal.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Decimal.MinValue},ta);
			Assert.AreEqual( (System.Decimal)(System.Decimal.MinValue), ap.TestDecimal((System.Decimal)(System.Decimal.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Decimal)(System.Decimal.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Decimal.MaxValue},ta);
			Assert.AreEqual( (System.Decimal)(System.Decimal.MaxValue), ap.TestDecimal((System.Decimal)(System.Decimal.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUIntPtr()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.UIntPtr)System.UInt32.MinValue},ta);
			Assert.AreEqual( (System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue), ap.TestUIntPtr((System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.UIntPtr)((System.UIntPtr)System.UInt32.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.UIntPtr)System.UInt32.MaxValue},ta);
			Assert.AreEqual( (System.UIntPtr)((System.UIntPtr)System.UInt32.MaxValue), ap.TestUIntPtr((System.UIntPtr)((System.UIntPtr)System.UInt32.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestIntPtr()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.IntPtr)((System.IntPtr)System.Int32.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.IntPtr)System.Int32.MinValue},ta);
			Assert.AreEqual( (System.IntPtr)((System.IntPtr)System.Int32.MinValue), ap.TestIntPtr((System.IntPtr)((System.IntPtr)System.Int32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.IntPtr)((System.IntPtr)System.Int32.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.IntPtr)System.Int32.MaxValue},ta);
			Assert.AreEqual( (System.IntPtr)((System.IntPtr)System.Int32.MaxValue), ap.TestIntPtr((System.IntPtr)((System.IntPtr)System.Int32.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt64()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.UInt64)(System.UInt64.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt64.MinValue},ta);
			Assert.AreEqual( (System.UInt64)(System.UInt64.MinValue), ap.TestUInt64((System.UInt64)(System.UInt64.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.UInt64)(System.UInt64.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt64.MaxValue},ta);
			Assert.AreEqual( (System.UInt64)(System.UInt64.MaxValue), ap.TestUInt64((System.UInt64)(System.UInt64.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt64()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Int64)(System.Int64.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int64.MinValue},ta);
			Assert.AreEqual( (System.Int64)(System.Int64.MinValue), ap.TestInt64((System.Int64)(System.Int64.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Int64)(System.Int64.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int64.MaxValue},ta);
			Assert.AreEqual( (System.Int64)(System.Int64.MaxValue), ap.TestInt64((System.Int64)(System.Int64.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt32()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.UInt32)(System.UInt32.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt32.MinValue},ta);
			Assert.AreEqual( (System.UInt32)(System.UInt32.MinValue), ap.TestUInt32((System.UInt32)(System.UInt32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.UInt32)(System.UInt32.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt32.MaxValue},ta);
			Assert.AreEqual( (System.UInt32)(System.UInt32.MaxValue), ap.TestUInt32((System.UInt32)(System.UInt32.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt32()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Int32)(System.Int32.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int32.MinValue},ta);
			Assert.AreEqual( (System.Int32)(System.Int32.MinValue), ap.TestInt32((System.Int32)(System.Int32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Int32)(System.Int32.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int32.MaxValue},ta);
			Assert.AreEqual( (System.Int32)(System.Int32.MaxValue), ap.TestInt32((System.Int32)(System.Int32.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt16()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.UInt16)(System.UInt16.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt16.MinValue},ta);
			Assert.AreEqual( (System.UInt16)(System.UInt16.MinValue), ap.TestUInt16((System.UInt16)(System.UInt16.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.UInt16)(System.UInt16.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt16.MaxValue},ta);
			Assert.AreEqual( (System.UInt16)(System.UInt16.MaxValue), ap.TestUInt16((System.UInt16)(System.UInt16.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt16()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Int16)(System.Int16.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int16.MinValue},ta);
			Assert.AreEqual( (System.Int16)(System.Int16.MinValue), ap.TestInt16((System.Int16)(System.Int16.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Int16)(System.Int16.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int16.MaxValue},ta);
			Assert.AreEqual( (System.Int16)(System.Int16.MaxValue), ap.TestInt16((System.Int16)(System.Int16.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestByte()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Byte)(System.Byte.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Byte.MinValue},ta);
			Assert.AreEqual( (System.Byte)(System.Byte.MinValue), ap.TestByte((System.Byte)(System.Byte.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Byte)(System.Byte.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Byte.MaxValue},ta);
			Assert.AreEqual( (System.Byte)(System.Byte.MaxValue), ap.TestByte((System.Byte)(System.Byte.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSByte()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.SByte)(System.SByte.MinValue));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.SByte.MinValue},ta);
			Assert.AreEqual( (System.SByte)(System.SByte.MinValue), ap.TestSByte((System.SByte)(System.SByte.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.SByte)(System.SByte.MaxValue));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.SByte.MaxValue},ta);
			Assert.AreEqual( (System.SByte)(System.SByte.MaxValue), ap.TestSByte((System.SByte)(System.SByte.MaxValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestBool()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.Boolean)(false));
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{false},ta);
			Assert.AreEqual( (System.Boolean)(false), ap.TestBool((System.Boolean)(false)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"1 method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"2 method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"3 method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.Boolean)(true));
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{true},ta);
			Assert.AreEqual( (System.Boolean)(true), ap.TestBool((System.Boolean)(true)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestDoubleRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Double.MinValue);
			System.Double p=System.Double.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Double.MinValue},ta);
			Assert.AreEqual( System.Double.MinValue, ap.TestDoubleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Double.MaxValue);
			p=System.Double.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Double.MaxValue},ta);
			Assert.AreEqual( System.Double.MaxValue, ap.TestDoubleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSingleRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Single.MinValue);
			System.Single p=System.Single.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Single.MinValue},ta);
			Assert.AreEqual( System.Single.MinValue, ap.TestSingleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Single.MaxValue);
			p=System.Single.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Single.MaxValue},ta);
			Assert.AreEqual( System.Single.MaxValue, ap.TestSingleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestDecimalRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Decimal.MinValue);
			System.Decimal p=System.Decimal.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Decimal.MinValue},ta);
			Assert.AreEqual( System.Decimal.MinValue, ap.TestDecimalRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Decimal.MaxValue);
			p=System.Decimal.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Decimal.MaxValue},ta);
			Assert.AreEqual( System.Decimal.MaxValue, ap.TestDecimalRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUIntPtrRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.UIntPtr)System.UInt32.MinValue);
			System.UIntPtr p=(System.UIntPtr)System.UInt32.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{(System.UIntPtr)System.UInt32.MinValue},ta);
			Assert.AreEqual( (System.UIntPtr)System.UInt32.MinValue, ap.TestUIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.UIntPtr)System.UInt32.MaxValue);
			p=(System.UIntPtr)System.UInt32.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{(System.UIntPtr)System.UInt32.MaxValue},ta);
			Assert.AreEqual( (System.UIntPtr)System.UInt32.MaxValue, ap.TestUIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestIntPtrRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect((System.IntPtr)System.Int32.MinValue);
			System.IntPtr p=(System.IntPtr)System.Int32.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{(System.IntPtr)System.Int32.MinValue},ta);
			Assert.AreEqual( (System.IntPtr)System.Int32.MinValue, ap.TestIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect((System.IntPtr)System.Int32.MaxValue);
			p=(System.IntPtr)System.Int32.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{(System.IntPtr)System.Int32.MaxValue},ta);
			Assert.AreEqual( (System.IntPtr)System.Int32.MaxValue, ap.TestIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt64Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.UInt64.MinValue);
			System.UInt64 p=System.UInt64.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt64.MinValue},ta);
			Assert.AreEqual( System.UInt64.MinValue, ap.TestUInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.UInt64.MaxValue);
			p=System.UInt64.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt64.MaxValue},ta);
			Assert.AreEqual( System.UInt64.MaxValue, ap.TestUInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt64Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Int64.MinValue);
			System.Int64 p=System.Int64.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int64.MinValue},ta);
			Assert.AreEqual( System.Int64.MinValue, ap.TestInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Int64.MaxValue);
			p=System.Int64.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int64.MaxValue},ta);
			Assert.AreEqual( System.Int64.MaxValue, ap.TestInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt32Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.UInt32.MinValue);
			System.UInt32 p=System.UInt32.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt32.MinValue},ta);
			Assert.AreEqual( System.UInt32.MinValue, ap.TestUInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.UInt32.MaxValue);
			p=System.UInt32.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt32.MaxValue},ta);
			Assert.AreEqual( System.UInt32.MaxValue, ap.TestUInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt32Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Int32.MinValue);
			System.Int32 p=System.Int32.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int32.MinValue},ta);
			Assert.AreEqual( System.Int32.MinValue, ap.TestInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Int32.MaxValue);
			p=System.Int32.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int32.MaxValue},ta);
			Assert.AreEqual( System.Int32.MaxValue, ap.TestInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt16Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.UInt16.MinValue);
			System.UInt16 p=System.UInt16.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt16.MinValue},ta);
			Assert.AreEqual( System.UInt16.MinValue, ap.TestUInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.UInt16.MaxValue);
			p=System.UInt16.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.UInt16.MaxValue},ta);
			Assert.AreEqual( System.UInt16.MaxValue, ap.TestUInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt16Ref()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Int16.MinValue);
			System.Int16 p=System.Int16.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int16.MinValue},ta);
			Assert.AreEqual( System.Int16.MinValue, ap.TestInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Int16.MaxValue);
			p=System.Int16.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Int16.MaxValue},ta);
			Assert.AreEqual( System.Int16.MaxValue, ap.TestInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestByteRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.Byte.MinValue);
			System.Byte p=System.Byte.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Byte.MinValue},ta);
			Assert.AreEqual( System.Byte.MinValue, ap.TestByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.Byte.MaxValue);
			p=System.Byte.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.Byte.MaxValue},ta);
			Assert.AreEqual( System.Byte.MaxValue, ap.TestByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSByteRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(System.SByte.MinValue);
			System.SByte p=System.SByte.MinValue;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.SByte.MinValue},ta);
			Assert.AreEqual( System.SByte.MinValue, ap.TestSByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(System.SByte.MaxValue);
			p=System.SByte.MaxValue;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{System.SByte.MaxValue},ta);
			Assert.AreEqual( System.SByte.MaxValue, ap.TestSByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestBoolRef()
		{
			callorder=1;
			TestAspectBase ta=new MultiMatchAspect(false);
			System.Boolean p=false;
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{false},ta);
			Assert.AreEqual( false, ap.TestBoolRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new MultiMatchAspect(true);
			p=true;
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{true},ta);
			Assert.AreEqual( true, ap.TestBoolRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
	}
}
