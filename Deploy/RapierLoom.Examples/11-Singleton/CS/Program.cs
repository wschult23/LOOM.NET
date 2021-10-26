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

namespace SingletonExample
{
    [Singleton] // applies the singleton pattern
    public class A
    {
        public A()
        {
            Console.WriteLine("Constructor of class A was executed.");
        }
    }

    public class B
    {
        public B()
        {
            Console.WriteLine("Constructor of class B was executed.");
        }
    }

    /// <summary>
    /// This example shows, how to apply the singleton pattern to target classes.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {

            A a1 = Weaver.Create<A>();
            A a2 = Weaver.Create<A>();  // returns already created a1 object

            Console.WriteLine(a1 == a2);

            B b1 = Weaver.Create<B>();  // normal behavior as expected
            B b2 = Weaver.Create<B>();

            Console.WriteLine(b1 == b2);

            Console.ReadKey();
        }
    }
}
