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

namespace Monitoring
{
    class Program
    {
        static void Main(string[] args)
        {
            MonitoringAspect monAspect = new MonitoringAspect();

            // interweave monitoring aspect
            Library lib = Weaver.Create<Library>(monAspect);

            ulong number = 30;
            
            Console.WriteLine("Computing {0}. fibonacci number.", number);

            // monitoring aspect calculates the time of execution
            ulong result = lib.Fibonacci(number);

            Console.WriteLine("Result is {0}.", result);

            Console.Read();
        }
    }
}
