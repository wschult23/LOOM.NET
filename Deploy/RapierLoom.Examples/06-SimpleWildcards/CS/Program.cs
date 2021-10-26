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

namespace SimpleWildcards
{
    /// <summary>
    /// This example shows how to interweave an aspect with a target class using wildcards.
    /// The aspect method uses generic type parameters and return types to match multiple target methods.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            MyAspect aspect = new MyAspect();

            // interweave aspects
            MyClass targetClass = Weaver.Create<MyClass>(aspect);

            Console.WriteLine("Calling targets foo1 method...");
            targetClass.foo1("Hello foo1");

            Console.WriteLine("Calling targets foo2 method...");
            targetClass.foo2(2);

            Console.WriteLine("Calling targets foo3 method...");
            targetClass.foo3("Hello foo3");

            Console.ReadKey();
        }
    }
}
