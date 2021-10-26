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

namespace Loom.UnitTests.InterfaceWeaving
{
	/// <summary>InterfaceWeavingTestBase contains all aspects for testing interweaving with create.</summary>
	public class InterfaceWeavingTestBase:TestBase
	{
		public interface IF1
		{
			void Test();
		}
		public interface IF2:IF1
		{
			void Second();
		}

		public interface IF3
		{
			void Test();
			object Test(object o);
			int Test(int i);
		}

		public interface IF4
		{
			int Int
			{
				get;
				set;
			}
			double Double
			{
				get;
			}
		}

        [InterfaceAnnotationAspect]
        public interface IF5
        {
            void Foo();
            void Bar();
        }

        public interface IF6
        {
            [MethodAnnotationAspect]
            void Foo();
            void Bar();
        }

        public interface IF7<T>
        {
            void Test(T t);
        }

        public interface IF8
        {
            void Test<T>(T t);
        }

		public class BeforeAspect:TestAspectBase
		{
			[Call(Advice.Before)]
			public void Test()
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect.Test()");
			}
		}	
		public class InsteadAspect:TestAspectBase
		{
			[Call(Advice.Around)]
			public void Test([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect.Test()");
				Context.Call();
			}
            [Call(Advice.Around)]
            public void Test([JPContext] Context Context, int i)
            {
                RegisterCall(ref aspectTestBefore, "BeforeAspect.Test()");
                Context.Call(i);
            }
            [Call(Advice.Around)]
            public void Test([JPContext] Context Context, string i)
            {
                RegisterCall(ref aspectTestBefore, "BeforeAspect.Test()");
                Context.Call(i);
            }
            [Call(Advice.Around)]
            public void Test<T>([JPContext] Context Context, T i)
            {
                RegisterCall(ref aspectTestBefore, "BeforeAspect.Test()");
                Context.Call(i);
            }
        }
		public class InsteadAspectRecursive:TestAspectBase
		{
			[Call(Advice.Around)]
			public void Test([JPContext] Context Context)
			{
				this.aspectTestInstead++;
				Context.Call();
			}
		}
		public class PropertyAspect:TestAspectBase
		{
            /*
			public int Int
			{
				[Call(Advice.Before)]
				set
				{
					RegisterCall(ref aspectTestBefore,"PropertyAspect.set_Int");
				}
				[Call(Advice.Before)]
				get
				{
					RegisterCall(ref aspectTestBefore,"PropertyAspect.get_Int");
					return 0;
				}
			}*/

            [Access(Advice.Before)]
            public void get_Int()
            {
                RegisterCall(ref aspectTestBefore, "PropertyAspect.get_Int");
            }
            [Access(Advice.Before)]
            public void set_Int(int value)
            {
                RegisterCall(ref aspectTestBefore, "PropertyAspect.set_Int");
            }
		}	
	
        [AttributeUsage(AttributeTargets.Class)]
		public class IntroduceAspect:TestAspectBase
		{
            [Introduce(typeof(IF1),ExistingInterfaces.Advice)]
			public void Test([JPContext] Context Context)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspect.Test()");
				Assert.IsNotNull(Context,"AspectAttribute context not set");
				Assert.IsNotNull(Context.Instance,"AspectAttribute context instance not set");

                if (!Context.CurrentMethod.DeclaringType.IsInterface)
                {
                    Context.Call();
                }
			}
		}


        [AttributeUsage(AttributeTargets.Method)]
        public class IntroduceAspectOnMethod : TestAspectBase
        {
            [Introduce(typeof(IF1),ExistingInterfaces.Advice)]
            public static void Test([JPContext] Context Context)
            {
                var asp=((IAspectInfo)Context.Instance).GetAspects()[0] as IntroduceAspectOnMethod;
                RegisterCall(ref asp.aspectTestBefore, "BeforeAspect.Test()");
                Assert.IsNotNull(Context, "AspectAttribute context not set");
                Assert.IsNotNull(Context.Instance, "AspectAttribute context instance not set");
                if (!Context.CurrentMethod.DeclaringType.IsInterface)
                {
                    Context.Call();
                }
            }
        }

        public class InterfaceAnnotationAspect : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void Before(params object[] args)
            {
                RegisterCall(ref aspectTestBefore, "InterfaceAnnotationAspect.Before");
            }
        }

        public class MethodAnnotationAspect : TestAspectBase
        {
            [Call(Advice.Before)]
            [IncludeAll]
            public void Before(params object[] args)
            {
                RegisterCall(ref aspectTestBefore, "InterfaceAnnotationAspect.Before");
            }
        }

		public class APublic:ClassBase,IF1
		{
			public void Test()
			{
				RegisterCall(ref testCall,"IF1.Test()");
			}
		}

		public class APrivate:ClassBase, IF1
		{
			void IF1.Test()
			{
				RegisterCall(ref testCall,"IF1.Test()");
			}
		}

		public class APrivateRecursive:IF1
		{
			public int reccount=0;
			void IF1.Test()
			{
				if(++reccount<3) ((IF1)this).Test();
			}
		}

		public class APublicRecursive:IF1
		{
			public int reccount=0;
			public void Test()
			{
				if(++reccount<3) ((IF1)this).Test();
			}
		}


		public class AMultibleFunctionsPublic:ClassBase, IF2
		{
			public void Test()
			{
				RegisterCall(ref testCall,"IF1.Test()");
			}
			public void Second()
			{
				RegisterCall(ref testCall,"IF2.Second()");
			}
		}

		public class APartial:ClassBase, IF3
		{
			public void Test()
			{
				RegisterCall(ref testCall,"IF3.Test()");
			}
			public object Test(object o)
			{
				RegisterCall(ref testCall,"IF3.Test(object)");
				return o;
			}
			public int Test(int i)
			{
				RegisterCall(ref testCall,"IF1.Test(int)");
				return i;
			}
		}

		public class AMultibleFunctionsPrivate:ClassBase, IF2
		{
			void IF1.Test()
			{
				RegisterCall(ref testCall,"IF2.Test()");
			}
			void IF2.Second()
			{
				RegisterCall(ref testCall,"IF2.Second()");
			}
		}

        public class GenericInterface : ClassBase, IF7<int>
        {
            public void Test(int t)
            {
                RegisterCall(ref testCall, "IF7.Test()");
            }
        }

        public class GenericInterfacePrivate : ClassBase, IF7<int>, IF7<string>
        {
            void IF7<int>.Test(int t)
            {
                RegisterCall(ref testCall, "IF7<int>.Test()");
            }

            void IF7<string>.Test(string t)
            {
                RegisterCall(ref testCall, "IF7<string>.Test()");
            }
        }

        public class GenericInterfaceMethod : ClassBase, IF8
        {
            public void Test<T>(T t)
            {
                RegisterCall(ref testCall, "IF8.Test()");
            }
        }

        public class GenericInterfaceMethodPrivate : ClassBase, IF8
        {
            void IF8.Test<T>(T t)
            {
                RegisterCall(ref testCall, "IF8.Test()");
            }
        }


		[IntroduceAspect]
		public class ANoInterface
		{}

        [IntroduceAspect]
        public class AWithInterface:ClassBase, IF1
        {
            public void Test()
            {
                RegisterCall(ref testCall, "IF1.Test()");
            }
        }

        public class ANoInterfaceOnMethod
        {
            [IntroduceAspectOnMethod]
            public virtual void foo()
            { }
        }

        public class AInterfaceOnMethod:ClassBase, IF1
        {
            [IntroduceAspectOnMethod]
            public virtual void foo()
            { }

            public void Test()
            {
                RegisterCall(ref testCall, "IF1.Test()");
            }
        }
        
		public class BNoInterface:ANoInterface
		{}

		public class AProperty:ClassBase,IF4
		{
			public int Int
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
			public double Double
			{
				get
				{
					RegisterCall(ref testCall,"PropertyAspect.get_Double");
					return 0;
				}
			}
		}

        public class InterfaceAnnotated : ClassBase, IF5

        /// <summary>AspectAttribute for SameInterfaceWeaving tests.</summary>
        {
            #region IF5 Members

            public void Foo()
            {
                RegisterCall(ref testCall, "InterfaceAnnotated.Foo");
            }

            public void Bar()
            {
                RegisterCall(ref testCall, "InterfaceAnnotated.Bar");
            }

            #endregion

            public virtual void Baz()
            {
                RegisterCall(ref testCall, "InterfaceAnnotated.Baz");
            }
        }

        public class InterfaceMethodAnnotated : ClassBase, IF6

        /// <summary>AspectAttribute for SameInterfaceWeaving tests.</summary>
        {
            #region IF6 Members

            public void Foo()
            {
                RegisterCall(ref testCall, "InterfaceAnnotated.Foo");
            }

            public void Bar()
            {
                RegisterCall(ref testCall, "InterfaceAnnotated.Bar");
            }

            #endregion
        }



        public class SameInterfaceAspect:TestAspectBase
		{
			[Include("Inc")]
			[Call(Advice.Before)]
			public void IncBefore(int i)
			{
				RegisterCall(ref aspectTestBefore, "SameInterfaceWeaving.IncBefore");
			}
			[Call(Advice.Around)]
            public int Inc([JPContext] Context Context, int i)
			{
				RegisterCall(ref aspectTestInstead, "SameInterfaceWeaving.IncInstead");
				Context.Call(i);
				return i++;
			}
		}
		/// <summary>AspectAttribute for InterweavingClassButExcludeItsInterface tests.</summary>
		public class AspectExcludeInterface:TestAspectBase
		{
			[Call(Advice.Before)][IncludeAll()][Exclude(typeof(I1))]
			public void IncBefore(int i)
			{
				RegisterCall(ref aspectTestBefore, "AspectExcludeInterface.IncBefore");
			}
			[Call(Advice.Around)][IncludeAll()][Exclude(typeof(I1))]
            public T IncInstead<T>([JPContext] Context Context, int i)
			{
				RegisterCall(ref aspectTestInstead, "AspectExcludeInterface.IncInstead");
				Context.Call(i);
				return (T)(object)i++;
			}
		}

		private interface ITest
		{
			void foo();
		}

        public interface ITest2
        {
            void notfoo();
        }

		public class TestAspect:TestAspectBase
		{
            [Introduce(typeof(ITest))]
			public virtual void foo()
			{
				RegisterCall(ref testCall,"TestAspect.foo()");
			}
		}

        public class TestAspect2 : TestAspectBase
        {
            [Introduce(typeof(ITest2))]
            public virtual void foo()
            {
                RegisterCall(ref testCall, "TestAspect2.foo()");
            }
        }


		public class TestClass:ClassBase
		{}
	}
	
	[TestClass]
	public class BeforeIFWeaving:InterfaceWeavingTestBase
	{
		[TestMethod]
		public void PublicInterface()
		{
			callorder=1;
			TestAspectBase asp=new BeforeAspect();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APublic),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");

		}
		[TestMethod]
		public void PrivateInterface()
		{
			callorder=1;
			TestAspectBase asp=new BeforeAspect();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APrivate),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");

		}
	}

	[TestClass]
	public class InsteadIFWeaving:InterfaceWeavingTestBase
	{
		[TestMethod]
		public void PublicInterface()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspect();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APublic),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");

		}
		[TestMethod]
		public void PrivateInterface()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspect();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APrivate),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");

		}

		[TestMethod]
		public void PrivateInterfaceRecursive()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspectRecursive();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APrivateRecursive),null,asp);
			i.Test();
			Assert.AreEqual(3,((APrivateRecursive)i).reccount,"invalid calling sequence");
			Assert.AreEqual(3,asp.aspectTestInstead,"invalid calling sequence");
			((APrivateRecursive)i).reccount=0;
			i.Test();
			Assert.AreEqual(6,asp.aspectTestInstead,"invalid calling sequence");
		}
		[TestMethod]
		public void PublicInterfaceRecursive()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspectRecursive();
			IF1 i=(IF1)Weaver.CreateInstance(typeof(APublicRecursive),null,asp);
			i.Test();
			Assert.AreEqual(3,((APublicRecursive)i).reccount,"invalid calling sequence");
			Assert.AreEqual(3,asp.aspectTestInstead,"invalid calling sequence");
			((APublicRecursive)i).reccount=0;
			i.Test();
			Assert.AreEqual(6,asp.aspectTestInstead,"invalid calling sequence");
		}
        [TestMethod]
		public void PublicInterfaceGeneric()
		{
            callorder = 1;
            TestAspectBase asp = new InsteadAspect();
            var i = (IF7<int>)Weaver.CreateInstance(typeof(GenericInterface), null, asp);
            i.Test(42);
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
		}
        [TestMethod]
        public void PrivateInterfaceGeneric()
        {
            callorder = 1;
            TestAspectBase asp = new InsteadAspect();
            var i = (IF7<int>)Weaver.CreateInstance(typeof(GenericInterfacePrivate), null, asp);
            i.Test(42);
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
            var i2 = (IF7<string>)i;
            callorder = 1;
            ((ClassBase)i).testCall = 0;
            asp.aspectTestBefore = 0;
            i2.Test("TesT");
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
        }
        [TestMethod]
        public void PublicInterfaceGenericMethod()
        {
            callorder = 1;
            TestAspectBase asp = new InsteadAspect();
            var i = (IF8)Weaver.CreateInstance(typeof(GenericInterfaceMethod), null, asp);
            i.Test(42);
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
            callorder = 1;
            ((ClassBase)i).testCall = 0;
            asp.aspectTestBefore = 0;
            i.Test("TesT");
        }
        [TestMethod]
        public void PrivateInterfaceGenericMethod()
        {
            callorder = 1;
            TestAspectBase asp = new InsteadAspect();
            var i = (IF8)Weaver.CreateInstance(typeof(GenericInterfaceMethodPrivate), null, asp);
            i.Test(42);
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
            callorder = 1;
            ((ClassBase)i).testCall = 0;
            asp.aspectTestBefore = 0;
            i.Test("TesT");
        }

	}
	[TestClass]
	public class BeforeMultibleIFWeaving:InterfaceWeavingTestBase
	{
		[TestMethod]
		public void PublicInterface()
		{
			callorder=1;
			TestAspectBase asp=new BeforeAspect();
			IF2 i=(IF2)Weaver.CreateInstance(typeof(AMultibleFunctionsPublic),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			asp.aspectTestBefore=0;
			callorder=1;
			((ClassBase)i).testCall=0;
			((IF1)i).Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			((ClassBase)i).testCall=0;
			callorder=1;
			i.Second();
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
		[TestMethod]
		public void PrivateInterface()
		{
			callorder=1;
			TestAspectBase asp=new BeforeAspect();
			IF2 i=(IF2)Weaver.CreateInstance(typeof(AMultibleFunctionsPrivate),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			asp.aspectTestBefore=0;
			callorder=1;
			((ClassBase)i).testCall=0;
			((IF1)i).Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			((ClassBase)i).testCall=0;
			callorder=1;
			i.Second();
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
		[TestMethod]
		public void PartialWeaving()
		{
			callorder=1;
			TestAspectBase asp=new BeforeAspect();
			IF3 i=(IF3)Weaver.CreateInstance(typeof(APartial),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			asp.aspectTestBefore=0;
			callorder=1;
			((ClassBase)i).testCall=0;
			i.Test(new object());
			Assert.AreEqual(0,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
			((ClassBase)i).testCall=0;
			callorder=1;
			i.Test(1);
			Assert.AreEqual(0,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
	}

	[TestClass]
	public class InsteadMultibleIFWeaving:InterfaceWeavingTestBase
	{
		[TestMethod]
		public void StdInterface()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspect();
			IF2 i=(IF2)Weaver.CreateInstance(typeof(AMultibleFunctionsPublic),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			asp.aspectTestBefore=0;
			callorder=1;
			((ClassBase)i).testCall=0;
			((IF1)i).Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			((ClassBase)i).testCall=0;
			callorder=1;
			i.Second();
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
		[TestMethod]
		public void PrivateInterface()
		{
			callorder=1;
			TestAspectBase asp=new InsteadAspect();
			IF2 i=(IF2)Weaver.CreateInstance(typeof(AMultibleFunctionsPrivate),null,asp);
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			asp.aspectTestBefore=0;
			callorder=1;
			((ClassBase)i).testCall=0;
			((IF1)i).Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(2,((ClassBase)i).testCall,"invalid calling sequence");
			((ClassBase)i).testCall=0;
			callorder=1;
			i.Second();
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
	}
	[TestClass]
	public class Properties:InterfaceWeavingTestBase
	{
		[TestMethod]
		public void PublicProp()
		{
			callorder=1;
			TestAspectBase asp=new PropertyAspect();
            IF4 i = Weaver.Create<AProperty>(asp) as IF4;
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
			asp.aspectTestBefore=0;
			double td=i.Double;
			Assert.AreEqual(0,asp.aspectTestBefore,"invalid calling sequence");
			Assert.AreEqual(1,((ClassBase)i).testCall,"invalid calling sequence");
		}
	}
	[TestClass]
	public class IntroduceIFWeaving:InterfaceWeavingTestBase
	{
        [TestMethod]
		public void IntroduceSimpleIF()
		{
			callorder=1;
			
            IF1 i = Weaver.Create<ANoInterface>() as IF1;
			Assert.IsNotNull(i,"introduction expected");
            TestAspectBase asp = ((IAspectInfo)i).GetAspects(typeof(IntroduceAspect))[0] as IntroduceAspect;
			i.Test();
			Assert.AreEqual(1,asp.aspectTestBefore,"invalid calling sequence");
		}

        [TestMethod]
        public void IntroduceExistingIF()
        {
            callorder = 1;

            IF1 i = Weaver.Create<AWithInterface>() as IF1;
            Assert.IsNotNull(i, "introduction expected");
            TestAspectBase asp = ((IAspectInfo)i).GetAspects(typeof(IntroduceAspect))[0] as IntroduceAspect;
            i.Test();
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
            Assert.AreEqual(3, callorder, "invalid calling sequence");
        }

		[TestMethod]
		// es wird ein Fehler erwartet, das private Interfaces nicht verwebbar sind
		[ExpectedException(typeof(AspectWeaverException))]
		public void IntroducePrivateInterface()
		{
			callorder = 1;
			TestAspect a = new TestAspect();
            TestClass o = Weaver.Create<TestClass>(a);
		}

        [TestMethod]
        public void IntroduceSimpleIFOnMethod()
        {
            callorder = 1;

            IF1 i = Weaver.Create<ANoInterfaceOnMethod>() as IF1;
            Assert.IsNotNull(i, "introduction expected");
            TestAspectBase asp = ((IAspectInfo)i).GetAspects(typeof(IntroduceAspectOnMethod))[0] as IntroduceAspectOnMethod;
            i.Test();
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
        }

        [TestMethod]
        public void IntroduceExistingIFOnMethod()
        {
            callorder = 1;

            IF1 i = Weaver.Create<AInterfaceOnMethod>() as IF1;
            Assert.IsNotNull(i, "introduction expected");
            TestAspectBase asp = ((IAspectInfo)i).GetAspects(typeof(IntroduceAspectOnMethod))[0] as IntroduceAspectOnMethod;
            i.Test();
            Assert.AreEqual(1, asp.aspectTestBefore, "invalid calling sequence");
            Assert.AreEqual(2, ((ClassBase)i).testCall, "invalid calling sequence");
            Assert.AreEqual(3, callorder, "invalid calling sequence");
        }
      
	}

    
	/// <summary>Tests for interweaving class that inherit from 2 interfaces with the same method.</summary>
	[TestClass]
	public class SameInterfaceWeaving:InterfaceWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void IncTest()
		{
			SameInterfaceAspect a = new SameInterfaceAspect();
			S s=(S)Weaver.CreateInstance(typeof(S),null,a);
			I1 i1=(I1)s;
			i1.Inc(1);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,s.testCall,"object not called");
			I2 i2=(I2)s;
			a.aspectTestBefore=0;
			a.aspectTestInstead=0;
			s.testCall=0;
			callorder=1;
			i2.Inc(1);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,s.testCall,"object not called");
		}
		[TestMethod]
		public virtual void VirtualIncTest()
		{
			SameInterfaceAspect a = new SameInterfaceAspect();
			VS vs=(VS)Weaver.CreateInstance(typeof(VS),null,a);
			I1 i1 = (I1)vs;
			i1.Inc(1);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect nit jpinterwoven");
			Assert.AreEqual(3,vs.testCall,"object not jpinterwoven");
			I2 i2 = (I2)vs;
			callorder = 1;
			a.aspectTestBefore = 0;
			a.aspectTestInstead = 0;
			vs.testCall = 0;
			i2.Inc(1);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect nit jpinterwoven");
			Assert.AreEqual(3,vs.testCall,"object not jpinterwoven");
		}
	}
	/// <summary>Tests for interweaving Aspect that exclude interface from pitarget class.</summary>
	[TestClass]
	public class InterweavingClassButExcludeItsInterface:InterfaceWeavingTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void Test()
		{
			AspectExcludeInterface a = new AspectExcludeInterface();

            VS1 t = Weaver.Create<VS1>(a);
			t.Inc(1);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,t.testCall,"object not jpinterwoven");
			callorder = 1;
			a.aspectTestBefore = 0;
			a.aspectTestInstead = 0;
			t.testCall = 0;
			((I1)t).Inc(1);
			Assert.AreEqual(0,a.aspectTestBefore,"Aspect jpinterwoven");
			Assert.AreEqual(0,a.aspectTestInstead,"Aspect jpinterwoven");
			Assert.AreEqual(1,t.testCall,"object not called");
		}
	}

    [TestClass]
    public class AnnotatedInterfaceWeaving : InterfaceWeavingTestBase
    {
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
        [TestMethod]
        public void WholeInterface()
        {
            IF5 ifa = Weaver.Create<InterfaceAnnotated>();
            ifa.Foo();
            Assert.IsTrue(ifa is IAspectInfo);
            Assert.IsTrue(ifa is InterfaceAnnotated); 
            Aspect[] aspects = ((IAspectInfo)ifa).GetAspects();
            Assert.IsNotNull(aspects);
            Assert.AreEqual(1, aspects.Length);
            Assert.IsInstanceOfType(aspects[0],typeof(InterfaceAnnotationAspect));
            Assert.AreEqual(1, ((InterfaceAnnotationAspect)aspects[0]).aspectTestBefore);
            Assert.AreEqual(2, ((InterfaceAnnotated)ifa).testCall);
            Assert.AreEqual(3, callorder);

            callorder = 1;
            ((InterfaceAnnotationAspect)aspects[0]).aspectTestBefore = 0;
            ((InterfaceAnnotated)ifa).testCall = 0;

            ifa.Bar();

            Assert.AreEqual(1, ((InterfaceAnnotationAspect)aspects[0]).aspectTestBefore);
            Assert.AreEqual(2, ((InterfaceAnnotated)ifa).testCall);
            Assert.AreEqual(3, callorder);

            callorder = 1;
            ((InterfaceAnnotationAspect)aspects[0]).aspectTestBefore = 0;
            ((InterfaceAnnotated)ifa).testCall = 0;

            ((InterfaceAnnotated)ifa).Baz();

            Assert.AreEqual(1, ((InterfaceAnnotated)ifa).testCall);
            Assert.AreEqual(2, callorder);

        }
        [TestMethod]
        public void Method()
        {
            IF6 ifa = Weaver.Create<InterfaceMethodAnnotated>();
            ifa.Foo();
            Assert.IsTrue(ifa is IAspectInfo);
            Assert.IsTrue(ifa is InterfaceMethodAnnotated);
            Aspect[] aspects = ((IAspectInfo)ifa).GetAspects();
            Assert.IsNotNull(aspects);
            Assert.AreEqual(1, aspects.Length);
            Assert.IsInstanceOfType(aspects[0],typeof(MethodAnnotationAspect));
            Assert.AreEqual(1, ((MethodAnnotationAspect)aspects[0]).aspectTestBefore);
            Assert.AreEqual(2, ((InterfaceMethodAnnotated)ifa).testCall);
            Assert.AreEqual(3, callorder);

            callorder = 1;
            ((MethodAnnotationAspect)aspects[0]).aspectTestBefore = 0;
            ((InterfaceMethodAnnotated)ifa).testCall = 0;

            ifa.Bar();

            Assert.AreEqual(1, ((InterfaceMethodAnnotated)ifa).testCall);
            Assert.AreEqual(2, callorder);
        }
    }
}