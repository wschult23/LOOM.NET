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
using Loom.UnitTests.providerInstanceCreationTest2;

[assembly: PerInstanceAspect]

namespace Loom.UnitTests.providerInstanceCreationTest2
{
    public class A
    {
        public virtual void foo()
        {
        }
    }

    [TestFixture]
    public class InstanceTest
    {
        [Test]
        public void PerInstanceproviderTest()
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
    }
}
