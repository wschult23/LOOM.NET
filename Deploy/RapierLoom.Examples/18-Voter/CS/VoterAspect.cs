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

namespace VotingExample
{
    [AttributeUsage(AttributeTargets.Class, Inherited=false)]
    public class VotingAspect:AspectAttribute
    {
        private Type[] versions;
        static object c_invalid = new object();

        public VotingAspect(params Type[] versions)
        {
            this.versions = versions;
        }

        [Create(Advice.After), IncludeAll]
        public void Create([JPVariable(Scope.Protected)] ref object[] instances, params object[] args)
        {
            instances = new object[versions.Length];
            for (int iPos = 0; iPos < versions.Length; iPos++)
            {
                instances[iPos] = Weaver.CreateInstance(versions[iPos], args);
            }
        }

        [Call(Advice.Around), IncludeAll]
        public T Call<T>([JPContext] Context ctx, [JPVariable(Scope.Protected)] object[] instances, params object[] args)
        {
            object res=c_invalid;
            int count;
            Dictionary<object, int> results = new Dictionary<object, int>(instances.Length);
            foreach (object instance in instances)
            {
                try
                {
                    res = ctx.InvokeOn(instance, args);
                  
                    if (results.TryGetValue(res, out count))
                    {
                        count++;
                        results[res] = count;
                    }
                    else
                    {
                        results.Add(res, 1);
                    }
                }
                catch (Exception) { }
            }
            count = 0;
            foreach(KeyValuePair<object,int> kvpair in results)
            {
                if (kvpair.Value > count)
                {
                    count = kvpair.Value;
                    res = kvpair.Key;
                }
                else if (kvpair.Value == count) res = c_invalid;
            }
            if (res == c_invalid) throw new ApplicationException();
            return (T)res;
        }
    }
}
