// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;

using Loom;
using Loom.JoinPoints;

namespace Loom.Runtime
{
    /// <summary>
    /// Implements an context for object construction
    /// </summary>
    [Serializable]
    public abstract class CreateContext : Context
    {
        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected ClassObject classobject;
        /// <summary>
        /// This method supports the LOOM.NET infrastructure and is not intended to be used directly from your code.
        /// </summary>
        protected Aspect[] aspects;

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public CreateContext(ClassObject classobject, Aspect[] aspects)
        {
            this.classobject = classobject;
            this.aspects = aspects;
        }

        /// <summary>
        /// Overridden from <see cref="Context.ReturnType"/>.
        /// </summary>
        public override Type ReturnType
        {
            get { return classobject.Type; }
        }

        /// <summary>
        /// Overridden from <see cref="Context.InterwovenClass"/>.
        /// </summary>
        public override Type InterwovenClass
        {
            get { return classobject.Type; }
        }

        /// <summary>
        /// Overridden from <see cref="Context.Invoke"/>. 
        /// </summary>
        /// <returns>throws a <see cref="MethodInvocationException"/></returns>
        public override object InvokeOn(object target, object[] args)
        {
            throw new MethodInvocationException(Resources.Errors.ERR_9001);
        }

        /// <summary>
        /// Overridden from <see cref="Context.ReturnType"/>.
        /// </summary>
        public override object ReCall(params object[] args)
        {
            return classobject.CreateInstance(args, aspects);
        }
    }
}
