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
using Test.Aspects;

namespace Test.Lib
{
    [MyAspect(42)]
    [MyAspect(43)]
    public class A
    {
        int t;

        public A(int i)
        {
        }

        public A(string s)
        {
        }

        public A(int i, string s)
        {
            t = i;
        }


        public virtual void foo()
        {
            Console.WriteLine("World");
        }
    }
}
