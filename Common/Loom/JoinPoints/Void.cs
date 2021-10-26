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

namespace Loom.JoinPoints
{
    /// <summary>
    /// Specifies a return value type for a method that does not return a value.
    /// </summary>
    /// <remarks>
    /// You need this class if you want to specifiy join point that matches only target methods that don't return a value.
    /// </remarks>
    /// <example>
    /// The advice <c>VoidAdvice</c> will interweave only <c>void</c>-methods.
    /** 
    <code>
     public class MyAspect : AspectAttribute
     {
        [Call(Advice.Around), IncludeAll]
        public T VoidAdvice&lt;T&gt;([JPContext] Context ctx, params object[] args) where T : _Void
        {
            ctx.Invoke(args);
            return (T)_Void.Value;
        }
     }
    </code> */
    /// </example>
    /// <seealso cref="Aspect"/>
    /// <seealso cref="AspectAttribute"/>
    public class _Void
    {
        /// <summary>
        /// The Instance
        /// </summary>
        static _Void c_void=new _Void();

        /// <summary>
        /// Internal use
        /// </summary>
        private _Void()
        {
        }

        /// <summary>
        /// Specifies a return value  in advices for a method that does not return a value.
        /// </summary>
        public static _Void Value { get { return c_void; } }
    }
}
