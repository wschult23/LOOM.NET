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
using Test.Lib;
using System.Collections;
using System.EnterpriseServices;

namespace Test
{
    [EventClass]
    public class ES
    {
    }

    class Program
    {
        public class Ar : ArrayList
        {
        }

        static void Main(string[] args)
        {
            IList list = new ArrayList();
            list.Add(25);

            var a1 = new A(1,"test");
            a1.foo();
            var a2 = new A(2);
            a2.foo();
            Console.ReadKey();
        }
    }
}
