// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleTracingBefore
{
    /// <summary>
    /// The target class on which the tracing aspect will be interwoven has to implement a base interface.
    /// The target methods have to be declared as virtual to become interwoven.
    /// </summary>
    public class MyClass : IMyClass
    {
        public virtual void SayHello(string name)
        {
            Console.WriteLine("Hello {0}.", name);
        }
    }
}
