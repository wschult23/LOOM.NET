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
using Loom.AspectProperties;

namespace SingletonExample
{
    // use this attribute to indicate, that the aspect class in created only once
    [CreateAspect(Per.Class)]
    public class Singleton : AspectAttribute
    {
        private object obj = null;

        [Create(Advice.Around)]
        public T Create<T>([JPContext] Context ctx)
        {
            // if constructor already called return the stored object, otherwise call the constructor
            if (obj == null) obj = ctx.Call();
            return (T)obj;
        }

        [Error(TargetTypes.Create, "constructors with parameters are not allowed")]
        [IncludeAll]
        public void Error<T>(T first, params object[] following)
        {
        }
    }
}
