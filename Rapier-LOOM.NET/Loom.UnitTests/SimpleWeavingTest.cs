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


namespace Loom.UnitTests.SimpleWeaving
{
    /// <summary>SimpleWeavingTestBase contains all aspects for testing weaving with base types.</summary>
    public class SimpleWeavingTestBase:TestBase
    {
        public class NothingAspect:TestAspectBase
		{
            [Call(Advice.Before)]
            [Include("VeryLongNohingToMatchName")]
            public void Test<T>(params object[] args)
            {
                Assert.Fail("invalid call");
            }

            [Call(Advice.Before)]
            public void VeryLongNohingToMatchName(params object[] args)
            {
                Assert.Fail("invalid call");
            }

            [Call(Advice.Before)]
			public void VeryLongNohingToMatchName()
			{
				Assert.Fail("invalid call");
			}
		}

        public class BeforeAspect : TestAspectBase
        {
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestA()
            {
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestA()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestB(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestB()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestC(C c)
            {
                Assert.IsNotNull(c, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestC()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestD(ref D d)
            {
                Assert.IsNotNull(d, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestD()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestE(V v)
            {
                Assert.AreEqual(1, v.i, "Invalid parameter value.");
                Assert.AreEqual(2.0, v.d, "Invalid parameter value.");
                Assert.AreEqual("Test", v.s, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestE()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestF(ref double f)
            {
                Assert.AreEqual(2.0, f, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestF()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestG(ref V v)
            {
                Assert.AreEqual(1, v.i, "Invalid parameter value.");
                Assert.AreEqual(2.0, v.d, "Invalid parameter value.");
                Assert.AreEqual("Test", v.s, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestF()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestG(Numbers n)
            {
                Assert.AreEqual(Numbers.One, n, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "BeforeAspect.TestH()");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestByVal(System.Boolean b1, System.SByte i8, System.Byte u8, System.Int16 i16, System.UInt16 u16, System.Int32 i32, System.UInt32 u32, System.Int64 i64, System.UInt64 u64, System.IntPtr ip, System.UIntPtr up, System.Decimal dec, System.Single r4, System.Double r8)
            {
                Assert.AreEqual(true, b1, "invalid parameter");
                Assert.AreEqual(i8, -1, "invalid parameter");
                Assert.AreEqual(u8, 1, "invalid parameter");
                Assert.AreEqual(i16, -1, "invalid parameter");
                Assert.AreEqual(u16, 1, "invalid parameter");
                Assert.AreEqual(i32, -1, "invalid parameter");
                Assert.AreEqual(u32, 1, "invalid parameter");
                Assert.AreEqual(i64, -1, "invalid parameter");
                Assert.AreEqual(u64, 1, "invalid parameter");
                Assert.AreEqual(ip, -1, "invalid parameter");
                Assert.AreEqual(up, 1, "invalid parameter");
                Assert.AreEqual(dec, 1.0, "invalid parameter");
                Assert.AreEqual(r4, -1.0, "invalid parameter");
                Assert.AreEqual(r8, 1.0, "invalid parameter");
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestByRef(ref System.Boolean b1, ref System.SByte i8, ref System.Byte u8, ref System.Int16 i16, ref System.UInt16 u16, ref System.Int32 i32, ref System.UInt32 u32, ref System.Int64 i64, ref System.UInt64 u64, ref System.IntPtr ip, ref System.UIntPtr up, ref System.Decimal dec, ref System.Single r4, ref System.Double r8)
            {
                Assert.AreEqual(true, b1, "invalid parameter");
                Assert.AreEqual(i8, -1, "invalid parameter");
                Assert.AreEqual(u8, 1, "invalid parameter");
                Assert.AreEqual(i16, -1, "invalid parameter");
                Assert.AreEqual(u16, 1, "invalid parameter");
                Assert.AreEqual(i32, -1, "invalid parameter");
                Assert.AreEqual(u32, 1, "invalid parameter");
                Assert.AreEqual(i64, -1, "invalid parameter");
                Assert.AreEqual(u64, 1, "invalid parameter");
                Assert.AreEqual(ip, -1, "invalid parameter");
                Assert.AreEqual(up, 1, "invalid parameter");
                Assert.AreEqual(dec, 1.0, "invalid parameter");
                Assert.AreEqual(r4, -1.0, "invalid parameter");
                Assert.AreEqual(r8, 1.0, "invalid parameter");
            }
        }

        public class AfterAspect : TestAspectBase
        {
            [Call(Advice.After)]
            [Include("Test")]
            public void TestA()
            {
                RegisterCall(ref aspectTestAfter, "AfterAspect.TestA()");
            }
            [Call(Advice.After)]
            [Include("Test")]
            public void TestB(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfter, "AfterAspect.TestB()");
            }
            [Call(Advice.After)]
            [Include("Test")]
            public void TestC(C c)
            {
                Assert.IsNotNull(c, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfter, "AfterAspect.TestC()");
            }
            [Call(Advice.After)]
            [Include("Test")]
            public void TestD(ref D d)
            {
                Assert.IsNotNull(d, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfter, "AfterAspect.TestD()");
            }
            [Call(Advice.After)]
            [Include("Test")]
            public void TestE(V v)
            {
                Assert.AreEqual(1, v.i, "Invalid parameter value.");
                Assert.AreEqual(2.0, v.d, "Invalid parameter value.");
                Assert.AreEqual("Test", v.s, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfter, "AfterAspect.TestE()");
            }
        }

        public class AfterReturningAspect : TestAspectBase
        {
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestA()
            {
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.TestA()");
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestB([JPRetVal] int retval, int i)
            {
                Assert.AreEqual(retval, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.TestB()");
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestC([JPRetVal] C retval, C c)
            {
                Assert.AreEqual(retval, c, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.TestC()");
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestD([JPRetVal] D retval, ref D d)
            {
                Assert.AreEqual(retval, d, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.TestD()");
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestE([JPRetVal] V retval, V v)
            {
                Assert.AreEqual(retval, v, "Invalid parameter value.");
                Assert.AreEqual(1, v.i, "Invalid parameter value.");
                Assert.AreEqual(2.0, v.d, "Invalid parameter value.");
                Assert.AreEqual("Test", v.s, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterReturning, "AfterReturningAspect.TestE()");
            }
        }

        public class AfterThrowingAspect : TestAspectBase
        {
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestA<T>([JPException] Exception e)
            {
                Assert.IsNotNull(e as TestException, "Invalid exception value.");
                RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingAspect.TestA()");
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestB<T>([JPException] Exception e, int i)
            {
                Assert.IsNotNull(e as TestException, "Invalid exception value.");
                Assert.AreEqual(23, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingAspect.TestB()");
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestC<T>([JPException] Exception e, C c)
            {
                Assert.IsNotNull(e as TestException, "Invalid exception value.");
                Assert.IsNotNull(c, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingAspect.TestC()");
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestD<T>([JPException] Exception e, ref D d)
            {
                Assert.IsNotNull(e as TestException, "Invalid exception value.");
                Assert.IsNotNull(d, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingAspect.TestD()");
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestE<T>([JPException] Exception e, V v)
            {
                Assert.IsNotNull(e as TestException, "Invalid exception value.");
                Assert.AreEqual(1, v.i, "Invalid parameter value.");
                Assert.AreEqual(2.0, v.d, "Invalid parameter value.");
                Assert.AreEqual("Test", v.s, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfterThrowing, "AfterThrowingAspect.TesE()");
                return default(T);
            }
        }

        public class MultiMatchAspectInt : TestAspectBase
        {
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestBBefore(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestBefore, "MultiMatchAspectInt.TestBBefore()");
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestBReturning([JPRetVal] int retval, int i)
            {
                Assert.AreEqual(i, retval, "invalid result");
                RegisterCall(ref aspectTestAfterReturning, "MultiMatchAspectInt.TestBReturning()");
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestBAfterThrowing<T>([JPException] Exception e, T t)
            {
                Assert.AreEqual(23, t, "Invalid parameter value.");
                Assert.IsNotNull(e as TestException, "invalid exception");
                RegisterCall(ref aspectTestAfterThrowing, "MultiMatchAspectInt.TestBAfterThrowing()");
                return default(T);
            }
            [Call(Advice.After)]
            [Include("Test")]
            public void TestBAfter(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                RegisterCall(ref aspectTestAfter, "MultiMatchAspectInt.TestBAfter()");
            }
        }

        public class MultiMatchAspectBefore : TestAspectBase
        {
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestB1(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestB2(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestB3(int i)
            {
                Assert.AreEqual(23, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestC1(C c)
            {
                Assert.IsNotNull(c, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestC2(C c)
            {
                Assert.IsNotNull(c, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.Before)]
            [Include("Test")]
            public void TestC3(C c)
            {
                Assert.IsNotNull(c, "Invalid parameter value.");
                callorder++;
            }
        }

        public class MultiMatchAspectAfterReturning : TestAspectBase
        {
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestB1([JPRetVal] int retval, int i)
            {
                Assert.AreEqual(retval, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestB2([JPRetVal] int retval, int i)
            {
                Assert.AreEqual(retval, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestB3([JPRetVal] int retval, int i)
            {
                Assert.AreEqual(retval, i, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestC1([JPRetVal] C retval, C c)
            {
                Assert.AreEqual(retval, c, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestC2([JPRetVal] C retval, C c)
            {
                Assert.AreEqual(retval, c, "Invalid parameter value.");
                callorder++;
            }
            [Call(Advice.AfterReturning)]
            [Include("Test")]
            public void TestC3([JPRetVal] C retval, C c)
            {
                Assert.AreEqual(retval, c, "Invalid parameter value.");
                callorder++;
            }
        }

        public class  MultiMatchAspectAfterThrowingPrimitive : TestAspectBase
        {
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestB1<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.AreEqual(23, t, "Invalid parameter value.");                         
                callorder++;
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestB2<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.AreEqual(23, t, "Invalid parameter value.");
                callorder++;
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestB3<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.AreEqual(23, t, "Invalid parameter value.");
                callorder++;
                return default(T);
            }
        }

        public class MultiMatchAspectAfterThrowingClass : TestAspectBase
        {
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestC1<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.IsNotNull(t, "Invalid parameter value.");
                callorder++;
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestC2<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.IsNotNull(t, "Invalid parameter value.");
                callorder++;
                return default(T);
            }
            [Call(Advice.AfterThrowing)]
            [Include("Test")]
            public T TestC3<T>([JPException] Exception e, T t)
            {
                Assert.IsNotNull(e as TestException, "invalid exception");
                Assert.IsNotNull(t, "Invalid parameter value.");
                callorder++;
                return default(T);
            }
        }

		public class AspectTestDouble:TestAspectBase
		{
			private object expected;
			public AspectTestDouble(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestDouble")]
			public void TestBefore(System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestDouble")]
			public void TestAfter(System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestDouble")]
            public void TestAfterReturning([JPRetVal] System.Double retval, System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Double)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestDouble")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Double)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestSingle:TestAspectBase
		{
			private object expected;
			public AspectTestSingle(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestSingle")]
			public void TestBefore(System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestSingle")]
			public void TestAfter(System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestSingle")]
            public void  TestAfterReturning([JPRetVal] System.Single retval, System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Single)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestSingle")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Single)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestDecimal:TestAspectBase
		{
			private object expected;
			public AspectTestDecimal(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestDecimal")]
			public void TestBefore(System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestDecimal")]
			public void TestAfter(System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestDecimal")]
            public void TestAfterReturning([JPRetVal] System.Decimal retval, System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Decimal)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestDecimal")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Decimal)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestUIntPtr:TestAspectBase
		{
			private object expected;
			public AspectTestUIntPtr(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUIntPtr")]
			public void TestBefore(System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUIntPtr")]
			public void TestAfter(System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUIntPtr")]
            public void TestAfterReturning([JPRetVal] System.UIntPtr retval, System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UIntPtr)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestUIntPtr")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestIntPtr:TestAspectBase
		{
			private object expected;
			public AspectTestIntPtr(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestIntPtr")]
			public void TestBefore(System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestIntPtr")]
			public void TestAfter(System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestIntPtr")]
            public void TestAfterReturning([JPRetVal] System.IntPtr retval, System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.IntPtr)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestIntPtr")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.IntPtr)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestUInt64:TestAspectBase
		{
			private object expected;
			public AspectTestUInt64(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt64")]
			public void TestBefore(System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt64")]
			public void TestAfter(System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt64")]
            public void TestAfterReturning([JPRetVal] System.UInt64 retval, System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt64)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestUInt64")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.UInt64)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestInt64:TestAspectBase
		{
			private object expected;
			public AspectTestInt64(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt64")]
			public void TestBefore(System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt64")]
			public void TestAfter(System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt64")]
            public void TestAfterReturning([JPRetVal] System.Int64 retval, System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int64)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestInt64")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Int64)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestUInt32:TestAspectBase
		{
			private object expected;
			public AspectTestUInt32(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt32")]
			public void TestBefore(System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt32")]
			public void TestAfter(System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt32")]
            public void TestAfterReturning([JPRetVal] System.UInt32 retval, System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt32)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestUInt32")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.UInt32)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestInt32:TestAspectBase
		{
			private object expected;
			public AspectTestInt32(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt32")]
			public void TestBefore(System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt32")]
			public void TestAfter(System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt32")]
            public void TestAfterReturning([JPRetVal] System.Int32 retval, System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int32)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestInt32")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Int32)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestUInt16:TestAspectBase
		{
			private object expected;
			public AspectTestUInt16(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt16")]
			public void TestBefore(System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt16")]
			public void TestAfter(System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt16")]
            public void TestAfterReturning([JPRetVal] System.UInt16 retval, System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt16)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestUInt16")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.UInt16)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestInt16:TestAspectBase
		{
			private object expected;
			public AspectTestInt16(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt16")]
			public void TestBefore(System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt16")]
			public void TestAfter(System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt16")]
            public void TestAfterReturning([JPRetVal] System.Int16 retval, System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int16)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestInt16")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Int16)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestByte:TestAspectBase
		{
			private object expected;
			public AspectTestByte(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestByte")]
			public void TestBefore(System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestByte")]
			public void TestAfter(System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestByte")]
            public void TestAfterReturning([JPRetVal] System.Byte retval, System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Byte)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestByte")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.Byte)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestSByte:TestAspectBase
		{
			private object expected;
			public AspectTestSByte(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestSByte")]
			public void TestBefore(System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestSByte")]
			public void TestAfter(System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestSByte")]
            public void TestAfterReturning([JPRetVal] System.SByte retval, System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.SByte)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestSByte")]
            public T TestAfterThrowing<T>([JPException] Exception e, T t)
			{
				Assert.AreEqual( (System.SByte)(expected), t,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return t;
			}
		}

		public class AspectTestBool:TestAspectBase
		{
			private object expected;
			public AspectTestBool(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestBool")]
			public void TestBefore(System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestBool")]
			public void TestAfter(System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestBool")]
            public void TestAfterReturning([JPRetVal] System.Boolean retval, System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Boolean)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)]
            public System.Boolean TestBool([JPException] Exception e, System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestChar:TestAspectBase
		{
			private object expected;
			public AspectTestChar(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestChar")]
			public void TestBefore(System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestChar.TestBefore()");
			}
			[Call(Advice.Around)] [Include("TestChar")]
			public T TestInstead<T>([JPContext] Context Context, System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestInstead,"AspectTestChar.TestInstead()");
				object[] args = new object[1];
				args[0] = c;
				return (T)Context.Invoke(args);
			}
			[Call(Advice.After)] [Include("TestChar")]
			public void TestAfter(System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestChar.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestChar")]
            public void TestAfterReturning([JPRetVal] System.Char returnvalie, System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestChar.AfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestChar")]
            public T TestAfterThrowing<T>([JPException] Exception e, System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestChar.TestAfterThrowing()");
				return (T)(object)c;
			}
		}
		public class AspectTestString:TestAspectBase
		{
			private object expected;
			public AspectTestString(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestString")]
			public void TestBefore(System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestString.TestBefore()");
			}
			[Call(Advice.Around)] [Include("TestString")]
			public T TestInstead<T>([JPContext] Context Context, System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestInstead,"AspectTestString.TestInstead()");
				object[] args = new object[1];
				args[0] = s;
				return (T)Context.Invoke(args);
			}
			[Call(Advice.After)] [Include("TestString")]
			public void TestAfter(System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestString.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestString")]
            public void TestAfterReturning([JPRetVal] System.String returnvalue, System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestString.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestString")]
            public T TestAfterThrowing<T>([JPException] Exception e, System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestString.TestAfterThrowing()");
				return (T)(object)s;
			}
		}
		public class AspectTestObject:TestAspectBase
		{
			private object expected;
			public AspectTestObject(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestObject")]
			public void TestBefore(System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestObject.TestBefore()");
			}
            [Call(Advice.Around)]
            [Include("TestObject")]
            public T TestInstead<T>([JPContext] Context Context, System.Object o)
            {
                Assert.AreEqual((System.Object)expected, o, "invalid parameter");
                RegisterCall(ref aspectTestInstead, "AspectTestObject.TestInstead()");
                object[] args = new object[1];
                args[0] = o;
                return (T)Context.Invoke(args);
            }
			[Call(Advice.After)] [Include("TestObject")]
			public void TestAfter(System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestObject.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestObject")]
            public void TestAfterReturning([JPRetVal] System.Object returnvalue, System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestObject.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestObject")]
            public T TestAfterThrowing<T>([JPException] Exception e, System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestObject.TestAfterThrowing()");
				return (T)o;
			}
		}
		public class AspectTestObjectRef:TestAspectBase
		{
			private object expected;
			public AspectTestObjectRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestObjectRef")]
			public void TestBefore(ref System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestObjectRef.TestBefore()");
			}
			[Call(Advice.Around)] [Include("TestObjectRef")]
			public T TestInstead<T>([JPContext] Context Context, ref System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestInstead,"AspectTestObjectRef.TestInstead()");
				object[] args = new object[1];
				args[0] = o;
				return (T)Context.Invoke(args);
			}
			[Call(Advice.After)] [Include("TestObjectRef")]
			public void TestAfter(ref System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestObjectRef.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestObjectRef")]
            public void TestAfterReturning([JPRetVal] System.Object returnvalue, ref System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestObjectRef.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestObjectRef")]
            public T TestAfterThrowing<T>([JPException] Exception e, ref System.Object o)
			{
				Assert.AreEqual((System.Object)expected,o,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestObjectRef.TestAfterThrowing()");
				return (T)o;
			}
		}
		public class AspectTestStringArray:TestAspectBase
		{
			private object expected;
			public AspectTestStringArray(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestStringArray")]
			public void TestBefore(System.String[] s)
			{
				Assert.AreEqual((System.String[])expected,s,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestStringArray.TestBefore()");
			}
			[Call(Advice.Around)] [Include("TestStringArray")]
			public T TestInstead<T>([JPContext] Context Context, System.String[] s)
			{
				Assert.AreEqual((System.String[])expected,s,"invalid parameter");
				RegisterCall(ref aspectTestInstead,"AspectTestStringArray.TestInstead()");
//				object[] args = new object[1];
//				args[0] = s;
//				object o = Context.Invoke(s);
				return (T)Context.Call(s);
				
			}
			[Call(Advice.After)] [Include("TestStringArray")]
			public void TestAfter(System.String[] s)
			{
				Assert.AreEqual((System.String[])expected,s,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestStringArray.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestStringArray")]
            public void TestAfterReturning([JPRetVal] System.String[] returnvalue, System.String[] s)
			{
				Assert.AreEqual((System.String[])expected,s,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestStringArray.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestStringArray")]
            public T TestAfterThrowing<T>([JPException] Exception e, System.String[] s)
			{
				Assert.AreEqual((System.String[])expected,s,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestStringArray.TestAfterThrowing()");
				return (T)(object)s;
			}
		}
		public class AspectTestStringRef:TestAspectBase
		{
			private object expected;
			public AspectTestStringRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestStringRef")]
			public void TestBefore(ref System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestStringRef.TestBefore()");
			}
            [Call(Advice.AfterThrowing)]
            [Include("TestStringRef")]
            public T TestAround<T>([JPContext] Context ctx, ref System.String s)
            {
                Assert.AreEqual((System.String)expected, s, "invalid parameter");
                RegisterCall(ref aspectTestAfterReturning, "AspectTestStringRef.TestAfterReturning()");
                object[] args=new object[]{s};
                try
                {
                    return (T)ctx.Invoke(args);
                }
                finally
                {
                    s=(string)args[0];
                }
            }
            [Call(Advice.After)]
            [Include("TestStringRef")]
			public void TestAfter(ref System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestStringRef.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestStringRef")]
            public void TestAfterReturning<T>([JPRetVal] System.String returnvalue, ref T t)
			{
				Assert.AreEqual((System.String)expected, t,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestStringRef.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestStringRef")]
            public T TestAfterThrowing<T>([JPException] Exception e, ref System.String s)
			{
				Assert.AreEqual((System.String)expected, s,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestStringRef.TestAfterReturning()");
                return (T)(object)s;
			}
		}
		public class AspectTestCharRef:TestAspectBase
		{
			private object expected;
			public AspectTestCharRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestCharRef")]
			public void TestBefore(ref System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AspectTestCharRef.TestBefore()");
			}
			[Call(Advice.After)] [Include("TestCharRef")]
			public void TestAfter(ref System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AspectTestCharRef.TestAfter()");
			}
			[Call(Advice.AfterReturning)] [Include("TestCharRef")]
            public void TestAfterReturning([JPRetVal] System.Char returnvalue, ref System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AspectTestCharRef.TestAfterReturning()");
			}
			[Call(Advice.AfterThrowing)] [Include("TestCharRef")]
            public T TestAfterThrowing<T>([JPException] Exception e, ref System.Char c)
			{
				Assert.AreEqual((System.Char)expected, c,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AspectTestCharRef.TestAfterThrowing()");
				return (T)(object)c;
			}
		}
		public class AspectTestDoubleRef:TestAspectBase
		{
			private object expected;
			public AspectTestDoubleRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestDoubleRef")]
			public void TestBefore(ref System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestDoubleRef")]
			public void TestAfter(ref System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestDoubleRef")]
            public void TestAfterReturning([JPRetVal] System.Double retval, ref System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Double)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)]
            public System.Double TestDoubleRef([JPException] Exception e, ref System.Double p)
			{
				Assert.AreEqual( (System.Double)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestSingleRef:TestAspectBase
		{
			private object expected;
			public AspectTestSingleRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestSingleRef")]
			public void TestBefore(ref System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestSingleRef")]
			public void TestAfter(ref System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestSingleRef")]
            public void TestAfterReturning([JPRetVal] System.Single retval, ref System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Single)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Single TestSingleRef([JPException] Exception e, ref System.Single p)
			{
				Assert.AreEqual( (System.Single)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestDecimalRef:TestAspectBase
		{
			private object expected;
			public AspectTestDecimalRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestDecimalRef")]
			public void TestBefore(ref System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestDecimalRef")]
			public void TestAfter(ref System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestDecimalRef")]
            public void TestAfterReturning([JPRetVal] System.Decimal retval, ref System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Decimal)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Decimal TestDecimalRef([JPException] Exception e, ref System.Decimal p)
			{
				Assert.AreEqual( (System.Decimal)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestUIntPtrRef:TestAspectBase
		{
			private object expected;
			public AspectTestUIntPtrRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUIntPtrRef")]
			public void TestBefore(ref System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUIntPtrRef")]
			public void TestAfter(ref System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUIntPtrRef")]
            public void TestAfterReturning([JPRetVal] System.UIntPtr retval, ref System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UIntPtr)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.UIntPtr TestUIntPtrRef([JPException] Exception e, ref System.UIntPtr p)
			{
				Assert.AreEqual( (System.UIntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestIntPtrRef:TestAspectBase
		{
			private object expected;
			public AspectTestIntPtrRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestIntPtrRef")]
			public void TestBefore(ref System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestIntPtrRef")]
			public void TestAfter(ref System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestIntPtrRef")]
            public void TestAfterReturning([JPRetVal] System.IntPtr retval, ref System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.IntPtr)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.IntPtr TestIntPtrRef([JPException] Exception e, ref System.IntPtr p)
			{
				Assert.AreEqual( (System.IntPtr)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestUInt64Ref:TestAspectBase
		{
			private object expected;
			public AspectTestUInt64Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt64Ref")]
			public void TestBefore(ref System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt64Ref")]
			public void TestAfter(ref System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt64Ref")]
            public void TestAfterReturning([JPRetVal] System.UInt64 retval, ref System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt64)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)]
            public System.UInt64 TestUInt64Ref([JPException] Exception e, ref System.UInt64 p)
			{
				Assert.AreEqual( (System.UInt64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestInt64Ref:TestAspectBase
		{
			private object expected;
			public AspectTestInt64Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt64Ref")]
			public void TestBefore(ref System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt64Ref")]
			public void TestAfter(ref System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt64Ref")]
            public void TestAfterReturning([JPRetVal] System.Int64 retval, ref System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int64)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Int64 TestInt64Ref([JPException] Exception e, ref System.Int64 p)
			{
				Assert.AreEqual( (System.Int64)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestUInt32Ref:TestAspectBase
		{
			private object expected;
			public AspectTestUInt32Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt32Ref")]
			public void TestBefore(ref System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt32Ref")]
			public void TestAfter(ref System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt32Ref")]
            public void TestAfterReturning([JPRetVal] System.UInt32 retval, ref System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt32)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.UInt32 TestUInt32Ref([JPException] Exception e, ref System.UInt32 p)
			{
				Assert.AreEqual( (System.UInt32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestInt32Ref:TestAspectBase
		{
			private object expected;
			public AspectTestInt32Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt32Ref")]
			public void TestBefore(ref System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt32Ref")]
			public void TestAfter(ref System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt32Ref")]
            public void TestAfterReturning([JPRetVal] System.Int32 retval, ref System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int32)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Int32 TestInt32Ref([JPException] Exception e, ref System.Int32 p)
			{
				Assert.AreEqual( (System.Int32)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestUInt16Ref:TestAspectBase
		{
			private object expected;
			public AspectTestUInt16Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestUInt16Ref")]
			public void TestBefore(ref System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestUInt16Ref")]
			public void TestAfter(ref System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestUInt16Ref")]
            public void TestAfterReturning([JPRetVal] System.UInt16 retval, ref System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.UInt16)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)]
            public System.UInt16 TestUInt16Ref([JPException] Exception e, ref System.UInt16 p)
			{
				Assert.AreEqual( (System.UInt16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestInt16Ref:TestAspectBase
		{
			private object expected;
			public AspectTestInt16Ref(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestInt16Ref")]
			public void TestBefore(ref System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestInt16Ref")]
			public void TestAfter(ref System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestInt16Ref")]
            public void TestAfterReturning([JPRetVal] System.Int16 retval, ref System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Int16)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)]
            public System.Int16 TestInt16Ref([JPException] Exception e, ref System.Int16 p)
			{
				Assert.AreEqual( (System.Int16)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestByteRef:TestAspectBase
		{
			private object expected;
			public AspectTestByteRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestByteRef")]
			public void TestBefore(ref System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestByteRef")]
			public void TestAfter(ref System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestByteRef")]
            public void TestAfterReturning([JPRetVal] System.Byte retval, ref System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Byte)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Byte TestByteRef([JPException] Exception e, ref System.Byte p)
			{
				Assert.AreEqual( (System.Byte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestSByteRef:TestAspectBase
		{
			private object expected;
			public AspectTestSByteRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestSByteRef")]
			public void TestBefore(ref System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestSByteRef")]
			public void TestAfter(ref System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestSByteRef")]
            public void TestAfterReturning([JPRetVal] System.SByte retval, ref System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.SByte)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.SByte TestSByteRef([JPException] Exception e, ref System.SByte p)
			{
				Assert.AreEqual( (System.SByte)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}

		public class AspectTestBoolRef:TestAspectBase
		{
			private object expected;
			public AspectTestBoolRef(object expected)
			{
				this.expected=expected;
			}
			[Call(Advice.Before)] [Include("TestBoolRef")]
			public void TestBefore(ref System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestBefore,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.After)] [Include("TestBoolRef")]
			public void TestAfter(ref System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfter,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterReturning)] [Include("TestBoolRef")]
            public void TestAfterReturning([JPRetVal] System.Boolean retval, ref System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				Assert.AreEqual( retval, (System.Boolean)(expected),"invalid parameter");
				RegisterCall(ref aspectTestAfterReturning,"AfterThrowingAspect.Test()");
			}
			[Call(Advice.AfterThrowing)] 
            public System.Boolean TestBoolRef([JPException] Exception e, ref System.Boolean p)
			{
				Assert.AreEqual( (System.Boolean)(expected), p,"invalid parameter");
				RegisterCall(ref aspectTestAfterThrowing,"AfterThrowingAspect.Test()");
				return p;
			}
		}



		public class BeforeAspectForOutInt:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(out int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutInt.f");
				i=0;
			}
		}
		public class BeforeAndWildcardAspectForOutInt:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
            public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForOutInt.f");
			}
		}
		public class InsteadAspectForOutInt:TestAspectBase
		{
			[IncludeAll] [IncludeDeclaredOnly]
			[Call(Advice.Around)]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspectForOutInt.f");
				return (T)Context.Invoke(args);
			}
		}
		public class BeforeAspectForRefInt:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(ref int i)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutInt.f");
			}
		}
		public class BeforeAndWildcardAspectForRefInt:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForRefInt.f");
			}
		}
		public class InsteadAspectForRefInt:TestAspectBase
		{
			[IncludeAll] [IncludeDeclaredOnly]
			[Call(Advice.Around)]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspectForRefInt.f");
				return (T)Context.Invoke(args);
			}
		}
		public class BeforeAspectForOutStruct:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(out V v)
			{
				v =new V();
				v.i=0;
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutStruct.f");	
			}
		}
		public class BeforeAndWildcardAspectForOutStruct:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForOutStruct.f");
			}
		}
		public class InsteadAspectForOutStruct:TestAspectBase
		{
			[Call(Advice.Around)]
			[IncludeAll] [IncludeDeclaredOnly]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"BeforeAndWildcardAspectForOutStruct.f");
				return (T)Context.Invoke(args);
			}
		}
		public class BeforeAspectForRefStStruct:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(ref V v)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForRefStStruct.f");
			}
		}
		public class BeforeAndWildcardAspectForRefStruct:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForRefStruct.f");
			}
		}
		public class InsteadAspectForRefStruct:TestAspectBase
		{
			[Call(Advice.Around)]
			[IncludeAll] [IncludeDeclaredOnly]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspectForRefStruct.f");
				return (T)Context.Invoke(args);
			}
		}
		public class BeforeAspectForOutEnum:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(out Numbers n)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutEnum.f");
				n=Numbers.One;
			}
		}
		public class BeforeAndWildcardAspectForOutEnum:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForOutEnum.f");
			}
		}
		public class BeforeAspectForRefEnum:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(ref Numbers n)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForRefEnum.f");
			}
		}
		public class BeforeAndWildcardAspectRefEnum:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectRefOutEnum.f");
			}
		}
		public class BeforeAspectForRefObject:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(ref O o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAspectForRefObject.f");	
			}
		}
		public class BeforeAndWildcardAspectForRefObject:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForRefObject.f");
			}
		}
		public class InsteadAspectForRefObject:TestAspectBase
		{
			[Call(Advice.Around)]
			[IncludeAll] [IncludeDeclaredOnly]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspectForRefObject.f");
				return (T)Context.Invoke(args);
			}
		}
		public class BeforeAspectForOutObject:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(out O o)
			{
				o=new O();
				o.i=0;
				RegisterCall(ref aspectTestBefore,"BeforeAspectForOutObject.f");
			}
		}
		public class BeforeAndWildcardAspectForOutObject:TestAspectBase
		{
			[Call(Advice.Before)]
			[IncludeAll] [IncludeDeclaredOnly]
			public void f(params object[] o)
			{
				RegisterCall(ref aspectTestBefore,"BeforeAndWildcardAspectForOutObject.f");
			}
		}
		public class InsteadAspectForOutObject:TestAspectBase
		{
			[Call(Advice.Around)]
			[IncludeAll] [IncludeDeclaredOnly]
			public T f<T>([JPContext] Context Context, params object[] args)
			{
				RegisterCall(ref aspectTestInstead,"InsteadAspectForOutObject.f");
				return (T)Context.Invoke(args);
		    }
		}
    }

    #region TestFixtures

    [TestClass]
	public class Before:SimpleWeavingTestBase
	{
		[TestMethod]
		public void NothingToInterweave()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new NothingAspect();
            A a = Weaver.Create<A>(ta);
			a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
		}
        
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect();
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
			ta=new BeforeAspect();
            B b = Weaver.Create<B>(ta);
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
			ta=new BeforeAspect();
            C c = Weaver.Create<C>(ta);
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
			ta=new BeforeAspect();
            D d = Weaver.Create<D>(ta);
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
			ta=new BeforeAspect();
            E e = Weaver.Create<E>(ta);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence E");
			Assert.AreEqual(2,e.testCall,"invalid calling sequence E");
		}
        
		[TestMethod]
		public void PrimitiveRef()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect();
			double d=2.0;
            F f = Weaver.Create<F>(ta);
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
			ta=new BeforeAspect();
            G g = Weaver.Create<G>(ta);
			Assert.AreEqual(v,g.Test(ref v),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence G");
			Assert.AreEqual(2,g.testCall,"invalid calling sequence G");
		}
		
        [TestMethod]
		public void Enum()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new BeforeAspect();
            H h = Weaver.Create<H>(ta);
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
            AllInOne aio = Weaver.Create<AllInOne>(ta);
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
	public class After:SimpleWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			callorder=1;
			TestAspectBase ta=new AfterAspect();
			//A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
            A a = Weaver.Create<A>(ta, null);
			a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterAspect();
			//B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
            B b = Weaver.Create<B>(ta, null);
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
			ta=new AfterAspect();
			//C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
            C c = Weaver.Create<C>(ta, null);
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
			ta=new AfterAspect();
			// D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
            D d = Weaver.Create<D>(ta, null);
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
			ta=new AfterAspect();
			//E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
            E e = Weaver.Create<E>(ta, null);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfter,"invalid calling sequence E");
		}
	}

	[TestClass]
	public class AfterReturning:SimpleWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterReturningAspect();
			//A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
            A a = Weaver.Create<A>(ta, null);
			a.Test();
			Assert.AreEqual(1,a.testCall,"invalid calling sequence A");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence A");
		}
		[TestMethod]
		public void Primitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterReturningAspect();
			//B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
            B b = Weaver.Create<B>(ta, null);
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
			ta=new AfterReturningAspect();
			// C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
            C c = Weaver.Create<C>(ta, null);
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
			ta=new AfterReturningAspect();
			//D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
            D d = Weaver.Create<D>(ta, null);
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
			ta=new AfterReturningAspect();
			//E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
            E e = Weaver.Create<E>(ta, null);
			Assert.AreEqual(v,e.Test(v),"invalid return value");
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"invalid calling sequence E");
		}		
	}
    
	[TestClass]
	public class AfterThrowing:SimpleWeavingTestBase
	{
		[TestMethod]
		public void NoParam()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new AfterThrowingAspect();
			//A a=(A)Weaver.CreateInstance(typeof(A),null,ta);
            A a = Weaver.Create<A>(ta, null);
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
			ta=new AfterThrowingAspect();
			//B b=(B)Weaver.CreateInstance(typeof(B),null,ta);
            B b = Weaver.Create<B>(ta, null);
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
			ta=new AfterThrowingAspect();
			//C c=(C)Weaver.CreateInstance(typeof(C),null,ta);
            C c = Weaver.Create<C>(ta, null);
			c.bThrowException=true;
			c.Test(carg);
			Assert.AreEqual(1,c.testCall,"invalid calling sequence C");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence C");
		}
		[TestMethod]
		public void Reference()
		{
			TestAspectBase ta;
			callorder=1;
			D darg=new D();
			ta=new AfterThrowingAspect();
			//D d=(D)Weaver.CreateInstance(typeof(D),null,ta);
            D d = Weaver.Create<D>(ta, null);
			d.bThrowException=true;
			Assert.AreEqual(null,d.Test(ref darg),"invalid return value");
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
			ta=new AfterThrowingAspect();
			//E e=(E)Weaver.CreateInstance(typeof(E),null,ta);
            E e = Weaver.Create<E>(ta, null);
			e.bThrowException=true;
			e.Test(v);
			Assert.AreEqual(1,e.testCall,"invalid calling sequence E");
			Assert.AreEqual(2,ta.aspectTestAfterThrowing,"invalid calling sequence E");
		}
	}

	[TestClass]
	public class MultiMatchDifferentInvokeOrders:SimpleWeavingTestBase
	{
		[TestMethod]
		public void CallAndReturn()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectInt();
            B b = Weaver.Create<B>(ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"invalid calling sequence B");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"invalid calling sequence B");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence B");
		}
		[TestMethod]
		public void CallAndThrow()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectInt();
            B b = Weaver.Create<B>(ta);
			b.bThrowException=true;
			Assert.AreEqual(0,b.Test(23),"invalid return value");
			Assert.AreEqual(1,ta.aspectTestBefore,"invalid calling sequence B");
			Assert.AreEqual(2,b.testCall,"invalid calling sequence B");
			Assert.AreEqual(0,ta.aspectTestAfterReturning,"invalid calling sequence B");
			Assert.AreEqual(3,ta.aspectTestAfterThrowing,"invalid calling sequence B");
			Assert.AreEqual(4,ta.aspectTestAfter,"invalid calling sequence B");
		}
	}

	[TestClass]
	public class MultiMatchSameInvokeOrders:SimpleWeavingTestBase
	{
		[TestMethod]
		public void BeforePrimitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectBefore();
            B b = Weaver.Create<B>(ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence Before B");
		}
		[TestMethod]
		public void BeforeClass()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectBefore();
            C c = Weaver.Create<C>(ta);
			Assert.AreEqual(c,c.Test(c),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence Before C");
		}
		public void AfterReturningPrimitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectAfterReturning();
            B b = Weaver.Create<B>(ta);
			Assert.AreEqual(23,b.Test(23),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence AfterReturning B");
		}
		[TestMethod]
		public void AfterReturningClass()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectAfterReturning();
            C c = Weaver.Create<C>(ta);
			Assert.AreEqual(c,c.Test(c),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence AfterReturning C");
		}
		[TestMethod] 
		public void AfterThrowingPrimitive()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectAfterThrowingPrimitive();
            B b = Weaver.Create<B>(ta);
			b.bThrowException=true;
			Assert.AreEqual(0,b.Test(23),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence AfterThrowing C");
		}
		[TestMethod]
		public void AfterThrowingClass()
		{
			TestAspectBase ta;
			callorder=1;
			ta=new MultiMatchAspectAfterThrowingClass();
            C c = Weaver.Create<C>(ta);
			c.bThrowException=true;
			Assert.AreEqual(null,c.Test(c),"invalid return value");
			Assert.AreEqual(5,callorder,"invalid calling sequence AfterThrowing C");
		}
	}

 	[TestClass]
	public class Primitives:SimpleWeavingTestBase
	{
		[TestMethod]
		public void TestDouble()
		{
			callorder=1;
			TestAspectBase ta=new AspectTestDouble(System.Double.MinValue);
			// AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Double.MinValue},ta);
            AllPrimitives ap = Weaver.Create<AllPrimitives>(ta, new object[] { System.Double.MinValue });
			Assert.AreEqual( (System.Double)(System.Double.MinValue), ap.TestDouble((System.Double)(System.Double.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestDouble(System.Double.MaxValue);
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
			TestAspectBase ta=new AspectTestSingle(System.Single.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Single.MinValue},ta);
			Assert.AreEqual( (System.Single)(System.Single.MinValue), ap.TestSingle((System.Single)(System.Single.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestSingle(System.Single.MaxValue);
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
			TestAspectBase ta=new AspectTestDecimal(System.Decimal.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Decimal.MinValue},ta);
			Assert.AreEqual( (System.Decimal)(System.Decimal.MinValue), ap.TestDecimal((System.Decimal)(System.Decimal.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestDecimal(System.Decimal.MaxValue);
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
			TestAspectBase ta=new AspectTestUIntPtr((System.UIntPtr)System.UInt32.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.UIntPtr)System.UInt32.MinValue},ta);
			Assert.AreEqual( (System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue), ap.TestUIntPtr((System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestUIntPtr((System.UIntPtr)System.UInt32.MaxValue);
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
			TestAspectBase ta=new AspectTestIntPtr((System.IntPtr)System.Int32.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{(System.IntPtr)System.Int32.MinValue},ta);
			Assert.AreEqual( (System.IntPtr)((System.IntPtr)System.Int32.MinValue), ap.TestIntPtr((System.IntPtr)((System.IntPtr)System.Int32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestIntPtr((System.IntPtr)System.Int32.MaxValue);
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
			TestAspectBase ta=new AspectTestUInt64(System.UInt64.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt64.MinValue},ta);
			Assert.AreEqual( (System.UInt64)(System.UInt64.MinValue), ap.TestUInt64((System.UInt64)(System.UInt64.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestUInt64(System.UInt64.MaxValue);
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
			TestAspectBase ta=new AspectTestInt64(System.Int64.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int64.MinValue},ta);
			Assert.AreEqual( (System.Int64)(System.Int64.MinValue), ap.TestInt64((System.Int64)(System.Int64.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestInt64(System.Int64.MaxValue);
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
			TestAspectBase ta=new AspectTestUInt32(System.UInt32.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt32.MinValue},ta);
			Assert.AreEqual( (System.UInt32)(System.UInt32.MinValue), ap.TestUInt32((System.UInt32)(System.UInt32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestUInt32(System.UInt32.MaxValue);
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
			TestAspectBase ta=new AspectTestInt32(System.Int32.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int32.MinValue},ta);
			Assert.AreEqual( (System.Int32)(System.Int32.MinValue), ap.TestInt32((System.Int32)(System.Int32.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestInt32(System.Int32.MaxValue);
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
			TestAspectBase ta=new AspectTestUInt16(System.UInt16.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.UInt16.MinValue},ta);
			Assert.AreEqual( (System.UInt16)(System.UInt16.MinValue), ap.TestUInt16((System.UInt16)(System.UInt16.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestUInt16(System.UInt16.MaxValue);
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
			TestAspectBase ta=new AspectTestInt16(System.Int16.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Int16.MinValue},ta);
			Assert.AreEqual( (System.Int16)(System.Int16.MinValue), ap.TestInt16((System.Int16)(System.Int16.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestInt16(System.Int16.MaxValue);
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
			TestAspectBase ta=new AspectTestByte(System.Byte.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.Byte.MinValue},ta);
			Assert.AreEqual( (System.Byte)(System.Byte.MinValue), ap.TestByte((System.Byte)(System.Byte.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestByte(System.Byte.MaxValue);
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
			TestAspectBase ta=new AspectTestSByte(System.SByte.MinValue);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{System.SByte.MinValue},ta);
			Assert.AreEqual( (System.SByte)(System.SByte.MinValue), ap.TestSByte((System.SByte)(System.SByte.MinValue)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestSByte(System.SByte.MaxValue);
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
			TestAspectBase ta=new AspectTestBool(false);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{false},ta);
			Assert.AreEqual( (System.Boolean)(false), ap.TestBool((System.Boolean)(false)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			ta=new AspectTestBool(true);
			ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{true},ta);
			Assert.AreEqual( (System.Boolean)(true), ap.TestBool((System.Boolean)(true)),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestChar()
		{
			callorder=1;
			System.Char c = 'a';
			AspectTestChar ta=new AspectTestChar(c);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{c},ta);
			Assert.AreEqual('a',ap.TestChar(c),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");		
			Assert.AreEqual(1,ta.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ta.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(4,ta.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(5,ta.aspectTestAfter,"Aspect not jpinterwoven");			
		}
		[TestMethod]
		public void TestString()
		{
			callorder=1;
			System.String s = "a";
			AspectTestString ta=new AspectTestString(s);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{s},ta);
			Assert.AreEqual("a",ap.TestString(s),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");		
			Assert.AreEqual(1,ta.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ta.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(4,ta.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(5,ta.aspectTestAfter,"Aspect not jpinterwoven");	
		}
		[TestMethod]
		public void TestObject()
		{
			callorder=1;
			System.Object o = "a";
			AspectTestObject a=new AspectTestObject(o);
			AllPrimitives ap=(AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{o},a);
			Assert.AreEqual("a",ap.TestObject(o),"invalid return value");
			Assert.AreEqual(0,a.aspectTestAfterThrowing,"wrong method called");		
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,ap.testCall,"object not jpinterwoven");
			Assert.AreEqual(4,a.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(5,a.aspectTestAfter,"Aspect not jpinterwoven");	
		}
		[TestMethod]
		public void TestRefObject()
		{			
			callorder=1;
			System.Object o = "a";
			AspectTestObjectRef a=new AspectTestObjectRef(o);
			// AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{o},a);
            AllPrimitivesRef ap = Weaver.Create<AllPrimitivesRef>(a, new object[] { o });
			Assert.AreEqual(o,ap.TestObjectRef(ref o),"invalid return value");
			Assert.AreEqual(0,a.aspectTestAfterThrowing,"wrong method called");		
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,ap.testCall,"object not jpinterwoven");
			Assert.AreEqual(4,a.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(5,a.aspectTestAfter,"Aspect not jpinterwoven");	
		}
		[TestMethod]
		public void TestStringArray()
		{
			callorder=1;
			System.String[] s = new string[5];
			s[0] = "L";
			s[1] = "O";
			s[2] = "O";
			s[3] = "M";
			AspectTestStringArray a = new AspectTestStringArray(s);
			AllPrimitives ap = (AllPrimitives)Weaver.CreateInstance(typeof(AllPrimitives),new object[]{s},a);
			ap.TestStringArray(s);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(3,ap.testCall,"object not jpinterwoven");
			Assert.AreEqual(4,a.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(5,a.aspectTestAfter,"Aspect not jpinterwoven");
			Assert.AreEqual(0,a.aspectTestAfterThrowing,"exception catched");		
		}
		[TestMethod]
		public void TestStringRef()
		{
			callorder=1;
			System.String s = "a";
			AspectTestStringRef ta=new AspectTestStringRef(s);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{s},ta);
			Assert.AreEqual("a",ap.TestStringRef(ref s),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");		
			Assert.AreEqual(1,ta.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(4,ta.aspectTestAfter,"Aspect not jpinterwoven");	
		}
		[TestMethod]
		public void TestCharRef()
		{
			callorder=1;
			System.Char c = 'a';
			AspectTestCharRef ta=new AspectTestCharRef(c);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{c},ta);
			Assert.AreEqual('a',ap.TestCharRef(ref c),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(3,ta.aspectTestAfterReturning,"Aspect not jpinterwoven");
			Assert.AreEqual(4,ta.aspectTestAfter,"Aspect not jpinterwoven");
			
		}
		[TestMethod]
		public void TestDoubleRef()
		{
			callorder=1;
			System.Double p=(System.Double)(System.Double.MinValue);
			TestAspectBase ta=new AspectTestDoubleRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Double)(System.Double.MinValue), ap.TestDoubleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Double)(System.Double.MaxValue);
			ta=new AspectTestDoubleRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Double)(System.Double.MaxValue), ap.TestDoubleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSingleRef()
		{
			callorder=1;
			System.Single p=(System.Single)(System.Single.MinValue);
			TestAspectBase ta=new AspectTestSingleRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Single)(System.Single.MinValue), ap.TestSingleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Single)(System.Single.MaxValue);
			ta=new AspectTestSingleRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Single)(System.Single.MaxValue), ap.TestSingleRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestDecimalRef()
		{
			callorder=1;
			System.Decimal p=(System.Decimal)(System.Decimal.MinValue);
			TestAspectBase ta=new AspectTestDecimalRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Decimal)(System.Decimal.MinValue), ap.TestDecimalRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Decimal)(System.Decimal.MaxValue);
			ta=new AspectTestDecimalRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Decimal)(System.Decimal.MaxValue), ap.TestDecimalRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUIntPtrRef()
		{
			callorder=1;
			System.UIntPtr p=(System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue);
			TestAspectBase ta=new AspectTestUIntPtrRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UIntPtr)((System.UIntPtr)System.UInt32.MinValue), ap.TestUIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.UIntPtr)((System.UIntPtr)System.UInt32.MaxValue);
			ta=new AspectTestUIntPtrRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UIntPtr)((System.UIntPtr)System.UInt32.MaxValue), ap.TestUIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestIntPtrRef()
		{
			callorder=1;
			System.IntPtr p=(System.IntPtr)((System.IntPtr)System.Int32.MinValue);
			TestAspectBase ta=new AspectTestIntPtrRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.IntPtr)((System.IntPtr)System.Int32.MinValue), ap.TestIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.IntPtr)((System.IntPtr)System.Int32.MaxValue);
			ta=new AspectTestIntPtrRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.IntPtr)((System.IntPtr)System.Int32.MaxValue), ap.TestIntPtrRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt64Ref()
		{
			callorder=1;
			System.UInt64 p=(System.UInt64)(System.UInt64.MinValue);
			TestAspectBase ta=new AspectTestUInt64Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt64)(System.UInt64.MinValue), ap.TestUInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.UInt64)(System.UInt64.MaxValue);
			ta=new AspectTestUInt64Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt64)(System.UInt64.MaxValue), ap.TestUInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt64Ref()
		{
			callorder=1;
			System.Int64 p=(System.Int64)(System.Int64.MinValue);
			TestAspectBase ta=new AspectTestInt64Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int64)(System.Int64.MinValue), ap.TestInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Int64)(System.Int64.MaxValue);
			ta=new AspectTestInt64Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int64)(System.Int64.MaxValue), ap.TestInt64Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt32Ref()
		{
			callorder=1;
			System.UInt32 p=(System.UInt32)(System.UInt32.MinValue);
			TestAspectBase ta=new AspectTestUInt32Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt32)(System.UInt32.MinValue), ap.TestUInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.UInt32)(System.UInt32.MaxValue);
			ta=new AspectTestUInt32Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt32)(System.UInt32.MaxValue), ap.TestUInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt32Ref()
		{
			callorder=1;
			System.Int32 p=(System.Int32)(System.Int32.MinValue);
			TestAspectBase ta=new AspectTestInt32Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int32)(System.Int32.MinValue), ap.TestInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Int32)(System.Int32.MaxValue);
			ta=new AspectTestInt32Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int32)(System.Int32.MaxValue), ap.TestInt32Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestUInt16Ref()
		{
			callorder=1;
			System.UInt16 p=(System.UInt16)(System.UInt16.MinValue);
			TestAspectBase ta=new AspectTestUInt16Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt16)(System.UInt16.MinValue), ap.TestUInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.UInt16)(System.UInt16.MaxValue);
			ta=new AspectTestUInt16Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.UInt16)(System.UInt16.MaxValue), ap.TestUInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestInt16Ref()
		{
			callorder=1;
			System.Int16 p=(System.Int16)(System.Int16.MinValue);
			TestAspectBase ta=new AspectTestInt16Ref(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int16)(System.Int16.MinValue), ap.TestInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Int16)(System.Int16.MaxValue);
			ta=new AspectTestInt16Ref(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Int16)(System.Int16.MaxValue), ap.TestInt16Ref(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestByteRef()
		{
			callorder=1;
			System.Byte p=(System.Byte)(System.Byte.MinValue);
			TestAspectBase ta=new AspectTestByteRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Byte)(System.Byte.MinValue), ap.TestByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Byte)(System.Byte.MaxValue);
			ta=new AspectTestByteRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Byte)(System.Byte.MaxValue), ap.TestByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestSByteRef()
		{
			callorder=1;
			System.SByte p=(System.SByte)(System.SByte.MinValue);
			TestAspectBase ta=new AspectTestSByteRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.SByte)(System.SByte.MinValue), ap.TestSByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.SByte)(System.SByte.MaxValue);
			ta=new AspectTestSByteRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.SByte)(System.SByte.MaxValue), ap.TestSByteRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
		[TestMethod]
		public void TestBoolRef()
		{
			callorder=1;
			System.Boolean p=(System.Boolean)(false);
			TestAspectBase ta=new AspectTestBoolRef(p);
			AllPrimitivesRef ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Boolean)(false), ap.TestBoolRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");

			callorder=1;
			p=(System.Boolean)(true);
			ta=new AspectTestBoolRef(p);
			ap=(AllPrimitivesRef)Weaver.CreateInstance(typeof(AllPrimitivesRef),new object[]{p},ta);
			Assert.AreEqual( (System.Boolean)(true), ap.TestBoolRef(ref p),"invalid return value");
			Assert.AreEqual(0,ta.aspectTestAfterThrowing,"wrong method called");
			Assert.AreEqual(1,ta.aspectTestBefore,"method not called");
			Assert.AreEqual(2,ta.aspectTestAfterReturning,"method not called");
			Assert.AreEqual(3,ta.aspectTestAfter,"method not called");
		}
	}
	
    [TestClass]
	public class OutAndRefParameterWeaving:SimpleWeavingTestBase
	{
		[TestMethod]
		public void BeforeOutInt()
		{
			callorder=1;
			BeforeAspectForOutInt a=new BeforeAspectForOutInt();
			L l=(L)Weaver.CreateInstance(typeof(L),null,a);
			int i=0;
			l.OutParameter(out i);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"out Parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardOutInt()
		{
			callorder=1;
			BeforeAndWildcardAspectForOutInt a=new BeforeAndWildcardAspectForOutInt();
			//L l=(L)Weaver.CreateInstance(typeof(L),null,a);
            L l = Weaver.Create<L>(a);
			int i=0;
			l.OutParameter(out i);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"out Parameter not changed");
		}
		[TestMethod]
		public void InsteadOutInt()
		{
			callorder=1;
			InsteadAspectForOutInt a=new InsteadAspectForOutInt();
			// L l=(L)Weaver.CreateInstance(typeof(L),null,a);
            L l = Weaver.Create<L>(a);
			int i=0;
			l.OutParameter(out i);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"out Parameter not changed");
		}
		[TestMethod]
		public void BeforeRefInt()
		{
			callorder=1;
			SimpleWeavingTestBase.BeforeAspectForRefInt a=new SimpleWeavingTestBase.BeforeAspectForRefInt();
			L l=(L)Weaver.CreateInstance(typeof(L),null,a);
			int i=0;
			l.RefParameter(ref i);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"ref parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardRefInt()
		{
			callorder=1;
			BeforeAndWildcardAspectForRefInt a=new BeforeAndWildcardAspectForRefInt();
			L l=(L)Weaver.CreateInstance(typeof(L),null,a);
			int i=0;
			l.RefParameter(ref i);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"ref parameter not changed");
		}
		[TestMethod]
		public void InsteadRefInt()
		{
			callorder=1;
			InsteadAspectForRefInt a=new InsteadAspectForRefInt();
			L l=(L)Weaver.CreateInstance(typeof(L),null,a);
			int i=0;
			l.RefParameter(ref i);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,l.testCall,"object not called");
			Assert.AreEqual(1,i,"ref parameter not changed");
		}
		[TestMethod]
		public void BeforeOutStruct()
		{
			callorder=1;
			BeforeAspectForOutStruct a=new BeforeAspectForOutStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v;
			m.OutParameter(out v);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardOutStruct()
		{
			callorder=1;
			BeforeAndWildcardAspectForOutStruct a=new BeforeAndWildcardAspectForOutStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v;
			m.OutParameter(out v);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"out parameter not changed");
		}
		[TestMethod]
		public void InsteadOutStruct()
		{
			callorder=1;
			InsteadAspectForOutStruct a=new InsteadAspectForOutStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v;
			m.OutParameter(out v);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeRefStruct()
		{
			callorder=1;
			BeforeAspectForRefStStruct a=new BeforeAspectForRefStStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v=new V();
			v.i=0;
			m.RefParameter(ref v);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"ref Parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardRefStruct()
		{
			callorder=1;
			BeforeAndWildcardAspectForRefStruct a=new BeforeAndWildcardAspectForRefStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v=new V();
			v.i=0;
			m.RefParameter(ref v);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"ref Parameter not changed");
		}
		[TestMethod]
		public void InsteadRefStruct()
		{
			callorder=1;
			InsteadAspectForRefStruct a=new InsteadAspectForRefStruct();
			M m=(M)Weaver.CreateInstance(typeof(M),null,a);
			V v=new V();
			v.i=0;
			m.RefParameter(ref v);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,m.testCall,"object not called");
			Assert.AreEqual(1,v.i,"ref Parameter not changed");
		}
		[TestMethod]
		public void BeforeOutEnum()
		{
			callorder=1;
			BeforeAspectForOutEnum a=new BeforeAspectForOutEnum();
			N n=(N)Weaver.CreateInstance(typeof(N),null,a);
			Numbers o;
			n.OutParameter(out o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,n.testCall,"object not called");
			Assert.AreEqual(Numbers.Two,o,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardOutEnum()
		{
			callorder=1;
			BeforeAndWildcardAspectForOutEnum a=new BeforeAndWildcardAspectForOutEnum();
			N n=(N)Weaver.CreateInstance(typeof(N),null,a);
			Numbers o;
			n.OutParameter(out o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,n.testCall,"object not called");
			Assert.AreEqual(Numbers.Two,o,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeRefEnum()
		{
			callorder=1;
			BeforeAspectForRefEnum a=new BeforeAspectForRefEnum();
			N n=(N)Weaver.CreateInstance(typeof(N),null,a);
			Numbers o=Numbers.One;
			n.RefParameter(ref o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,n.testCall,"object not called");
			Assert.AreEqual(Numbers.Two,o,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardRefEnum()
		{
			callorder=1;
			BeforeAndWildcardAspectRefEnum a=new BeforeAndWildcardAspectRefEnum();
			N n=(N)Weaver.CreateInstance(typeof(N),null,a);
			Numbers o=Numbers.One;
			n.RefParameter(ref o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,n.testCall,"object not called");
			Assert.AreEqual(Numbers.Two,o,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeOutObject()
		{
			callorder=1;
			BeforeAspectForOutObject a=new BeforeAspectForOutObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o;
			p.OutParameter(out o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardOutObject()
		{
			callorder=1;
			BeforeAndWildcardAspectForOutObject a=new BeforeAndWildcardAspectForOutObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o;
			p.OutParameter(out o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"out parameter not changed");
		}
		[TestMethod]
		public void InsteadOutObject()
		{
			callorder=1;
			InsteadAspectForOutObject a=new InsteadAspectForOutObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o;
			p.OutParameter(out o);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"out parameter not changed");
		}
		[TestMethod]
		public void BeforeRefObject()
		{
			callorder=1;
			BeforeAspectForRefObject a=new BeforeAspectForRefObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o=new O();
			o.i=0;
			p.RefParameter(ref o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"ref parameter not changed");
		}
		[TestMethod]
		public void BeforeAndWildcardRefObject()
		{
			callorder=1;
			BeforeAndWildcardAspectForRefObject a=new BeforeAndWildcardAspectForRefObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o=new O();
			o.i=0;
			p.RefParameter(ref o);
			Assert.AreEqual(1,a.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"ref parameter not changed");
		}
		[TestMethod]
		public void InsteadRefObject()
		{
			callorder=1;
			InsteadAspectForRefObject a=new InsteadAspectForRefObject();
			P p=(P)Weaver.CreateInstance(typeof(P),null,a);
			O o=new O();
			o.i=0;
			p.RefParameter(ref o);
			Assert.AreEqual(1,a.aspectTestInstead,"Aspect not jpinterwoven");
			Assert.AreEqual(2,p.testCall,"object not called");
			Assert.AreEqual(1,o.i,"ref parameter not changed");
		}
    }

#endregion
}
