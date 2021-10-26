// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.JoinPoints;
using System.Collections;

namespace Loom.UnitTests.AspectConstructors
{
    /// <summary>SimpleWeavingTestBase contains all aspects for testing weaving with base types.</summary>

    public class TestAspect : AspectAttribute, IEqualityComparer<object>
    {
        public enum tenum { First, Second };
        public static int ctrorrun;
        public static IEnumerable testarray;
        public TestAspect(Boolean b, Byte bt, SByte sb, Int16 i16, UInt16 u16, Int32 i32, UInt32 u32, Int64 i64, UInt64 u64, Single si, Double db, tenum en, Type tp, char c, string s)
        {
            Assert.AreEqual(true, b);
            Assert.AreEqual(1, bt);
            Assert.AreEqual(-2, sb);
            Assert.AreEqual(-3, i16);
            Assert.AreEqual(4, u16);
            Assert.AreEqual(-5, i32);
            Assert.AreEqual(6u, u32);
            Assert.AreEqual(-7, i64);
            Assert.AreEqual(8u, u64);
            Assert.AreEqual(9.0f, si);
            Assert.AreEqual(-10.0, db);
            Assert.AreEqual(tenum.Second, en);
            Assert.AreEqual(typeof(TestAspect), tp);
            Assert.AreEqual('c', c);
            Assert.AreEqual("foo", s);
            ctrorrun++;
        }

        public TestAspect(object[] arr)
        {
            if (arr == null || testarray == null)
            {
                return;
            }
            Assert.IsTrue(testarray.Cast<object>().SequenceEqual(arr, this));
            ctrorrun++;
        }


        public TestAspect(int[] arr)
        {
            if (arr == null || testarray == null)
            {
                return;
            }
            Assert.IsTrue(testarray.Cast<object>().SequenceEqual(arr.Cast<object>()));
            ctrorrun++;
        }

        public TestAspect(double[] arr)
        {
            if (arr == null || testarray == null)
            {
                return;
            }
            Assert.IsTrue(testarray.Cast<object>().SequenceEqual(arr.Cast<object>()));
            ctrorrun++;
        }

        public TestAspect(string[] arr)
        {
            if (arr == null || testarray == null)
            {
                return;
            }
            Assert.IsTrue(testarray.Cast<object>().SequenceEqual(arr.Cast<object>()));
            ctrorrun++;
        }

        [Call(Advice.Before),IncludeAll]
        public void foo(params object[] args)
        {
        }

        bool IEqualityComparer<object>.Equals(object x, object y)
        {
            var ex = x as IEnumerable;
            var ey = y as IEnumerable;
            if (ex == null && ey == null)
            {
                return x.Equals(y);
            }
            if (ex == null || ey == null)
            {
                return false;
            }
            return ex.Cast<object>().SequenceEqual(ey.Cast<object>(), this);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode() ;
        }
    }

    [TestAspect(true,1,-2,-3,4,-5,6,-7,8,9.0f,-10.0,TestAspect.tenum.Second,typeof(TestAspect),'c',"foo")]
    public class A
    {
    }

    [TestAspect(new int[] { 1, 2, 3, 4, 5, 6 })]
    public class B
    {
    }

    [TestAspect(new double[] { 1, 2, 3, 4, 5, 6 })]
    public class C
    {
    }

    [TestAspect(new string[] { "foo", "bar", "baz" })]
    public class D
    {
    }

    [TestAspect(new Object[] { "foo", 1, 1.0, TestAspect.tenum.Second, typeof(TestAspect) })]
    public class E
    {
    }

    [TestAspect(new Object[] { new Object[] { "foo", 1.0, 2 }, new int[] { 1, 2, 3}})]
    public class F
    {
    }

    public class G
    {
    }

    public class H
    {
    }
    
    [TestClass]
    public class AspectConstructors
    {
        [TestInitialize]
        public void SetUp()
        {
            TestAspect.ctrorrun = 0;
            TestAspect.testarray = null;
        }

        [TestMethod]
        public void TestPrimitives()
        {
            Weaver.Create<A>();
            Assert.AreEqual(1,TestAspect.ctrorrun);
        }

        [TestMethod]
        public void TestIntArray()
        {
            TestAspect.testarray = new int[] { 1, 2, 3, 4, 5, 6 };
            Weaver.Create<B>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }

        [TestMethod]
        public void TestDoubleArray()
        {
            TestAspect.testarray=new double[]{1,2,3,4,5,6};
            Weaver.Create<C>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }

        [TestMethod]
        public void TestStringArray()
        {
            TestAspect.testarray = new string[] { "foo", "bar", "baz" };
            Weaver.Create<D>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }
        
        [TestMethod]
        public void TestObjectArray()
        {
            TestAspect.testarray = new object[] { "foo", 1, 1.0, TestAspect.tenum.Second, typeof(TestAspect) };
            Weaver.Create<E>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }

        [TestMethod]
        public void TestNestedObjectArray()
        {
            TestAspect.testarray = new Object[] { new Object[] { "foo", 1.0, 2 }, new int[] { 1, 2, 3 } };
            Weaver.Create<F>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }

        [TestMethod,Ignore]
        public void TestConfiguredPrimitives()
        {
            Weaver.Create<G>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }

        [TestMethod, Ignore]
        public void TestConfiguredIntArray()
        {
            TestAspect.testarray = new int[] { 1, 2, 3, 4, 5, 6 };
            Weaver.Create<H>();
            Assert.AreEqual(1, TestAspect.ctrorrun);
        }
    }

}
