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

namespace AfterReturingAndAfterThrowing
{
    /// <summary>
    /// This example shows how to interweave an aspect with a target class.
    /// An aspect is called every time the SayHello method returns and throws an exception.
    /// The example shows how to modify return values from target methods and use an aspect as an exception handler.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // aspect which will be interwoven on target class
            MyAspect myAspect = new MyAspect();
            
            // target class on which the aspect will be interwoven
            MyClass myObject = Weaver.Create<MyClass>(myAspect);

            Console.WriteLine("Saying hello to Peter...");
            Console.WriteLine(myObject.SayHello("Peter"));

            Console.WriteLine("Saying hello to nobody...");
            // invoking the interwoven method will call the aspect 
            // after an exception is thrown in the target method
            Console.WriteLine(myObject.SayHello(""));

            Console.WriteLine("Saying hello to Paul...");
            // invoking the interwoven method will call the aspect 
            // after the target method returns
            Console.WriteLine(myObject.SayHello("Paul"));

            Console.Read();
        }


    }
}
