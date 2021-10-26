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
using Loom.JoinPoints;


namespace SimpleTracingBefore
{
    /// <summary>
    /// This is a simple tracing aspect. Every aspect has to inherit the AspectAttribute.
    /// </summary>
    public class TracingAspectBefore : AspectAttribute
    {
        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  return value doesn't matter; use [JPRetval] attribute to define specific return type
        ///  method name is exactly "SayHello"
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="name">the name of the parameter doesn't matter</param>
        [Call(Advice.Before)] // JoinPoint, which indicates that this aspect is called before the target method 
        public void SayHello(string name)
        {
            Console.WriteLine("This is before entering method SayHello.");
        }
    }
}
