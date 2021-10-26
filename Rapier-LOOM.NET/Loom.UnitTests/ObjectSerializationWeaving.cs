// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom;
using Loom.JoinPoints;

namespace Loom.UnitTests.ObjectSerialization
{
	/// <summary>
	/// ObjectSerializationTestBase contains all aspects for testing object serialization for jpinterwoven objects.
	/// </summary>
	public class ObjectSerializationTestBase:TestBase
	{
		/// <summary>
		/// AspectAttribute for interweaving a method call with Advice.Before.
		/// </summary>
		[Serializable]
		[AttributeUsage(AttributeTargets.Class)]
		public class BeforeAspect1:TestAspectBase
		{
			[IncludeAll()][Call(Advice.Before)]
			public void incI()
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspect1.incI");
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a method call with Advice.Before.
		/// </summary>
		[Serializable]
		[AttributeUsage(AttributeTargets.Class)]
		public class BeforeAspect2:TestAspectBase
		{
			[IncludeAll()][Call(Advice.Before)]
			public void incI()
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspect2.incI");
			}
		}
		/// <summary>
		/// AspectAttribute for interweaving a method call with Advice.Before.
		/// </summary>
		[Serializable]
		[AttributeUsage(AttributeTargets.Class)]
		public class BeforeAspect3:TestAspectBase
		{
			[IncludeAll()][Call(Advice.Before)]
			public void incI()
			{
				RegisterCall(ref aspectTestBefore, "BeforeAspect3.incI");
			}
		}
	}
	/// <summary>
	/// Class with Serializable Attribute for testing serialization of static jpinterwoven objects with one Aspect.
	/// </summary>
	[Serializable]
	[ObjectSerializationTestBase.BeforeAspect1]
	public class T1:TestBase.ClassBase
	{
		public int i = 0;
		public string s = "";
		public T1(){}
		public T1(int i,string s)
		{
			this.i = i;
			this.s = s;
		}
		public virtual void incI()
		{
			TestBase.RegisterCall(ref testCall,"T1.incI()");
			if(bThrowException) throw new TestBase.TestException();
			++i;
		}
	}
	/// <summary>
	/// Class with Serializable Attribute for testing serialization of static jpinterwoven objects with three aspects.
	/// </summary>
	[Serializable]
	[ObjectSerializationTestBase.BeforeAspect1]
	[ObjectSerializationTestBase.BeforeAspect2]
	[ObjectSerializationTestBase.BeforeAspect3]
	public class T2:TestBase.ClassBase
	{
		public int i = 0;
		public string s = "";
		public T2(){}
		public T2(int i,string s)
		{
			this.i = i;
			this.s = s;
		}
		public virtual void incI()
		{
			TestBase.RegisterCall(ref testCall,"T2.incI()");
			if(bThrowException) throw new TestBase.TestException();
			++i;
		}
	}
	/// <summary>
	/// Class for testing serialization of static jpinterwoven objects with one Aspect that implement ISerializable.
	/// </summary>
	[Serializable]
	[ObjectSerializationTestBase.BeforeAspect1]
	public class U1:TestBase.ClassBase,ISerializable
	{
		public int i = 0;
		public string s = "";
		public U1(SerializationInfo info,StreamingContext context)
		{
			this.i = info.GetInt32("i");
			this.s = info.GetString("s");
		}
		public U1(int i,string s)
		{
			this.i = i;
			this.s = s;
		}
		public virtual void incI()
		{
			TestBase.RegisterCall(ref testCall,"U1.incI()");
			if(bThrowException) throw new TestBase.TestException();
			++i;
		}
		void ISerializable.GetObjectData(SerializationInfo info,StreamingContext context)
		{
			info.AddValue("i",i);
			info.AddValue("s",s);
		}
	}
	/// <summary>
	/// Class for testing serialization of static jpinterwoven objects with three aspects 
	/// that implement ISerializable.
	/// </summary>
	[Serializable]
	[ObjectSerializationTestBase.BeforeAspect1]
	[ObjectSerializationTestBase.BeforeAspect2]
	[ObjectSerializationTestBase.BeforeAspect3]
	public class U2:TestBase.ClassBase,ISerializable
	{
		public int i = 0;
		public string s = "";
		public U2(SerializationInfo info,StreamingContext context)
		{
			this.i = info.GetInt32("i");
			this.s = info.GetString("s");
		}
		public U2(int i,string s)
		{
			this.i = i;
			this.s = s;
		}
		public virtual void incI()
		{
			TestBase.RegisterCall(ref testCall,"U2.incI()");
			if(bThrowException) throw new TestBase.TestException();
			++i;
		}
		void ISerializable.GetObjectData(SerializationInfo info,StreamingContext context)
		{
			info.AddValue("i",i);
			info.AddValue("s",s);
		}
	}
	/// <summary>
	/// ObjectSerializationInvokeBefore contains all tests for serialization of objects 
	/// with Serializable Attribute that are jpinterwoven with Advice.Before.
	/// </summary>
	[TestClass]
	public class ObjectSerializationSerializableAttribute:ObjectSerializationTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>
		/// Test with one Aspect.
		/// </summary>
		[TestMethod]
		public void TestObjectSerializationOfOneAspect()
		{
			BeforeAspect1 ta = new BeforeAspect1();
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			TestBase.T t = (TestBase.T)Weaver.CreateInstance(typeof(TestBase.T),args,ta);
			t.incI();
			Assert.AreEqual(4,t.i,"method not executed");
			Assert.AreEqual(1,ta.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,t.testCall,"object not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, t);
			stream.Seek(0,0);            
            TestBase.T t1 = (TestBase.T)formatter.Deserialize(stream);
            BeforeAspect1 ta1 = (BeforeAspect1)Weaver.GetAspects(t1, typeof(BeforeAspect1))[0];
            stream.Close();
            Assert.AreEqual(t.i, t1.i, "object serialization failed");
            Assert.AreEqual(t.s, t1.s, "object serialization failed");
            Assert.AreEqual(t.testCall, t1.testCall, "object serialization failed");
            t1.testCall = 0;
            ta1.aspectTestBefore = 0;
            callorder = 1;
            t1.incI();
            Assert.AreEqual(5, t1.i, "method not executed");
            Assert.AreEqual(1, ta1.aspectTestBefore, "Aspect not jpinterwoven");
            Assert.AreEqual(2, t1.testCall, "object not jpinterwoven");            
		}
		/// <summary>
		/// Test with three aspects.
		/// </summary>
		[TestMethod]
		public void TestObjectSerializationOfThreeAspects()
		{
			BeforeAspect1 ba1 = new BeforeAspect1();
			BeforeAspect2 ba2 = new BeforeAspect2();
			BeforeAspect3 ba3 = new BeforeAspect3();
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			AspectAttribute[] aspects = new AspectAttribute[3];
			aspects[0] = ba1;
			aspects[1] = ba2;
			aspects[2] = ba3;
			TestBase.T t = (TestBase.T)Weaver.CreateInstance(typeof(TestBase.T),args,aspects);
			t.incI();
			Assert.AreEqual(4,t.i,"method not executed");
			Assert.AreEqual(3,ba1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ba2.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,ba3.aspectTestBefore,"object not jpinterwoven");
			Assert.AreEqual(4,t.testCall,"Aspect not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, t);
			stream.Seek(0,0);
			TestBase.T t1 = (TestBase.T)formatter.Deserialize(stream);
			BeforeAspect1 ba4 = (BeforeAspect1)Weaver.GetAspects(t1,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba5 = (BeforeAspect2)Weaver.GetAspects(t1,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba6 = (BeforeAspect3)Weaver.GetAspects(t1,typeof(BeforeAspect3))[0];
			stream.Close();
			Assert.AreEqual(t.i,t1.i,"object serialization failed");
			Assert.AreEqual(t.s,t1.s,"object serialization failed");
			Assert.AreEqual(t.testCall,t1.testCall,"object serialization failed");
			t1.testCall = 0;
			ba4.aspectTestBefore = 0;
			ba5.aspectTestBefore = 0;
			ba6.aspectTestBefore = 0;
			callorder = 1;
			t1.incI();
			Assert.AreEqual(5,t1.i,"method not executed");
			Assert.AreEqual(3,ba4.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ba5.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,ba6.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,t1.testCall,"object not jpinterwoven");
		}
	}
	/// <summary>
	/// ObjectSerializationISerializable contains all tests for serialization of objects 
	/// that implements ISerializable and are jpinterwoven with Advice.Before.
	/// </summary>
	[TestClass]
	public class ObjectSerializationISerializable:ObjectSerializationTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		/// <summary>
		/// Test with one Aspect.
		/// </summary>
		[TestMethod]
		public void TestWithOneAspect()
		{
            
            BeforeAspect1 ta = new BeforeAspect1();
            object[] args = new object[2];
            args[0] = 3;
            args[1] = "Rapier-Loom.Net";
            TestBase.U u = (TestBase.U)Weaver.CreateInstance(typeof(TestBase.U), args, ta);
            u.incI();
            Assert.AreEqual(4, u.i, "method not executed");
            Assert.AreEqual(1, ta.aspectTestBefore, "Aspect not jpinterwoven");
            Assert.AreEqual(2, u.testCall, "object not jpinterwoven");
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, u);
            stream.Seek(0, 0);
            TestBase.U u1 = (TestBase.U)formatter.Deserialize(stream);

            Aspect[] aspects= Weaver.GetAspects(u1, typeof(BeforeAspect1));
            Assert.AreEqual(1, aspects.Length);
            BeforeAspect1 ta1 = (BeforeAspect1) aspects[0];
            stream.Close();
            Assert.AreEqual(u.i, u1.i, "object serialization failed");
            Assert.AreEqual(u.s, u1.s, "object serialization failed");
            //Assert.AreEqual(u.testCall,u1.testCall,"object serialization failed");
            callorder = 1;
            u1.testCall = 0;
            ta1.aspectTestBefore = 0;
            u1.incI();
            Assert.AreEqual(5, u1.i, "method not executed");
            Assert.AreEqual(1, ta1.aspectTestBefore, "Aspect not jpinterwoven");
            Assert.AreEqual(2, u1.testCall, "object not jpinterwoven");
		}
		/// <summary>
		/// Test with three aspects.
		/// </summary>
		[TestMethod]
		public void TestWithThreeAspects()
		{
			BeforeAspect1 ba1 = new BeforeAspect1();
			BeforeAspect2 ba2 = new BeforeAspect2();
			BeforeAspect3 ba3 = new BeforeAspect3();
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			AspectAttribute[] aspects = new AspectAttribute[3];
			aspects[0] = ba1;
			aspects[1] = ba2;
			aspects[2] = ba3;
			TestBase.U u = (TestBase.U)Weaver.CreateInstance(typeof(TestBase.U),args,aspects);
			u.incI();
			Assert.AreEqual(4,u.i,"method not executed");
			Assert.AreEqual(3,ba1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ba2.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,ba3.aspectTestBefore,"object not jpinterwoven");
			Assert.AreEqual(4,u.testCall,"Aspect not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, u);
			stream.Seek(0,0);
			TestBase.U u1 = (TestBase.U)formatter.Deserialize(stream);
			BeforeAspect1 ba4 = (BeforeAspect1)Weaver.GetAspects(u1,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba5 = (BeforeAspect2)Weaver.GetAspects(u1,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba6 = (BeforeAspect3)Weaver.GetAspects(u1,typeof(BeforeAspect3))[0];
			stream.Close();
			Assert.AreEqual(u.i,u1.i,"object serialization failed");
			Assert.AreEqual(u.s,u1.s,"object serialization failed");
			//Assert.AreEqual(u.testCall,u1.testCall,"object serialization failed");
			callorder = 1;
			u1.testCall = 0;
			ba4.aspectTestBefore = 0;
			ba5.aspectTestBefore = 0;
			ba6.aspectTestBefore = 0;
			u1.incI();
			Assert.AreEqual(5,u1.i,"method not executed");
			Assert.AreEqual(3,ba4.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,ba5.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(1,ba6.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,u1.testCall,"object not jpinterwoven");
		}
	}
	/// <summary>
	/// StaticInterweavingSerializableAttribute contains all tests for serialization of objects
	/// with Serializable Attribute that are jpinterwoven statically with Advice.Before.
	/// </summary>
	[TestClass]
	public class StaticInterweavingSerializableAttribute:ObjectSerializationTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void SerializationOfOneAspect()
		{
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			T1 t = (T1)Weaver.CreateInstance(typeof(T1),args);
			t.incI();
			Assert.AreEqual(4,t.i,"method not executed");
			Assert.AreEqual(2,t.testCall,"Aspect not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, t);
			stream.Seek(0,0);
			T1 t1 = (T1)formatter.Deserialize(stream);
			BeforeAspect1 a1 = (BeforeAspect1)Weaver.GetAspects(t1,typeof(BeforeAspect1))[0];
			stream.Close();
			Assert.AreEqual(t.i,t1.i,"serialization failed");
			Assert.AreEqual(t.s,t1.s,"serialization failed");
			Assert.AreEqual(t.testCall,t1.testCall,"serialization failed");
			callorder = 1;
			t1.testCall = 0;
			a1.aspectTestBefore = 0;
			t1.incI();
			Assert.AreEqual(5,t1.i,"method not executed");
			Assert.AreEqual(1,a1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,t1.testCall,"object not jpinterwoven");
		}
		[TestMethod]
		public void SerializatioOfThreeAspects()
		{
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			T2 t = (T2)Weaver.CreateInstance(typeof(T2),args);
			t.incI();
			Assert.AreEqual(4,t.i,"method not executed");
			Assert.AreEqual(4,t.testCall,"object not jpinterwoven");
			BeforeAspect1 ba1 = (BeforeAspect1)Weaver.GetAspects(t,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba2 = (BeforeAspect2)Weaver.GetAspects(t,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba3 = (BeforeAspect3)Weaver.GetAspects(t,typeof(BeforeAspect3))[0];
			Assert.IsTrue(0!=ba1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.IsTrue(0!=ba2.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.IsTrue(0!=ba3.aspectTestBefore,"Aspect not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, t);
			stream.Seek(0,0);
			T2 t1 = (T2)formatter.Deserialize(stream);
			BeforeAspect1 ba4 = (BeforeAspect1)Weaver.GetAspects(t1,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba5 = (BeforeAspect2)Weaver.GetAspects(t1,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba6 = (BeforeAspect3)Weaver.GetAspects(t1,typeof(BeforeAspect3))[0];
			stream.Close();
			Assert.AreEqual(t.i,t1.i,"object serialization failed");
			Assert.AreEqual(t.s,t1.s,"object serialization failed");
			Assert.AreEqual(t.testCall,t1.testCall,"object serialization failed");
			t1.testCall = 0;
			ba4.aspectTestBefore = 0;
			ba5.aspectTestBefore = 0;
			ba6.aspectTestBefore = 0;
			callorder = 1;
			t1.incI();
			Assert.AreEqual(5,t1.i,"method not executed");
			// sollte dieselbe Reihenfolge sein wie vor dem Verweben
			Assert.AreEqual(ba1.aspectTestBefore,ba4.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(ba2.aspectTestBefore,ba5.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(ba3.aspectTestBefore,ba6.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,t1.testCall,"object not jpinterwoven");
		}
	}
	/// <summary>
	/// StaticInterweavingISerializable contains all test for serialization of objects
	/// that implements ISerializable and are jpinterwoven statically with Advice.Before.
	/// </summary>
	[TestClass]
	public class StaticInterweavingISerializable:ObjectSerializationTestBase
	{
		[TestInitialize]
		public void Init()
		{
			base.SetUp();
			callorder=1;
		}
		[TestMethod]
		public void SerializationOfOneAspect()
		{
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			U1 u = (U1)Weaver.CreateInstance(typeof(U1),args);
			u.incI();
			Assert.AreEqual(4,u.i,"method not executed");
			Assert.AreEqual(2,u.testCall,"object not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, u);
			stream.Seek(0,0);
			U1 u1 = (U1)formatter.Deserialize(stream);
			BeforeAspect1 ta1 = (BeforeAspect1)Weaver.GetAspects(u1,typeof(BeforeAspect1))[0];
			stream.Close();
			Assert.AreEqual(u.i,u1.i,"object serialization failed");
			Assert.AreEqual(u.s,u1.s,"object serialization failed");
			//Assert.AreEqual(u.testCall,u1.testCall,"object serialization failed");
			callorder = 1;
			u1.testCall = 0;
			ta1.aspectTestBefore = 0;
			u1.incI();
			Assert.AreEqual(5,u1.i,"method not executed");
			Assert.AreEqual(1,ta1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(2,u1.testCall,"object not jpinterwoven");
		}
		[TestMethod]
		public void SerializatioOfThreeAspects()
		{
			object[] args = new object[2];
			args[0] = 3;
			args[1] = "Rapier-Loom.Net";
			U2 u = (U2)Weaver.CreateInstance(typeof(U2),args);
			u.incI();
			Assert.AreEqual(4,u.i,"method not executed");
			Assert.AreEqual(4,u.testCall,"object not jpinterwoven");
			BeforeAspect1 ba1 = (BeforeAspect1)Weaver.GetAspects(u,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba2 = (BeforeAspect2)Weaver.GetAspects(u,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba3 = (BeforeAspect3)Weaver.GetAspects(u,typeof(BeforeAspect3))[0];
			// Aufrufreihenfolge ist nicht klar bei statischen ASpekten
			Assert.IsTrue(0!=ba1.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.IsTrue(0!=ba2.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.IsTrue(0!=ba3.aspectTestBefore,"Aspect not jpinterwoven");
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize(stream, u);
			stream.Seek(0,0);
			U2 u1 = (U2)formatter.Deserialize(stream);
			BeforeAspect1 ba4 = (BeforeAspect1)Weaver.GetAspects(u1,typeof(BeforeAspect1))[0];
			BeforeAspect2 ba5 = (BeforeAspect2)Weaver.GetAspects(u1,typeof(BeforeAspect2))[0];
			BeforeAspect3 ba6 = (BeforeAspect3)Weaver.GetAspects(u1,typeof(BeforeAspect3))[0];
			stream.Close();
			Assert.AreEqual(u.i,u1.i,"object serialization failed");
			Assert.AreEqual(u.s,u1.s,"object serialization failed");
			//Assert.AreEqual(u.testCall,u1.testCall,"object serialization failed");
			callorder = 1;
			u1.testCall = 0;
			ba4.aspectTestBefore = 0;
			ba5.aspectTestBefore = 0;
			ba6.aspectTestBefore = 0;
			u1.incI();
			Assert.AreEqual(5,u1.i,"method not executed");
			// sollte dieselbe Reihenfolge sein wie vor dem Verweben
			Assert.AreEqual(ba1.aspectTestBefore,ba4.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(ba2.aspectTestBefore,ba5.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(ba3.aspectTestBefore,ba6.aspectTestBefore,"Aspect not jpinterwoven");
			Assert.AreEqual(4,u1.testCall,"object not jpinterwoven");
		}
	}
}
