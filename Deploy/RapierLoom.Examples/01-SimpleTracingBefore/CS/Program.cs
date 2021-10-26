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

namespace SimpleTracingBefore
{
    /// <summary>
    /// This example shows how to interweave a simple tracing aspect with a target class.
    /// A simple tracing aspect is called every time before the SayHello method is called on an instance of MyClass.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // tracing aspect which will be interwoven on target class
            TracingAspectBefore traceBeforeAspect = new TracingAspectBefore();
            
            // target class on which the tracing aspect will be interwoven
            MyClass myObject = Weaver.Create<MyClass>(traceBeforeAspect);
            
            // invoking the interwoven method will call the tracing aspect before the target method is called
            myObject.SayHello("Peter");

            Console.Read();

        }

      
    }
}
