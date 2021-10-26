// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTracingInstead
{
    /// <summary>
    /// The target class on which the tracing aspect will be interwoven implement a base interface that will be interwoven via configuration(see config.xml).
    /// </summary>
    public class MyClass : IMyClass
    {
        public void SayHello(string name)
        {
            Console.WriteLine("Hello {0}.", name);
        }

        public string GetHello(string name)
        {
            return "Hello " + name + ".";
        }

        public string GetHello(string firstname, string lastname)
        {
            return "Hello " + firstname + " " + lastname + ".";
        }
    }
}
