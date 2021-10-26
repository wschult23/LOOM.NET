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

using Loom.ContextSharp;

namespace ContextSharpExample
{
    /// <summary>
    /// This is the implementation of the Employment layer
    /// </summary>
    public class Employment:Layer
    {
    }

    /// <summary>
    /// And the layer methods
    /// </summary>
    public partial class Person
    {
        public string ToString(Employment layer)
        {
            return layer.Proceed() + "; Employer: " + this.employer;
        }
    }
}
