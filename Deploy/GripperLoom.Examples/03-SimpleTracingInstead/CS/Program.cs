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

namespace SimpleTracingInstead
{
    /// <summary>
    /// This example shows how to interweave a simple tracing aspect with a target class.
    /// A simple tracing aspect is called every time before and after the SayHello method is called on an instance of MyClass.
    /// Different aspect methods are called before the target method, matching the signature of the target methods.
    /// A tracing aspect is called instead of the SayHello(string name) method.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
           
            // target class on which the tracing aspect will be interwoven
            IMyClass myObject = new MyClass();
            
            // invoking the interwoven method will call a tracing aspect before and another aspect instead of the target method
            myObject.SayHello("Peter");

            
            string greetings;

            // invoking the interwoven method will call a tracing aspect before the target method named Foo2
            greetings = myObject.GetHello("Peter");
            Console.WriteLine(greetings);

            // invoking the interwoven method will call a tracing aspect before the target method named Foo3
            greetings = myObject.GetHello("Peter", "Piper");
            Console.WriteLine(greetings);

            Console.Read();
        }

      
    }
}
