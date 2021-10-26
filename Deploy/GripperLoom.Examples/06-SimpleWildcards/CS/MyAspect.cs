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
using System.Runtime.Serialization;

namespace SimpleWildcards
{
    /// <summary>
    /// This aspect demonstrates the usage of wildcards.
    /// Every aspect has to inherit the AspectAttribute.
    /// </summary>
    public class MyAspect : AspectAttribute
    {
        /// <summary>
        /// This aspect shows the usage of wildcards with generic type parameters. T is used as a wildcard, so any type except void is matched.
        /// </summary>
        /// <typeparam name="T">is used as parameter and return type - only methods with same parameter and return type will be matched</typeparam>
        /// <param name="t">indicates that all methods with one paramter of any type</param>
        /// <returns>indicates that all methods with a return parameter of any type except void are matched</returns>
        [IncludeAll]            // include every target method
        [Call(Advice.Around)]   // call aspect instead of the targets method
        public T foo<T>(T t)
        {
            Console.WriteLine("Aspect: Type in aspect was '{0}' with value '{1}'.", typeof(T), t.ToString());
            return default(T);
        }
    }
}
