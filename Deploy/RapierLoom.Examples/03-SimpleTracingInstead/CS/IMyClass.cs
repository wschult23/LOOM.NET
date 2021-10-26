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
    /// This is the base class for the MyClass class.
    /// Every target object which should be interwoven has to implement a base interface.
    /// </summary>
    public interface IMyClass
    {
        void SayHello(string name);
        string GetHello(string name);
        string GetHello(string firstname, string lastname);
    }
}
