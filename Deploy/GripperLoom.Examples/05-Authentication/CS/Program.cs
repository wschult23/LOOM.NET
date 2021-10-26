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

namespace Authentication
{
    class Program
    {
        static void Main(string[] args)
        {
            // create a new object instance
            MyClass m = new MyClass();

            try
            {
                // perform simple operation
                Console.WriteLine("Printing list:");
                m.PrintList();

                // perform operation that needs authentication first
                Console.WriteLine("Adding items to list:");
                m.Add("Entry 1");
                m.Add("100");
                m.Add(Guid.NewGuid());

                Console.WriteLine("Printing list:");
                m.PrintList();
            }
            catch (AuthenticationException exc)
            {
                Console.WriteLine(exc.Message);
            }

            Console.Read();
        }
    }
}
