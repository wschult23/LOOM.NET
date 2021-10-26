// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom.JoinPoints;
using Loom.AspectProperties;

namespace Loom.UnitTests.providerInstanceCreationTest3
{
    [CreateAspect(Per.Annotation)]
    public class PerAttributeAspect : AspectAttribute
    {
        public int flag = 0;

        [Call(Advice.Before)]
        [IncludeAll]
        public void foo(params object[] args)
        {
            flag++;
        }
    }
}
