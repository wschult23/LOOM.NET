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
using System.Threading;
using Loom;

namespace AsyncronousExample
{
    /// <summary>
    /// This class demonstrates the usage of the Asyncronous aspect
    /// </summary>
    public class MyClass
    {
        public ManualResetEvent evt = new ManualResetEvent(false);
        public volatile int i;

        /// <summary>
        /// If a method is marked as Asynchronous, it has to return a Future object
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [Asynchronous]
        public virtual Future<int> LongRunningOperation(int i)
        {
            Thread.SpinWait(200000000);
            return i;
        }

        /// <summary>
        /// Or it has to be void
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [Asynchronous]
        public virtual void FireAndForget(int i)
        {
            Thread.SpinWait(500000000);
            this.i = i;
            evt.Set();
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            var c = Weaver.Create<MyClass>();
            
            Console.WriteLine("Start LongRunningOperation 1");
            var r1=c.LongRunningOperation(1);

            r1.ContinueWith(i => c.FireAndForget(i));

            Console.WriteLine("Start LongRunningOperation 2");
            var r2 = c.LongRunningOperation(2);
            Console.WriteLine("Start LongRunningOperation 3");
            var r3 = c.LongRunningOperation(3);

            Console.WriteLine("Access results of LongRunningOperation 2 and 3");
            Console.WriteLine("LongRunningOperation {0} and {1} finished", r2, r3);

            Console.WriteLine("Start LongRunningOperation 4");
            var r4 = c.LongRunningOperation(4);
            Console.WriteLine("Start LongRunningOperation 5");
            var r5 = c.LongRunningOperation(5);

            Console.WriteLine("Wait for LongRunningOperation 4");
            r4.Wait();
            Console.WriteLine("Wait for LongRunningOperation 5");
            r5.Wait();
            Console.WriteLine("LongRunningOperation {0} and {1} finished", r4.Result, r5.Result);

            Console.WriteLine("Wait for FireAndForget");
            c.evt.WaitOne();
            Console.WriteLine("LongRunningOperation {0} and FireAndForget finished", c.i);

        }
    }
}
