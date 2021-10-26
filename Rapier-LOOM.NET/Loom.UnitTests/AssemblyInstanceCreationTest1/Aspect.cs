// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom.AspectProperties;
using Loom.JoinPoints;

namespace Loom.UnitTests.InstanceCreation.Test1
{
    [CreateAspect(Per.ClassAndAnnotation)]
    public class PerClassAspect : AspectAttribute
    {
        public int flag = 0;

        [Call(Advice.Before)]
        public void foo()
        {
            flag++;
        }

        [Call(Advice.Before)]
        public void bar()
        {
            flag++;
        }
    }

    [CreateAspect(Per.Class)]
    public class PerClassAspectVirtual : AspectAttribute
    {
        public int flag = 0;

        [Call(Advice.Before)]
        public void foo()
        {
            flag++;
        }

        [Call(Advice.Before)]
        public void bar()
        {
            flag++;
        }
    }
}
