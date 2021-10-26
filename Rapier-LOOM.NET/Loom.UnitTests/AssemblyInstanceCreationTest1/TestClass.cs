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

[assembly: Loom.UnitTests.InstanceCreation.Test1.PerClassAspect]
[assembly: Loom.UnitTests.InstanceCreation.Test1.PerClassAspectVirtual]

namespace Loom.UnitTests.InstanceCreation.Test1
{
    [PerClassAspect]
    [PerClassAspectVirtual]
    public class ABase
    {
        [PerClassAspect]
        [PerClassAspectVirtual]
        public virtual void foo()
        {
        }

        [PerClassAspect]
        [PerClassAspectVirtual]
        public virtual void bar()
        {
        }
    }

    public class A:ABase
    {
        [PerClassAspect]
        [PerClassAspectVirtual]
        public override void foo()
        {
        }

        public override void bar()
        {
        }
    }

    public class B
    {
        [PerClassAspect]
        [PerClassAspectVirtual]
        public virtual void foo()
        {
        }

        [PerClassAspect]
        [PerClassAspectVirtual]
        public virtual void bar()
        {
        }
    }
}
