// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleWildcards
{
    /// <summary>
    /// The target class on which the aspect will be interwoven has to implement a base interface.
    /// The target methods have to be declared as virtual to become interwoven.
    /// </summary>
    [MyAspect]
    public class MyClass : IBase
    {
        public virtual string foo1(string s)
        {
            Console.WriteLine("Target Class: Was {0}.", s);
            return s;
        }

        public virtual int foo2(int i)
        {
            Console.WriteLine("Target Class: Was {0}.", i);
            return i;
        }

        public virtual int foo3(string s)
        {
            Console.WriteLine("Target Class: Was {0}.", s);
            return s.Length;
        }

    }
}
