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
    /// This is the implementation of the Address layer
    /// </summary>
    public class Address:Layer
    {
    }

    /// <summary>
    /// And the layer methods
    /// </summary>
    public partial class Employer
    {
        public string ToString(Address layer)
        {
            return layer.Proceed() + "; Address: " + this.address;
        }
    }

    /// <summary>
    /// And the layer methods
    /// </summary>
    public partial class Person
    {
        public string ToString(Address layer)
        {
            return layer.Proceed() + "; Address: " + this.address; ;
        }
    }
}
