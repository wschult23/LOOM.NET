// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom.JoinPoints;
using Loom;

namespace SimpleTracingInstead
{
    /// <summary>
    /// This is a simple tracing aspect. Every aspect has to inherit the AspectAttribute.
    /// </summary>
    public class TracingAspect : AspectAttribute
    {
        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  return value void
        ///  any method name, not Foo - see IncludeAll attribute
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="name"></param>
        [Call(Advice.Before)] // JoinPoint, which indicates that this aspect is called before the target method 
        [Include("SayHello")]        // includes all methods with the following signature and the method name "SayHello"
        public void Foo(string name) // the name Foo of the aspect method doesn't matter now
        {
            Console.WriteLine("This is before entering a method.");
        }

        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  return value void
        ///  method name is exactly "SayHello"
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="name">the name of the parameter doesn't matter</param>
        [Call(Advice.Around)] // JoinPoint, which indicates that this aspect is called instead of the target method 
        public void SayHello(string name) 
        {
            Console.WriteLine("This is not the SayHello method. But I know who you are {0}.", name);
        }

        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  return value string, indicated through [JPRetVal] attribute
        ///  any method name, not Foo - see IncludeAll attribute
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="retval">indicates, that target method has return parameter of type string</param>
        /// <param name="name"></param>
        [Call(Advice.Before)] // JoinPoint, which indicates that this aspect is called before the target method 
        [IncludeAll]          // includes all methods with the following signature and any method name
        public void Foo2([JPRetVal] string retval, string name) // the name Foo2 of the aspect method doesn't matter now
        {
            Console.WriteLine("This is before entering a method with the string parameter '{0}' which returns a string value.", name);
        }

        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  return value string, indicated through [JPRetVal] attribute
        ///  any method name, not Foo - see IncludeAll attribute
        ///  two method parameters of type string and any name
        /// </summary>
        /// <param name="retval">indicates, that target method has return parameter of type string</param>
        /// <param name="name"></param>
        [Call(Advice.Before)] // JoinPoint, which indicates that this aspect is called before the target method 
        [IncludeAll]          // includes all methods with the following signature and any method name
        public void Foo3([JPRetVal] string retval, string param1, string param2) // the name Foo3 of the aspect method doesn't matter now
        {
            Console.WriteLine("This is before entering a method with two string parameters '{0}', '{1}' which returns a string value.", param1, param2);
        }

    }
}
