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

namespace EventConsumerExample
{
    public class A: IDisposable
    {
        [CustomEventOne] // mark this target method to become interwoven with EventConsumer aspect
        public virtual void foo(Event eventargs)
        {
            Console.WriteLine("foo called: {0}",eventargs);
        }

        public void Dispose()
        {
        }
    }

    public class B:IDisposable
    {
        [CustomEventOne] // mark this target method to become interwoven with EventConsumer aspect
        [CustomEventTwo]
        public virtual void bar(Event eventargs)
        {
            Console.WriteLine("bar called: {0}",eventargs);
        }
        public void Dispose()
        {
        }
    }

    /// <summary>
    /// This example shows how to implement an EventConsumer aspect. The EventConsumer aspect stores target methods
    /// to become fired using a single Fire() method.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // create my class objects
            using (A a = new A())
            {
                using (B b = new B())
                {

                    // calls all marked Event methods; even from different objects!
                    Console.WriteLine("Firing CustomEventOne:");
                    new CustomEventOne().Fire();
                    Console.WriteLine("\nFiring CustomEventTwo:");
                    new CustomEventTwo().Fire();
                }
            }

            Console.ReadKey();
        }
    }
}
