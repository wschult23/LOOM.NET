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

namespace TimeoutExample
{
    public class A
    {
        [Timeout(1000)]
        public virtual int LongOperation(int i)
        {
            DateTime dt = DateTime.Now;
            TimeSpan ts;
            do
            {
                ts = DateTime.Now - dt;
            }
            while (ts.TotalMilliseconds < i);

            return i;
        }
    }

    /// <summary>
    /// This example shows how to implement a timeout aspect to define runtime constraints to methods.
    /// If a method exceeds its defined quota, it is aborted and an exception is thrown.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            A a = Weaver.Create<A>();
            CallOperation(a, 200);  // perform an operation which finishes in about 200ms
            CallOperation(a, 2000);  // should be timed out
            CallOperation(a, 20000);  // should be timed out
            Console.ReadKey();
        }

        private static void CallOperation(A a, int simulatedtimeinprogress)
        {
            DateTime dt = DateTime.Now;
            try
            {
                a.LongOperation(simulatedtimeinprogress);
                TimeSpan ts = DateTime.Now - dt;
                Console.WriteLine("Method returns after {0} ms", ts.TotalMilliseconds);
            }
            catch (TimeoutException)
            {
                TimeSpan ts = DateTime.Now - dt;
                Console.WriteLine("Timeout after {0} ms", ts.TotalMilliseconds);
            }
        }
    }
}
