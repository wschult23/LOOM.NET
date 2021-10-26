// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Loom.AspectModel;

namespace Loom.JoinPoints
{

    /// <summary>
    /// This attribute is obsolete. 
    /// </summary>
    /// <remarks>
    /// Instead of using the <see cref="DestroyAttribute"/>, you should use the <see cref="IntroduceAttribute"/>.
    /// </remarks>
    /// <example>
    /// This example shows how to replace the implemetation of as destroy attribute:
    /**
        <code>
        public class A 
        {
        }

        public class DestroyAspect:Aspect
        {
    
            // [Destroy]
            [Introduce(typeof(IDisposable), ExistingInterfaces.Advice)]
            public void Dispose([JPContext] Context ctx)
            {
                if(ctx.HasJoinPoint) // In case of a base implementation that already implements IDisposable
                {
                    ctx.Call();
                }
            }
        }
        
        ...
        A a = Weaver.Create&lt;A&gt;(new DestroyAspect());
        var disp=(IDisposable)a;
        disp.Dispose();
        ...
        </code> */
    /// </example>
    [Obsolete("This attribute is no longer supported. See documentation for more information.", true)]
    public class DestroyAttribute : IntroduceAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DestroyAttribute"/> attribute and marks a method as aspect method. 
        /// It can be used to specify the type of the join-point and the position where the aspect method becomes interwoven.
        /// </summary>
        /// <param name="advice">describes at which point an interwoven method will be invoked</param>
        public DestroyAttribute(Advice advice)
            : base(typeof(IDisposable), ExistingInterfaces.Advice)
        {
        }


        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmember)
        {
            throw new AspectWeaverException(23, Resources.Errors.ERR_0023);
        }

    }
}
