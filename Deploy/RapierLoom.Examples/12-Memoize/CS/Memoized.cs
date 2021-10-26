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
using System.Reflection;


namespace Memoize
{
    public class Memoized : AspectAttribute
    {
        private Dictionary<ArgArray, object> cache = new Dictionary<ArgArray, object>();   // internal dictionary with hashkey and method result for fast access

        [Call(Advice.Around)]   // aspect is called instead the target method
        [IncludeAll]            // method name of the target method doesn't matter, but signature must match
        public T Memoizer<T>([JPContext] Context ctx, params object[] args) // params object[] indicates the usage of wildcards, matches every parameter signature
        {
            ArgArray newArray = new ArgArray(args);     // create helper object, for parameter comparison
            object result;

            if (!cache.TryGetValue(newArray, out result)) // lookup dictionary for cached results
            {
                // dictionary doesn't contain result for this method and parameter values, so invoke the originally called target method
                result = ctx.Invoke(args);
                cache.Add(newArray, result);  // add the result to the cache
            }

            return (T)result;
        }
    }

    // helper class
    internal class ArgArray
    {
        private object[] argArray;

        public ArgArray(object[] args)
        {
            this.argArray = args;
        }

        public override bool Equals(object obj)
        {
            // cast object to object array
            ArgArray comparedObject = obj as ArgArray;
            if (comparedObject == null) return false;

            // compare the array lengths
            if (comparedObject.argArray.Length == this.argArray.Length)
            {
                for (int i = 0; i < argArray.Length; i++)
                {
                    // check each argument
                    if (!argArray[i].Equals(comparedObject.argArray[i]))
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            int hashcode = 0;

            // calculate new hashcode for argument array
            foreach (object arg in this.argArray)
            {
                hashcode += arg.GetHashCode();
            }

            return hashcode;
        }
    }
}
