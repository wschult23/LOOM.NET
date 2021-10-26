// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom;
using Loom.JoinPoints;
using Loom.AspectProperties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loom.UnitTests.InstanceCreation.Test1;

namespace Loom.UnitTests.InstanceCreation
{
    [InstanceCreationTestBase.PerInstanceAspect]
    public class A
    {
        public virtual void foo()
        {
        }
    }

    [InstanceCreationTestBase.PerClassAspect]
    public class B
    {
        public virtual void foo()
        {
        }
    }

    public class C : B
    {
    }

    [InstanceCreationTestBase.PerAttributeAspect]
    public class D
    {
        public virtual void foo()
        {
        }
    }

    public class E : D 
    {

    }

    public class InstanceCreationTestBase
    {
        [CreateAspectAttribute(Per.Instance)]
        public class PerInstanceAspect : AspectAttribute
        {
            public int flag = 0;

            [Call(Advice.Before)]
            public void foo()
            {
                flag++;
            }
        }

        [CreateAspectAttribute(Per.Class)]
        public class PerClassAspect : AspectAttribute
        {
            public int flag = 0;

            [Call(Advice.Before)]
            public void foo()
            {
                flag++;
            }
        }

        [CreateAspectAttribute(Per.Annotation)]
        public class PerAttributeAspect : AspectAttribute
        {
            public int flag = 0;

            [Call(Advice.Before)]
            public void foo()
            {
                flag++;
            }
        }
    }

    [TestClass]
    public class InstanceCreationClassTests : InstanceCreationTestBase
    {
        [TestMethod]
        public void PerInstanceTest()
        {
            A a1 = Weaver.Create<A>();
            A a2 = Weaver.Create<A>();

            Aspect[] aspects_a1 = ((IAspectInfo)a1).GetAspects();
            Aspect[] aspects_a2 = ((IAspectInfo)a2).GetAspects();

            PerInstanceAspect asp1 = (PerInstanceAspect)aspects_a1[0];
            PerInstanceAspect asp2 = (PerInstanceAspect)aspects_a2[0];
            
            asp1.flag++;

            Assert.AreNotEqual(asp1.flag, asp2.flag);

        }

        [TestMethod]
        public void PerClassTest()
        {
            B b1 = Weaver.Create<B>();
            b1.foo();
            B b2 = Weaver.Create<B>();
            b2.foo();
            C c = Weaver.Create<C>();
            c.foo();

            Aspect[] aspects_b1 = ((IAspectInfo)b1).GetAspects();
            Aspect[] aspects_b2 = ((IAspectInfo)b2).GetAspects();
            Aspect[] aspects_c = ((IAspectInfo)c).GetAspects();

            PerClassAspect asp1 = (PerClassAspect)aspects_b1[0];
            PerClassAspect asp2 = (PerClassAspect)aspects_b2[0];
            PerClassAspect asp3 = (PerClassAspect)aspects_c[0];


            Assert.AreEqual(asp1.flag, asp2.flag);
            Assert.AreNotEqual(asp2.flag, asp3.flag);
        }

        [TestMethod]
        public void PerAttriuteTest()
        {
            D d1 = Weaver.Create<D>();
            D d2 = Weaver.Create<D>();
            E e = Weaver.Create<E>();

            Aspect[] aspects_d1 = ((IAspectInfo)d1).GetAspects();
            Aspect[] aspects_d2 = ((IAspectInfo)d2).GetAspects();
            Aspect[] aspects_e = ((IAspectInfo)e).GetAspects();

            PerAttributeAspect asp1 = (PerAttributeAspect)aspects_d1[0];
            PerAttributeAspect asp2 = (PerAttributeAspect)aspects_d2[0];
            PerAttributeAspect asp3 = (PerAttributeAspect)aspects_e[0];

            asp1.flag++;

            Assert.AreEqual(asp1.flag, asp2.flag);
            Assert.AreEqual(asp2.flag, asp3.flag);
        }
    }

    [TestClass]
    public class AssemblyTests
    {
        [TestMethod]
        public void PerClassAssemblyTest()
        {
            var a1 = Weaver.Create<Loom.UnitTests.InstanceCreation.Test1.A>();
            var a2 = Weaver.Create<Loom.UnitTests.InstanceCreation.Test1.A>();
            var b1 = Weaver.Create<Loom.UnitTests.InstanceCreation.Test1.B>();

            Aspect[] aspects_a1 = ((IAspectInfo)a1).GetAspects(typeof(PerClassAspect));
            Aspect[] aspects_a2 = ((IAspectInfo)a2).GetAspects(typeof(PerClassAspect));

            Assert.AreEqual(4, aspects_a1.Length);

            PerClassAspect asp1 = (PerClassAspect)aspects_a1[0];
            PerClassAspect asp2 = (PerClassAspect)aspects_a2[0];

            asp1.flag=42;

            a1.foo();

            Assert.AreEqual(43, asp1.flag);
            Assert.AreEqual(43, asp2.flag);

            Aspect[] aspects_a3 = ((IAspectInfo)a1).GetAspects(typeof(PerClassAspectVirtual));
            Aspect[] aspects_a4 = ((IAspectInfo)a2).GetAspects(typeof(PerClassAspectVirtual));

            Assert.AreEqual(1, aspects_a4.Length);

            var asp3 = (PerClassAspectVirtual)aspects_a3[0];
            var asp4 = (PerClassAspectVirtual)aspects_a4[0];
        }
    }
}
