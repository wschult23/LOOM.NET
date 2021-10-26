﻿// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

using Loom;

namespace Memoize
{
    public class Fibonacci
    {
        /// <summary>
        /// Calculates the n'th fibonacci number
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [Memoized]  // mark this method to become interwoven with the Memoized aspect
        public virtual ulong Calculate(uint x)
        {
            if (x < 2)
                return x;
            else
                return Calculate(x - 1) + Calculate(x - 2);
        }
    }

    /// <summary>
    /// This example shows how to implement a simple caching mechanism for your application methods.
    /// The Memoized aspect caches calculated results of interwoven target methods and stores them into an internal dictionary.
    /// Recalling the method wit the same parameter values will return the previously calculated result.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            uint fibno = 42;
            DateTime start;
            ulong result;
            DateTime end;
            TimeSpan span;

            // first calculate w/o aspect
            Fibonacci t = new Fibonacci();

            start = DateTime.Now;
            Console.WriteLine("Calculating without aspect...");
            result = t.Calculate(fibno);
            end = DateTime.Now;
            span = end - start;
            Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds);

            // create a new object using the Weaver
            t = Weaver.Create<Fibonacci>();

            start = DateTime.Now;
            Console.WriteLine("Calculating with aspect...");
            result = t.Calculate(fibno);
            end = DateTime.Now;
            span = end - start;
            Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds);

            fibno +=5;
            start = DateTime.Now;
            Console.WriteLine("Calculating again...");
            result = t.Calculate(fibno);  // this should cause a cache hit
            end = DateTime.Now;
            span = end - start;
            Console.WriteLine("{0}. fibonacci number is {1}. Calculated in {2} ms.", fibno, result, span.TotalMilliseconds);
            
            Console.ReadKey();
        }
    }
}
