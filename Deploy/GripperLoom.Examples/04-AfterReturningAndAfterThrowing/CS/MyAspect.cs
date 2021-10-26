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

namespace AfterReturingAndAfterThrowing
{
    /// <summary>
    /// This is a simple aspect to demonstrate the usage of Advice.AfterThrowing and Advice.AfterReturning JoinPoints.
    /// It introduces to the AspectParamters JPException and JPRetVal.
    /// Every aspect has to inherit the AspectAttribute.
    /// </summary>
    public class MyAspect : AspectAttribute
    {
        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  the return type is irrelevant - using AfterReturing JoinPoint it has to be generic
        ///  any method name, not Foo - see IncludeAll attribute
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="_exception">the JPExcption attribute marks an additional parameter, where the original thrown exception from the target method is passed</param>
        /// <param name="name"></param>
        [Call(Advice.AfterThrowing)] // JoinPoint, which indicates that this aspect is called after an exception is thrown inside the target method
        [IncludeAll]                 // includes all methods with the following signature and the any method name
        public T Foo<T>([JPException] Exception _exception, string name) 
        {
            Console.WriteLine("ERROR: An exception of type {0} occured.", _exception.GetType().FullName);
            Console.WriteLine("Details:\n{0}", _exception.Message);
            return default(T);
        }
        
        /// <summary>
        /// This aspect will be interwoven with methods matching this signature.
        ///  the return type is irrelevant - so it has to be void
        ///  any method name, not Foo - see IncludeAll attribute
        ///  one method parameter of type string and any name
        /// </summary>
        /// <param name="returnValue">
        /// The JPRetVal attribute marks an additional parameter, where the original return value from the target method is passed.
        /// Use the ref keyword if you want to overwrite the value.
        /// </param>
        /// <param name="name"></param>
        [Call(Advice.AfterReturning)] // JoinPoint, which indicates that this aspect is called after returning from the target method
        [IncludeAll]                  // includes all methods with the following signature and the any method name
        public void Foo2([JPRetVal] ref string returnValue, string name)
        {
            if (returnValue.Contains("Paul"))
                returnValue = "Don't want to say hello to Paul...";
        }
    }

}
