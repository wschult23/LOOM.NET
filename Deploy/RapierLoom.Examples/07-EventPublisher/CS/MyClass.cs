// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace EventPublisher
{
    [EventPublisher] // marked to be interwoven with EventPublisher aspect
    public class MyClass
    {
        public virtual void Add(object item)
        {
        }
    }
}
