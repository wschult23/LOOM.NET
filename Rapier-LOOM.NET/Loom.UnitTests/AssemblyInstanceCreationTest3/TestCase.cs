// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Loom;
using Loom.UnitTests.providerInstanceCreationTest3;

[assembly: PerAttributeAspect]

namespace Loom.UnitTestsInstanceCreationTest3
{
    [PerAttributeAspect]
    public class D
    {
        public virtual void foo()
        {
        }
    }

    public class E : D
    {

    }

    [TestFixture]
    public class InstanceTest
    {
        [Test]
        public void PerAttributeproviderTest()
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
}
