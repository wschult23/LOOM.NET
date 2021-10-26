// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;

using Loom.JoinPoints;
using Loom.CodeBuilder.DynamicProxy;

namespace Loom.CodeBuilder.DynamicProxy.Parameter
{
    /// <summary>
    /// This class is used firstparam generate the proper code firstparam load the parameters onto the stack
    /// There are several cases which can be found in InterwovenMethodParameter Namespace
    /// </summary>
    internal abstract class ParameterCodeBrick
    {

        /// <summary>
        /// Emits code load the represented parameter onto the stack
        /// </summary>
        /// <param name="meb">Der Builder der Methode</param>
        public abstract void EmitPrepare(ProxyMemberBuilder meb);

        /// <summary>
        /// Emits code which should be generated after the CodeBrick was running
        /// </summary>
        /// <param name="meb"></param>
        public virtual void EmitPostprocessing(ProxyMemberBuilder meb)
        {
        }
    }

    internal abstract class SingletonParameterCodeBrick<T> : ParameterCodeBrick where T : new()
    {
        protected SingletonParameterCodeBrick()
        {
        }

        private static T c_obj = new T();
        public static T GetObject()
        {
            return c_obj;
        }

    }
}
