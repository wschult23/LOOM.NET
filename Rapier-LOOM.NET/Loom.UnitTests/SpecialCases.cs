// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;

namespace Loom.UnitTests.SpecialCases
{
    // Die Reihenfolge ist leider undefiniert
    [SpecialCasesTestBase.AspectBefore(2)]
    [SpecialCasesTestBase.AspectInstead(2)]
    public class BDerived : TestBase.B
    {
    }


    public class SpecialCasesTestBase : TestBase
	{

        public class AspectAfter : AspectAttribute
        {
            public int seq;
            public AspectAfter(int seq)
            {
                this.seq = seq;
            }
            [Create(Advice.Around)]
            public T ctor<T>([JPContext] Context ctx)
            {
                return (T)ctx.Call();
            }

            [Call(Advice.Before)]
            [IncludeAll]
            public void Test(int i)
            {
                Assert.AreEqual(seq, callorder, "invalid call order");
                callorder++;
            }
        }

        public class AspectBefore : AspectAttribute
		{
			public int seq;
			public AspectBefore(int seq)
			{
				this.seq=seq;
			}
            [Create(Advice.Around)]
            public T ctor<T>([JPContext] Context ctx)
            {
                return (T)ctx.Call();
            }

			[Call(Advice.Before)] [IncludeAll]
			public void Test(int i)
			{
				Assert.IsTrue(callorder>= seq,"invalid call order");
				callorder++;
			}
		}
        public class AspectInstead : AspectAttribute
        {
            public int seq;
            public AspectInstead(int seq)
            {
                this.seq = seq;
            }

            [Call(Advice.Around)]
            [IncludeAll]
            public T Test<T>([JPContext] Context Context, int i)
            {
                Assert.IsTrue(callorder>= seq, "invalid call order");
                callorder++;
                return (T)Context.Invoke(new object[] { i });
            }
            /* TODO
            [IncludeAll]
            [Call(Advice.Around)]
            public T Test<T>([AspectParameter] AspectContext Context, int i)
            {
                Assert.AreEqual(seq, callorder, "invalid call order");
                callorder++;
                return (T)Context.Invoke(new object[] { i });
                
            }
              */
        }
        public class DifferentAspectBefore : AspectAttribute
		{
			public int seq;
			public DifferentAspectBefore(int seq)
			{
				this.seq=seq;
			}
			[Call(Advice.Before)] [IncludeAll]
			public void Test(Int16 i)
			{
				Assert.AreEqual(seq,callorder,"invalid call order");
				callorder++;
			}
		}

		public class AProperty:ClassBase
		{
			public virtual int Int
			{
				set
				{
					RegisterCall(ref testCall,"PropertyAspect.set_Int");
				}
				get
				{
					RegisterCall(ref testCall,"PropertyAspect.get_Int");
					return 0;
				}
			}

            
			public virtual double Double
			{
				get
				{
					RegisterCall(ref testCall,"PropertyAspect.get_Double");
					return 0;
				}
			}
		}

		public class PropertyAspect:TestAspectBase
		{
            [Access(Advice.Before)]
            public void set_Int(int value)
            {
                RegisterCall(ref aspectTestBefore, "PropertyAspect.set_Int");
            }

            [Access(Advice.Before)]
            public void get_Int()
            {
                RegisterCall(ref aspectTestBefore,"PropertyAspect.get_Int");
            }
		}

		public class AllDeclaredAspect:TestAspectBase
		{
			[Call(Advice.Around)] [IncludeAll]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestBefore,"AllDeclaredAspect.f");
				return (T)Context.Invoke(args);
			}
		}

		public class BeforeAspectForManyParameters:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(int a,int b,int c,int d,int e,int f,int g,int h,int i,int j,int k,int l,int m, int n)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForManyParameters.f");
			}
		}
		public class BeforeAspectForManyParametersWithWildcard:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForManyParametersWithWildcard.f");
			}
		}
		public class IFDTestAttribute:Attribute
		{}
		public class IFDTestAspect:TestAspectBase
		{
			[IncludeAnnotatedAttribute(typeof(IFDTestAttribute))]
			[Call(Advice.Around)]
            public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestBefore,"IFDTestAspect.f");
				return (T)Context.Invoke(args);
			}	
		}

		public class IFDTest:ClassBase
		{
			[IFDTest]
			public virtual void Test1()
			{
				RegisterCall(ref testCall,"IFDTest.Test()");
				if(bThrowException) throw new TestException();
				return;
			}
			public virtual void Test2()
			{
				RegisterCall(ref testCall,"IFDTest.Test()");
				if(bThrowException) throw new TestException();
				return;
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class BeforeAspectForVirtualMethodTest:TestAspectBase
		{
			[Call(Advice.Before)][Include("Inc")]
			public void f(int i)
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspectForVirtualMethodTest.f");
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class BeforeAspectForVirtualMethodTest1:TestAspectBase
		{
			[Call(Advice.Before)][Include("Inc")]
			public void f(int i)
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspectForVirtualMethodTest1.f");
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class InsteadAspectForVirtualMethodTest:TestAspectBase
		{
			[Call(Advice.Around)][Include("Inc")]
            public T f<T>([JPContext] Context Context, int i)
			{
				RegisterCall(ref aspectTestInstead, "InsteadAspectForVirtualMethodTest.f");
				object[] args = new object[1];
				args[0] = i;
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class InsteadAspectForVirtualMethodTest1:TestAspectBase
		{
			[Call(Advice.Around)][Include("Inc")]
            public T f<T>([JPContext] Context Context, int i)
			{
				RegisterCall(ref aspectTestInstead, "InsteadAspectForVirtualMethodTest1.f");
				object[] args = new object[1];
				args[0] = i;
				object o=Context.Invoke(args);
				return (T)o;
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class BeforeAspectForVirtualMethodTest2:TestAspectBase
		{
			[Call(Advice.Before)][Include("Add")]
			public void f(params object[] args)
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspectForVirtualMethodTest2.f");				
			}
		}
		/// <summary>AspectAttribute for VirtualMethodAndMultipleAspects tests.</summary>
		public class BeforeAspectForVirtualMethodTest3:TestAspectBase
		{
			[Call(Advice.Before)][Include("Add")]
			public void f(params object[] args)
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspectForVirtualMethodTest3.f");				
			}
		}
		public class BeforeFinalize:TestAspectBase
		{
            [Finalize(Advice.Before)]
			public void f()
			{
				RegisterCall(ref aspectTestBefore, "BeforeFinalize.f");				
			}
		}
        /* TODO
		public class InsteadFinalize:TestAspectBase
		{
			//[Call(Advice.Around)][Include("Finalize")]
            [Destroy(Advice.Around)]
			public object f([AspectParameter] AspectContext Context)
			{
				RegisterCall(ref aspectTestBefore, "InsteadFinalize.f");
				object o = Context.Call();
				return o;
			}
		}
        */

		/// <summary>AspectAttribute for MultibleAspectWeaving tests.</summary>
		public class BeforeAspect:TestAspectBase
		{
			[Call(Advice.Before)][Include("Inc")]
			public void Inc(int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect.Inc()");
			}
		}
		/// <summary>AspectAttribute for MultibleAspectWeaving tests.</summary>
		public class BeforeAspect1:TestAspectBase
		{
			[Call(Advice.Before)][Include("Inc")]
			public void Inc(int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect1.Inc()");
			}
		}
		/// <summary>AspectAttribute for MultibleAspectWeaving tests.</summary>
		public class BeforeAspect2:TestAspectBase
		{
			[Call(Advice.Before)][Include("Inc")]
			public void Inc(int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect2.Inc()");
			}
		}
        public class NotMatchingAspect1 : TestAspectBase
        {
            [Call(Advice.Before)]
            [Include("foo")]
            public void bar(params object[] parameters)
            {

            }

        }
        public class NotMatchingAspect2 : TestAspectBase
        {
            [Call(Advice.After)]
            [Include("foo2")]
            public void bar2(params object[] parameters)
            {

            }
        }

        public class MatchingEverythingAspect : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void bar2(params object[] parameters)
            {

            }
        }



        public class TargetClassStatic
        {
            [NotMatchingAspect1]
            [NotMatchingAspect2]
            public virtual void a()
            {
            }
        }
        public class TargetClassDynamic
        {
            public virtual void a()
            {
            }
        }
        public class TargetClassStaticMatch
        {
            [NotMatchingAspect1]
            [MatchingEverythingAspect]
            [NotMatchingAspect2]
            public virtual void a()
            {
            }
        }




	}
	/// <summary>Tests for interweaving of multiple aspects.</summary>	
	[TestClass]
	public class MultibleAspectWeaving:SpecialCasesTestBase
	{
		[TestMethod]		
		public void SameAspectsInvokeBefore()
		{
			callorder=1;
            B b = Weaver.Create<B>(new AspectAttribute[] { new AspectBefore(3), new AspectBefore(2), new AspectBefore(1) });
			b.Test(1);
			Assert.AreEqual(5,callorder,"AspectAttribute not called");
		}
		[TestMethod]		
		public void SameAspectsInvokeInstead()
		{
			callorder=1;
            B b = Weaver.Create<B>(new AspectAttribute[] { new AspectInstead(3), new AspectInstead(2), new AspectInstead(1) });
			b.Test(1);
			Assert.AreEqual(5,callorder,"AspectAttribute not called");
		}
		[TestMethod]		
		public void DifferentAspectsSameMethod()
		{
			callorder=1;
            B b = Weaver.Create<B>(new AspectAttribute[] { new AspectBefore(2), new AspectInstead(1) });
			b.Test(1);
			Assert.AreEqual(4,callorder,"AspectAttribute not called");
		}
        [TestMethod]
        public void StaticAndDynamic()
        {
            callorder = 1;
            BDerived b = Weaver.Create<BDerived>(new AspectAfter(1));
            b.Test(1);
            Assert.AreEqual(5, callorder, "AspectAttribute not called");
        }
        [TestMethod]		
		public void DifferentAspectsDifferentMethods()
		{
			callorder=1;
			AllPrimitives b=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{1},new AspectAttribute[]{new AspectBefore(1), new DifferentAspectBefore(2)});
            //AllPrimitives b = Weaver.Create<AllPrimitives>(new object[] { 1 }, new AspectAttribute[] { new AspectBefore(1), new DifferentAspectBefore(2) });
			b.TestInt32(1);
			b.expected=(Int16)2;
			b.TestInt16(2);
			Assert.AreEqual(3,callorder,"AspectAttribute not called");
		}
		/// <summary>Tests for interweaving the same aspects multiple with a class derived from an interface.</summary>
		[TestMethod]
		public void SameAspects()
		{
            callorder = 1;
			I1 o = (I1)Weaver.CreateInstance(typeof(VS2),null,new AspectAttribute[]{new BeforeAspect(),new BeforeAspect(),new BeforeAspect()});
			Assert.AreEqual(2,o.Inc(1),"invalid return value");
			Assert.AreEqual(4,((VS2)o).testCall,"interweaving failed");
		}
		/// <summary>Tests for interweaving different aspects with a class derived from an interface.</summary>
		[TestMethod]
		public void DifferentAspects()
		{
            callorder = 1;
			I1 o = (I1)Weaver.CreateInstance(typeof(VS2),null,new AspectAttribute[]{new BeforeAspect(),new BeforeAspect1(),new BeforeAspect2()});
			Assert.AreEqual(2,o.Inc(1),"invalid return value");
			Assert.AreEqual(4,((VS2)o).testCall,"interweaving failed");
		}
	}
	[TestClass]
	public class AttributeWeaving:SpecialCasesTestBase
	{
		[TestMethod]		
		public void Simple()
		{
			callorder=1;
			IFDTest b=(IFDTest)Weaver.CreateInstance(typeof(IFDTest),null, new IFDTestAspect());
			b.Test1();
			Assert.AreEqual(2,((ClassBase)b).testCall,"invalid calling sequence");
			b.testCall=0;
			b.Test2();
			Assert.AreEqual(3,((ClassBase)b).testCall,"invalid calling sequence");
		}

	}
	[TestClass]
	public class PropertyWeaving:SpecialCasesTestBase
	{
		[TestMethod]
		public void Simple()
		{
			callorder=1;
			TestAspectBase asp=new PropertyAspect();
            AProperty i = Weaver.Create<AProperty>(asp) as AProperty;
			i.Int=0;
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			callorder=1;
			((ClassBase)i).testCall=0;
			asp.aspectTestBefore=0;
			int ti=i.Int;
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			callorder=1;
			((ClassBase)i).testCall=0;
			double td=i.Double;
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
	}
	[TestClass]
	public class AllDeclaredWeaving:SpecialCasesTestBase
	{
		[TestMethod]		
		public void SimpleClasses()
		{
			callorder=1;
			TestAspectBase tb=new AllDeclaredAspect();
			B b=(B)Weaver.CreateInstance(typeof(B),null,new AspectAttribute[]{tb});
			b.Test(1);
			Assert.AreEqual(3,callorder,"AspectAttribute not called");
			callorder=1;
			tb.aspectTestBefore=0;
			b.GetHashCode();
			Assert.AreEqual(2,callorder,"AspectAttribute not called");
		}
	}

	[TestClass]
	public class ManyParametersWeaving:SpecialCasesTestBase
	{
		[TestMethod]
		public void WithManyParameters()
		{
			callorder=1;
			BeforeAspectForManyParameters a=new BeforeAspectForManyParameters();
            K k = Weaver.Create<K>(a);
			k.k(1,2,3,4,5,6,7,8,9,10,11,12,13,14);
			Assert.AreEqual(1,a.aspectTestBefore,"AspectAttribute not called");
			Assert.AreEqual(2,k.testCall,"object not created");
		}
		[TestMethod]
		public void WithWildcard()
		{
			callorder=1;
			BeforeAspectForManyParametersWithWildcard a=new BeforeAspectForManyParametersWithWildcard();
            K k = Weaver.Create<K>(a);
			k.k(1,2,3,4,5,6,7,8,9,10,11,12,13,14);
			Assert.AreEqual(1,a.aspectTestBefore,"AspectAttribute not called");
			Assert.AreEqual(2,k.testCall,"object not created");
		}
	}
	/// <summary>Tests for interweaving multiple aspects and pitarget class with virtual method.</summary>
	[TestClass]
	public class VirtualMethodAndMultipleAspects:SpecialCasesTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void InvokeBeforeSameAspect()
		{
			BeforeAspectForVirtualMethodTest a1 = new BeforeAspectForVirtualMethodTest();
			BeforeAspectForVirtualMethodTest a2 = new BeforeAspectForVirtualMethodTest();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			I1 t = (I1)Weaver.CreateInstance(typeof(VS1),null,aspects);
			Assert.AreEqual(2,t.Inc(1),"method not called");
			Assert.AreEqual(2,a1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestBefore,"Aspect not jpinterwoven");
			VS1 t1 = (VS1)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
		[TestMethod]
		public void InvokeBeforeSameAspects2()
		{
			BeforeAspectForVirtualMethodTest2 a1 = new BeforeAspectForVirtualMethodTest2();
			BeforeAspectForVirtualMethodTest2 a2 = new BeforeAspectForVirtualMethodTest2();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			IW t = (IW)Weaver.CreateInstance(typeof(W),null,aspects);
			Assert.AreEqual(6,t.Add(1,2,3),"method not called");
			Assert.AreEqual(2,a1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestBefore,"Aspect not jpinterwoven");
			W t1 = (W)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
		[TestMethod]
		public void InvokeBeforeDifferentAspects()
		{
			BeforeAspectForVirtualMethodTest a1 = new BeforeAspectForVirtualMethodTest();
			BeforeAspectForVirtualMethodTest1 a2 = new BeforeAspectForVirtualMethodTest1();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			I1 t = (I1)Weaver.CreateInstance(typeof(VS1),null,aspects);
			Assert.AreEqual(2,t.Inc(1),"method not called");
			Assert.AreEqual(2,a1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestBefore,"Aspect not jpinterwoven");
			VS1 t1 = (VS1)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
		[TestMethod]
		public void InvokeBeforeDifferentAspects2()
		{
			BeforeAspectForVirtualMethodTest2 a1 = new BeforeAspectForVirtualMethodTest2();
			BeforeAspectForVirtualMethodTest3 a2 = new BeforeAspectForVirtualMethodTest3();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			IW t = (IW)Weaver.CreateInstance(typeof(W),null,aspects);
			Assert.AreEqual(6,t.Add(1,2,3),"method not called");
			Assert.AreEqual(2,a1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestBefore,"Aspect not jpinterwoven");
			W t1 = (W)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
		[TestMethod]
		public void InvokeInsteadSameAspect()
		{
			InsteadAspectForVirtualMethodTest a1 = new InsteadAspectForVirtualMethodTest();
			InsteadAspectForVirtualMethodTest a2 = new InsteadAspectForVirtualMethodTest();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			I1 t = (I1)Weaver.CreateInstance(typeof(VS1),null,aspects);
			Assert.AreEqual(2,t.Inc(1),"method not called");
			Assert.AreEqual(2,a1.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestInstead,"Aspect not jpinterwoven");
			VS1 t1 = (VS1)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
		[TestMethod]
		public void InvokeInsteadDifferentAspects()
		{
			InsteadAspectForVirtualMethodTest a1 = new InsteadAspectForVirtualMethodTest();
			InsteadAspectForVirtualMethodTest1 a2 = new InsteadAspectForVirtualMethodTest1();
			AspectAttribute[] aspects = new AspectAttribute[2];
			aspects[0] = a1;
			aspects[1] = a2;
			I1 t = (I1)Weaver.CreateInstance(typeof(VS1),null,aspects);
			Assert.AreEqual(2,t.Inc(1),"method not called");
			Assert.AreEqual(2,a1.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(1,a2.aspectTestInstead,"Aspect not jpinterwoven");
			VS1 t1 = (VS1)t;
			Assert.AreEqual(3,t1.testCall,"object not jpinterwoven"); 
		}
	}
	/// <summary>Tests for interweaving finalize method.</summary>
	[TestClass]
	public class Finalize:SpecialCasesTestBase
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
			BeforeFinalize a = new BeforeFinalize();
			X o = (X)Weaver.CreateInstance(typeof(X),null,a);
			Assert.AreEqual(1,o.testCall,"object not jpinterwoven");			
			o=null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Assert.AreEqual(2,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,X.call,"finalize not called");
		}
        /* TODO
		[TestMethod]
		public void Instead()
		{
			InsteadFinalize a = new InsteadFinalize();
			X o = (X)Weaver.CreateInstance(typeof(X),null,a);
			Assert.AreEqual(1,o.testCall,"object not jpinterwoven");
			o=null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Assert.AreEqual(2,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,X.call,"finalize not called");
		}
         * */
	}
    /// <summary>
    /// Weaver matching test - if no aspect was interwoven (static or dynamically), the weaver shouldn't throw an exception
    /// </summary>
    [TestClass]
    public class NoMatch : SpecialCasesTestBase
    {
        [TestMethod]
        public void StaticWeavingNoMatching()
        {
            TargetClassStatic c = Weaver.Create<TargetClassStatic>();
            c.a();

            Aspect[] aspectArray = new Aspect[2];
            aspectArray[0] = new NotMatchingAspect1();
            aspectArray[1] = new NotMatchingAspect2();

            TargetClassDynamic c2 = Weaver.Create<TargetClassDynamic>(aspectArray);
            c2.a();
        }

        [TestMethod]
        public void StaticWeavingOneMatch()
        {
        //    TargetClassStaticMatch c = Weaver.Create<TargetClassStaticMatch>();
         //   c.a();

            Aspect[] aspectArray = new Aspect[3];
            aspectArray[0] = new NotMatchingAspect1();
            aspectArray[1] = new MatchingEverythingAspect();
            aspectArray[2] = new NotMatchingAspect2();
            

            TargetClassDynamic c2 = Weaver.Create<TargetClassDynamic>(aspectArray);
            c2.a();
        }

    }
}