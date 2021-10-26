// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Loom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;



namespace Loom.UnitTests.InvokeInsteadWeaving
{
	public class AdviceAroundWeavingTestBase:TestBase
	{
		[ReentranceAspect]
		public class ReentranceClass
		{
			public virtual int Call1(int i)
			{
				Assert.AreEqual(1,i,"invalid parameter");
				return Call2(i+1);
			}
			public virtual int Call2(int i)
			{
				Assert.AreEqual(2,i,"invalid parameter");
				return Call3(i+1);
			}
			public virtual int Call3(int i)
			{
				Assert.AreEqual(3,i,"invalid parameter");
				return i;
			}
		}

		public class AdviceAroundAndAfterAspect:TestAspectBase
		{
			[Call(Advice.Around)]
			public void Test([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestInstead,"InvokeInsteadAndAfterAspect.Test()");
				Context.Call();
			}
			[Call(Advice.After)] [Include("Test")]
			public void TestAfter()
			{
				RegisterCall(ref aspectTestAfter,"InvokeInsteadAndAfterAspect.TestAfter()");
			}
		}

		public class ReentranceAspect:AspectAttribute
		{
			public int seq=0;
			[Call(Advice.Around)] [IncludeAll]
			public T Test<T>([JPContext] Context Context, int i)
			{
				seq++;
				Assert.AreEqual(seq, i,"Invalid parameter value.");
				Context ctx=Context;
				int nseq=(int)Context.Invoke(new object[]{i});
				Assert.AreEqual(ctx, Context,"AspectAttribute context changed");
				Assert.AreEqual(3, nseq,"Invalid parameter value.");
				return (T)(object)nseq;
			}
		}

		public class SimpleAspect:TestAspectBase
		{
			[Call(Advice.Around)] [Include("Test")]
            public T TestA<T>([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestA()");
				return (T)Context.Call();
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestB<T>([JPContext] Context Context, int i)
			{
				Assert.AreEqual(23,i,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestB()");
				return (T)Context.Invoke(new object[]{i});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestC<T>([JPContext] Context Context, C c)
			{
				Assert.IsNotNull(c,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestC()");
				return (T)Context.Invoke(new object[]{c});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestD<T>([JPContext] Context Context, ref D d)
			{
				Assert.IsNotNull(d,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestD()");
				return (T)Context.Invoke(new object[]{d});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestE<T>([JPContext] Context Context, V v)
			{
				Assert.AreEqual(1,v.i,"Invalid parameter value.");
				Assert.AreEqual(2.0,v.d,"Invalid parameter value.");
				Assert.AreEqual("Test",v.s,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestE()");
				return (T)Context.Invoke(new object[]{v});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestF<T>([JPContext] Context Context, ref double f)
			{
				Assert.AreEqual(2.0,f,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestF()");
				return (T)Context.Invoke(new object[]{f});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestG<T>([JPContext] Context Context, ref V v)
			{
				Assert.AreEqual(1,v.i,"Invalid parameter value.");
				Assert.AreEqual(2.0,v.d,"Invalid parameter value.");
				Assert.AreEqual("Test",v.s,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestF()");
				return (T)Context.Invoke(new object[]{v});
			}
			[Call(Advice.Around)] [Include("Test")]
			public T TestG<T>([JPContext] Context Context, Numbers n)
			{
				Assert.AreEqual(Numbers.One,n,"Invalid parameter value.");
				RegisterCall(ref aspectTestBefore,"SimpleAspect.TestH()");
				return (T)Context.Invoke(new object[]{n});
			}
		}

		public class WildcardAspect:TestAspectBase
		{
			private object expected;
			private bool check;
			public WildcardAspect(object expected)
			{
				this.expected=expected;
				this.check=true;
			}
			public WildcardAspect()
			{
				this.check=false;
			}

			[Call(Advice.Around)] [IncludeAll] [IncludeDeclaredOnly]
			public T Test<T>([JPContext] Context Context, params object[] param)
			{
				if(check)
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
				}
				RegisterCall(ref aspectTestBefore,"WildcardAspect.Test()");
				return (T)Context.Invoke(param);
			}
		}
		public class ContextInvokeOnAspect:TestAspectBase
		{
			object obj;
			public ContextInvokeOnAspect(object obj)
			{
				this.obj=obj;
			}
			[Call(Advice.Around)]
			[Include("Test")]
			[Include("Inc")]
			public T Test<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"ContextInvokeOnAspect.Test()");
				Assert.IsNotNull(Context,"AspectContext is null");
				return (T)Context.InvokeOn(obj,args);
			}
		}

        public class ContextRecallAspect : TestAspectBase
        {
            [Call(Advice.Around)]
            [MatchMode(Loom.JoinPoints.Match.All)]
            [Include("Test")]
            [IncludeNonInterwoven]
            public T Test<T>([JPContext] Context Context, params object[] args)
            {
                RegisterCall(ref aspectTestInstead, "ContextReCallAspect.Test()");
                Assert.IsNotNull(Context, "AspectContext is null");
                Assert.AreEqual(0, args.Length);
                args = new object[] { 1 };
                int iRes = (int)Context.ReCall(args);
                Assert.AreEqual(1, iRes);
                return default(T);
            }

            [Call(Advice.Initialize)]
            public void Test(int i)
            {
            }


        }
	}

	[TestClass]
	public class SimpleInvoke:AdviceAroundWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new SimpleAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
			a.Test();
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence A");
			Assert.AreEqual(2,a.testCall,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new SimpleAspect();
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
			ta=new SimpleAspect();
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
			ta=new SimpleAspect();
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
			ta=new SimpleAspect();
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
			ta=new SimpleAspect();
			double d=2.0;
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
			ta=new SimpleAspect();
			G g=(G)Weaver.CreateInstance(typeof(G),null,ta);
			//Weaver.Saveprovider();
			V vold=v;
			V vret=g.Test(ref v);
			Assert.AreEqual(vold,v,"invalid reference");
			Assert.AreEqual(vret,v,"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			//Assert.AreEqual(2,g.testCall,"invalid calling sequence G");
		}
		[TestMethod]
		public void ClassRef()
		{
			callorder=1;
			TestAspectBase ta;
			ta=new SimpleAspect();
			D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
			Assert.AreEqual(d,d.Test(ref d),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			Assert.AreEqual(2,d.testCall,"invalid calling sequence G");
		}
		[TestMethod]
		public void Enum()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new SimpleAspect();
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
			ta=new SimpleAspect();
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
	public class WildcardInvoke:AdviceAroundWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new WildcardAspect(null);
			A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
			a.Test();
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence A");
			Assert.AreEqual(2,a.testCall,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new WildcardAspect(23);
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
			ta=new WildcardAspect(carg);
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
			ta=new WildcardAspect(darg);
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
			ta=new WildcardAspect(v);
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
			ta=new WildcardAspect(d);
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
			ta=new WildcardAspect(v);
			G g=(G)Weaver.CreateInstance(typeof(G),null,ta);
			Assert.AreEqual(v,g.Test(ref v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			Assert.AreEqual(2,g.testCall,"invalid calling sequence G");
		}
		[TestMethod]
		public void Enum()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new WildcardAspect(Numbers.One);
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
			ta=new WildcardAspect();
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
	public class SpecialTests:AdviceAroundWeavingTestBase
	{
		[TestMethod]
		public void ReentranceAndStaticWeaving()
		{
			ReentranceClass r=(ReentranceClass)Weaver.CreateInstance(typeof(ReentranceClass));
			ReentranceAspect asp=(ReentranceAspect)Weaver.GetAspects(r,typeof(ReentranceAspect))[0];
			Assert.IsNotNull( asp,"Static weaving failed");
			Assert.AreEqual(3,r.Call1(1),"invalid return value");
			Assert.AreEqual(3, asp.seq,"Aspect not called");
		}

		[TestMethod]
		[ExpectedException(typeof(TestException))]
		public void ThrowException()
		{
			A a=(A)Weaver.CreateInstance(typeof(A),null,new WildcardAspect(null));
			a.bThrowException=true;
			a.Test();
		}

		[TestMethod]
		public void InvokeInsteadAndAfter()
		{
			callorder=1;
			AdviceAroundAndAfterAspect asp=new AdviceAroundAndAfterAspect();
			A a=(A)Weaver.CreateInstance(typeof(A),null,asp);
			a.Test();
			Assert.AreEqual(1, asp.aspectTestInstead,"Aspect not called");
			Assert.AreEqual(2, a.testCall,"Aspect not called");
			Assert.AreEqual(3, asp.aspectTestAfter,"Aspect not called");
		}
	}
	[TestClass]
	public class ContextInvokeOn:AdviceAroundWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>
		/// Der Aspekt leitet den aufruf an eine fremde Instanz weiter
		/// </summary>
		[TestMethod]
		public void InstanceTest()
		{
			A o2=new A();
			ContextInvokeOnAspect a = new ContextInvokeOnAspect(o2);
			A o = (A)Weaver.CreateInstance(typeof(A),null,a);
			o.Test();
			Assert.AreEqual(0,o.testCall,"wrong object method executed");
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,o2.testCall,"object method not executed");
		}

		/// <summary>
		/// Der Aspekt leitet den Aufruf an ein fremdes Interface weiter
		/// </summary>
		[TestMethod]
		public void InterfaceTest()
		{
			VS1 vs1=new VS1();
			ContextInvokeOnAspect a = new ContextInvokeOnAspect(vs1);
			VS vs = Weaver.Create<VS>(a);
            ((I1)vs).Inc(1);
			Assert.AreEqual(0,vs.testCall,"wrong object method executed");
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,vs1.testCall,"object method not executed");
		}
	}
    [TestClass]
    public class ContextRecall : AdviceAroundWeavingTestBase
    {
        [TestInitialize]
        public void Init()
        {
            base.SetUp();
            callorder = 0;
        }
        [TestMethod]
        public void RecallInt()
        {
            ContextRecallAspect aspect = new ContextRecallAspect();
            AllInOne aio = Weaver.Create<AllInOne>(aspect);
            aio.Test();
            Assert.AreEqual(1, aio.testCall, "wrong object method executed");
            Assert.AreEqual(0, aspect.aspectTestInstead, "Aspect not jpinterwoven");

        }
    }
}

