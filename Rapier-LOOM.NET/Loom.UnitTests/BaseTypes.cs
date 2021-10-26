// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom;
using Loom.WeaverMessages;

namespace Loom.UnitTests
{
	public class TestBase
	{

		//static private WeaverMessageEventHandler evt;
		protected static int callorder;
		[ClassInitialize]
		public void SetUp()
		{
			//Weaver.WeaverMessages+=evt=new WeaverMessageEventHandler(ReportWeaverEvent);
		}
		[ClassCleanup]
		public void TearDown()
		{
			//Weaver.Saveprovider();
			//Weaver.WeaverMessages-=evt;
			//System.GC.Collect();
		}
		void ReportWeaverEvent(object sender, WeaverMessageEventArgs args)
		{
			StringBuilder sb=new StringBuilder();
			sb.Append(args.Type.ToString());
			sb.Append(": ");
			sb.Append(args.Message);
			string msg=sb.ToString();

			System.Diagnostics.Debug.WriteLine(msg);
			switch(args.Type)
			{
				default:
				case MessageType.Error:
				case MessageType.Warning:
					System.Console.Error.WriteLine(msg);
					break;
				case MessageType.Info:
					System.Console.Out.WriteLine(msg);
					break;
			}
		}
		public static void RegisterCall(ref int call, string method)
		{
			Assert.IsTrue(call==0,method+" was called twice");
			call=callorder++;
		}
		public class TestException:Exception
		{}
		/// <summary>Base class.</summary>
		[Serializable]
		public class ClassBase
		{
			public int testCall=0;
			public bool bThrowException;
		}
		[AttributeUsage(AttributeTargets.Method)]
		public class MethodTestAttribute:Attribute
		{}
		public class A:ClassBase
		{
			public virtual void Test()
			{
				RegisterCall(ref testCall,"A.Test1()");
				if(bThrowException) throw new TestException();
			}
			[MethodTest]
			public virtual void Test2()
			{
				RegisterCall(ref testCall,"A.Test2()");
				if(bThrowException) throw new TestException();
			}
		}
		public class A1:A
		{
			public virtual void Test11()
			{
				RegisterCall(ref testCall,"A1.Test11()");
				if(bThrowException) throw new TestException();
			}
		}
		public class B:ClassBase
		{
			public virtual int Test(int i)
			{
				RegisterCall(ref testCall,"B.Test()");
				if(bThrowException) throw new TestException();
				return i;
			}
			[MethodTest]
			public virtual int Test2(int i)
			{
				RegisterCall(ref testCall,"B.Test()");
				if(bThrowException) throw new TestException();
				return i;
			}
		}
		public class B1:B
		{
			public virtual int Test11(int i)
			{
				RegisterCall(ref testCall,"B1.Test11()");
				if(bThrowException) throw new TestException();
				return i;
			}
		}
		public class C:ClassBase
		{
			public virtual C Test(C c)
			{
				RegisterCall(ref testCall,"C.Test()");
				if(bThrowException) throw new TestException();
				return c;
			}
		}
		public class D:ClassBase
		{
			public virtual D Test(ref D d)
			{
				RegisterCall(ref testCall,"D.Test()");
				if(bThrowException) throw new TestException();
				return d;
			}
		}
		public struct V
		{
			public int i;
			public double d;
			public string s;
			public void InitStruct()
			{
				i=1;
				d=2.0;
				s="Test";
			}
		}
		public class E:ClassBase
		{
			public virtual V Test(V v)
			{
				RegisterCall(ref testCall,"E.Test()");
				if(bThrowException) throw new TestException();
				return v;
			}
		}
		public class F:ClassBase
		{
			public virtual double Test(ref double d)
			{
				RegisterCall(ref testCall,"F.Test()");
				if(bThrowException) throw new TestException();
				return d;
			}
			public virtual double Test2(ref double d)
			{
				RegisterCall(ref testCall,"F.Test2()");
				if(bThrowException) throw new TestException();
				return d;
			}
		}
		public class G:ClassBase
		{
			public virtual V Test(ref V v)
			{
				RegisterCall(ref testCall,"G.Test()");
				if(bThrowException) throw new TestException();
				return v;
			}
		}
		public enum Numbers {One, Two, Three};
		public class H:ClassBase
		{
			public virtual Numbers Test(Numbers n)
			{
				RegisterCall(ref testCall,"H.Test()");
				if(bThrowException) throw new TestException();
				return n;
			}
		}
		public class I:ClassBase
		{
			public virtual void Test(params object[] o)
			{
				RegisterCall(ref testCall,"I.Test()");
				if(bThrowException) throw new TestException();
			}
			public virtual object Test2()
			{
				RegisterCall(ref testCall,"I.Test2()");
				if(bThrowException) throw new TestException();
				object o=new object();
				return o;
			}
			public virtual object Test3(params object[] o)
			{
				RegisterCall(ref testCall,"I.Test3()");
				if(bThrowException) throw new TestException();
				return o[0];
			}
			public virtual int Test4()
			{
				RegisterCall(ref testCall,"I.Test4()");
				if(bThrowException) throw new TestException();
				int i=1;
				return i;
			}
			public virtual void Test5(int i)
			{
				RegisterCall(ref testCall,"I.Test5()");
				if(bThrowException) throw new TestException();
			}
			public virtual int Test6(int i)
			{
				RegisterCall(ref testCall,"I.Test6()");
				if(bThrowException) throw new TestException();
				return i;
			}
		}
		public class J:ClassBase
		{
			public object o;
			public J()
			{
				RegisterCall(ref testCall,"J.J");
				o="empty";
			}
			public J(int i)
			{
				RegisterCall(ref testCall,"J.JWithIntParameter");
				o=i;
			}
			public J(string s)
			{
				RegisterCall(ref testCall,"J.JWithStringParameter");
				o=s;
			}
		}
        public class J2 : ClassBase
        {
            public object o;
            public J2(int i)
            {
                RegisterCall(ref testCall, "J2.JWithIntParameter");
                o = i;
            }
            public J2(J2 j)
            {
                RegisterCall(ref testCall, "J2.JWithTypeParameter");
                o = j;
            }
            public J2(object obj)
            {
                RegisterCall(ref testCall, "J2.JWithObjectParameter");
                o = obj;
            }
        }
		public class K:ClassBase
		{
			public virtual void k(int a,int b,int c,int d,int e,int f,int g,int h,int i,int j,int k,int l,int m, int n)
			{
				RegisterCall(ref testCall,"K.k()");
				if(bThrowException) throw new TestException();
			}
		}
		public class L:ClassBase
		{
			public virtual void RefParameter(ref int i)
			{
				i=1;
				RegisterCall(ref testCall,"L.Ref()");
				if(bThrowException) throw new TestException();
			}
			public virtual void OutParameter(out int i)
			{
				i=1;
				RegisterCall(ref testCall,"L.Out()");
				if(bThrowException) throw new TestException();
			}
		}
		public class M:ClassBase
		{
			public virtual void RefParameter(ref V v)
			{
				v.i=1;
				RegisterCall(ref testCall,"M.RefParameter()");
				if(bThrowException) throw new TestException();
			}
			public virtual void OutParameter(out V v)
			{
				v=new V();
				v.i=1;
				RegisterCall(ref testCall,"M.OutParameter()");
				if(bThrowException) throw new TestException();
			}
		}
		public class N:ClassBase
		{
			public virtual void RefParameter(ref Numbers n)
			{
				n=Numbers.Two;
				RegisterCall(ref testCall,"N.RefParameter()");
				if(bThrowException) throw new TestException();
			}
			public virtual void OutParameter(out Numbers n)
			{
				n=Numbers.Two;
				RegisterCall(ref testCall,"N.OutParameter()");
				if(bThrowException) throw new TestException();
			}
		}
		public class O
		{
			public int i=0;
		}
		public class P:ClassBase
		{
			public virtual void RefParameter(ref O o)
			{
				o.i=1;
				RegisterCall(ref testCall,"P.RefParameter()");
				if(bThrowException) throw new TestException();
			}
			public virtual void OutParameter(out O o)
			{
				o=new O();
				o.i=1;
				RegisterCall(ref testCall,"P.OutParameter()");
				if(bThrowException) throw new TestException();
			}
		}
		public class Q:ClassBase
		{
			public Q(out int i)
			{
				i=1;
				RegisterCall(ref testCall,"Q.Q()");
				if(bThrowException) throw new TestException();
			}
		}
		public class R:ClassBase
		{
			public R(ref int i)
			{
				i=1;
				RegisterCall(ref testCall,"R.R()");
				if(bThrowException) throw new TestException();
			}
		}
		public class AllInOne:ClassBase
		{
			public virtual void Test()
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
			}
			public virtual int Test(int i)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return i;
			}
			public virtual C Test(C c)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return c;
			}
			public virtual D Test(ref D d)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return d;
			}
			public virtual double Test(ref double d)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return d;
			}
			public virtual V Test(V v)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return v;
			}
			public virtual V Test(ref V v)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return v;
			}
			public virtual Numbers Test(Numbers n)
			{
				RegisterCall(ref testCall,"AllInOne.Test()");
				if(bThrowException) throw new TestException();
				return n;
			}
		}
		public class AllPrimitives:ClassBase
		{
			public object expected;
			public AllPrimitives(object expected)
			{
				this.expected=expected;
			}

			public virtual System.Boolean TestBool(System.Boolean b1)
			{
				Assert.AreEqual( expected, b1,"invalid parameter");
				return b1;
			}
			public virtual System.SByte TestSByte(System.SByte i8)
			{
				Assert.AreEqual(expected, i8,"invalid parameter");
				return i8;
			}
			public virtual System.Byte TestByte(System.Byte u8)
			{
				Assert.AreEqual(expected, u8,"invalid parameter");
				return u8;
			}
			public virtual System.Int16 TestInt16(System.Int16 i16)
			{
				Assert.AreEqual(expected, i16,"invalid parameter");
				return i16;
			}
			public virtual System.UInt16 TestUInt16(System.UInt16 u16)
			{
				Assert.AreEqual(expected, u16,"invalid parameter");
				return u16;
			}
			public virtual System.Int32 TestInt32(System.Int32 i32)
			{
				Assert.AreEqual(expected, i32,"invalid parameter");
				return i32;
			}
			public virtual System.UInt32 TestUInt32(System.UInt32 u32)
			{
				Assert.AreEqual(expected, u32,"invalid parameter");
				return u32;
			}
			public virtual System.Int64 TestInt64(System.Int64 i64)
			{
				Assert.AreEqual(expected, i64,"invalid parameter");
				return i64;
			}
			public virtual System.UInt64 TestUInt64(System.UInt64 u64)
			{
				Assert.AreEqual(expected, u64,"invalid parameter");
				return u64;
			}
			public virtual System.IntPtr TestIntPtr(System.IntPtr ip)
			{
				Assert.AreEqual( expected, ip,"invalid parameter");
				return ip;
			}
			public virtual System.UIntPtr TestUIntPtr(System.UIntPtr up)
			{
				Assert.AreEqual( expected, up,"invalid parameter");
				return up;
			}
			public virtual System.Decimal TestDecimal(System.Decimal dec)
			{
				Assert.AreEqual( expected, dec,"invalid parameter");
				return dec;
			}
			public virtual System.Single TestSingle(System.Single r4)
			{
				Assert.AreEqual( expected, r4,"invalid parameter");
				return r4;
			}
			public virtual System.Double TestDouble(System.Double r8)
			{
				Assert.AreEqual(expected, r8,"invalid parameter");
				return r8;
			}
			public virtual System.Char TestChar(System.Char c)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestChar()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected, c,"invalid parameter");
				return c;
			}
			public virtual System.String TestString(System.String s)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestString()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected, s,"invalid parameter");
				return s;
			}
			public virtual System.Object TestObject(System.Object o)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestObject()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected,o,"invalid parameter");
				return o;
			}
			public virtual System.String[] TestStringArray(System.String[] s)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestStringArray()");
				if(bThrowException) throw new TestException();
				return s;
			}
		}
		public class AllPrimitivesRef:ClassBase
		{
			private object expected;
			public AllPrimitivesRef(object expected)
			{
				this.expected=expected;
			}
			public virtual System.Boolean TestBoolRef(ref System.Boolean b1)
			{
				Assert.AreEqual( expected, b1,"invalid parameter");
				return b1;
			}
			public virtual System.SByte TestSByteRef(ref System.SByte i8)
			{
				Assert.AreEqual(expected, i8,"invalid parameter");
				return i8;
			}
			public virtual System.Byte TestByteRef(ref System.Byte u8)
			{
				Assert.AreEqual(expected, u8,"invalid parameter");
				return u8;
			}
			public virtual System.Int16 TestInt16Ref(ref System.Int16 i16)
			{
				Assert.AreEqual(expected, i16,"invalid parameter");
				return i16;
			}
			public virtual System.UInt16 TestUInt16Ref(ref System.UInt16 u16)
			{
				Assert.AreEqual(expected, u16,"invalid parameter");
				return u16;
			}
			public virtual System.Int32 TestInt32Ref(ref System.Int32 i32)
			{
				Assert.AreEqual(expected, i32,"invalid parameter");
				return i32;
			}
			public virtual System.UInt32 TestUInt32Ref(ref System.UInt32 u32)
			{
				Assert.AreEqual(expected, u32,"invalid parameter");
				return u32;
			}
			public virtual System.Int64 TestInt64Ref(ref System.Int64 i64)
			{
				Assert.AreEqual(expected, i64,"invalid parameter");
				return i64;
			}
			public virtual System.UInt64 TestUInt64Ref(ref System.UInt64 u64)
			{
				Assert.AreEqual(expected, u64,"invalid parameter");
				return u64;
			}
			public virtual System.IntPtr TestIntPtrRef(ref System.IntPtr ip)
			{
				Assert.AreEqual( expected, ip,"invalid parameter");
				return ip;
			}
			public virtual System.UIntPtr TestUIntPtrRef(ref System.UIntPtr up)
			{
				Assert.AreEqual( expected, up,"invalid parameter");
				return up;
			}
			public virtual System.Decimal TestDecimalRef(ref System.Decimal dec)
			{
				Assert.AreEqual( expected, dec,"invalid parameter");
				return dec;
			}
			public virtual System.Single TestSingleRef(ref System.Single r4)
			{
				Assert.AreEqual( expected, r4,"invalid parameter");
				return r4;
			}
			public virtual System.Double TestDoubleRef(ref System.Double r8)
			{
				Assert.AreEqual(expected, r8,"invalid parameter");
				return r8;
			}
			public virtual System.Char TestCharRef(ref System.Char c)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestCharRef()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected, c,"invalid parameter");
				return c;
			}
			public virtual System.String TestStringRef(ref System.String s)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestStringRef()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected, s,"invalid parameter");
				return s;
			}
			public virtual System.Object TestObjectRef(ref System.Object o)
			{
				RegisterCall(ref testCall,"AllPrimitives.TestObjectRef()");
				if(bThrowException) throw new TestException();
				Assert.AreEqual(expected,o,"invalid parameter");
				return o;
			}
		}
		/// <summary>Base AspectAttribute.</summary>
		[Serializable]
		public class TestAspectBase:AspectAttribute
		{
			public int aspectTestBefore=0;
			public int aspectTestAfter=0;
			public int aspectTestInstead=0;
			public int aspectTestAfterThrowing=0;
			public int aspectTestAfterReturning=0;
			public int testCall=0;
		}
		public interface I1
		{
			int Inc(int i);
		}
		public interface I2
		{
			int Inc(int i);
		}
		public class S:ClassBase,I1,I2
		{
			public int Inc(int i)
			{
				RegisterCall(ref testCall,"S.Inc()");
				if(bThrowException) throw new TestException();
				return i++;
			}
		}
		public class VS:ClassBase,I1,I2
		{
			public virtual int Inc(int i)
			{
				RegisterCall(ref testCall,"VS.Inc()");
				if(bThrowException) throw new TestException();
				return i++;
			}
		}
		public class VS1:ClassBase,I1
		{
			public virtual int Inc(int i)
			{
				RegisterCall(ref testCall,"VS1.Inc()");
				if(bThrowException) throw new TestException();
				return ++i;
			}
		}
		public class VS2:ClassBase,I1
		{
			public int Inc(int i)
			{
				RegisterCall(ref testCall,"VS2.Inc()");
				if(bThrowException) throw new TestException();
				return ++i;
			}
		}
		/// <summary>
		/// Class with Serializable Attribute for testing serialization of dynamic jpinterwoven objects.
		/// </summary>
		[Serializable]
		public class T:ClassBase
		{
			public int i = 0;
			public string s = "";
			public T(){}
			public T(int i,string s)
			{
				this.i = i;
				this.s = s;
			}
			public virtual void incI()
			{
				RegisterCall(ref testCall,"T.incI()");
				if(bThrowException) throw new TestException();
				++i;
			}
		}		
		/// <summary>
		/// Class for testing serialization of dynamic jpinterwoven objects that implement ISerializable.
		/// </summary>
		[Serializable]
		public class U:ClassBase,ISerializable
		{
			public int i = 0;
			public string s = "";
			public U(SerializationInfo info,StreamingContext context)
			{
				this.i = info.GetInt32("i");
				this.s = info.GetString("s");
			}
			public U(int i,string s)
			{
				this.i = i;
				this.s = s;
			}
			public virtual void incI()
			{
				RegisterCall(ref testCall,"U.incI()");
				if(bThrowException) throw new TestException();
				++i;
			}
			void ISerializable.GetObjectData(SerializationInfo info,StreamingContext context)
			{
				info.AddValue("i",i);
				info.AddValue("s",s);
			}
		}
		public interface IW
		{
			int Add(params object[] args);
		}
		public class W:ClassBase,IW
		{
			public virtual int Add(params object[] args)
			{
				RegisterCall(ref testCall,"W.Add()");
				if(bThrowException) throw new TestException();
				int res=0;
				foreach(object o in args)
					res+=Convert.ToInt32(o);
				return res;
			}
		}
		public class X:ClassBase
		{
			public static int call = 0;
			public X()
			{
				RegisterCall(ref testCall,"X.X()");
			}
			~X()
			{
				testCall = 0;
				RegisterCall(ref testCall,"X.~X()");
				call = testCall;
			}
		}
	}
}
