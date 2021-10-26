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
using Loom.AspectProperties;
using Loom.JoinPoints;
using Loom;

namespace Test.Aspects
{
    [CreateAspect(Per.InstanceAndAnnotation)]
    public class MyAspect : AspectAttribute
    {
        [Call(Advice.Before), IncludeAll]
        public void foo([JPContext] Context ctx)
        {
            Console.WriteLine("{0},{1} -> {2}", c, i, ctx.CurrentMethod.Name);
        }

        static int c_c = 0;
        int c;
        int i;
        public MyAspect(int i)
        {
            this.c = c_c++;
            this.i = i;
            Console.WriteLine("MyAspect {0}", c);
        }
    }

    public class TestList : AspectAttribute
    {
        [Call(Advice.Before), IncludeAll]
        public void foo<T>([JPContext] Context ctx, T obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
