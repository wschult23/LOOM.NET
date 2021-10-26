// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

using Loom.AspectModel;

namespace Loom.JoinPoints
{
    /// <summary>
    /// When an aspect tries to introduce an already existing interface then this enumeration configures the aspect weaver to perform one of the defined actions.
    /// </summary>
    public enum ExistingInterfaces
    {
        /// <summary>
        /// Alter every introduction method of this interface to a [<see cref="CallAttribute">Call</see>(<see cref="Advice"/>.<see cref="Loom.JoinPoints.Advice.Around"/>)] advice method.
        /// </summary>
        Advice,
        /// <summary>
        /// Don't interweave this interface. (This is the default behaviour)
        /// </summary>
        Skip,
        /// <summary>
        /// Throw an exception.
        /// </summary>
        Error
    }

    /// <summary>
    /// This attribute defines an aspect method which introduces a method.
    /// </summary>
    /// <example>
    /// This example shows how to introduce an interface to a class:
    /**
    <code>
        public interface IBase
        {
            int foo(int i);
        }

        public class A { }

        public class B : IBase
        {
            public int foo(int i) { return i; }
        }

        public class IntroduceAspect : Aspect
        {
            // Introduces int IBase.foo(int i)
            [Introduce(typeof(IBase),ExistingInterfaces.Advice)]
            public int foo([JPContext] Context ctx, int i)
            {
                Console.WriteLine(ctx.CurrentMethod.DeclaringType.Name);
                return i+1;
            }
        }

     
        ...

        IBase a = (IBase)Weaver.Create&lt;A&gt;(new IntroduceAspect());
        IBase b = Weaver.Create&lt;B&gt;(new IntroduceAspect());
        Console.WriteLine(a.foo(2));
        Console.WriteLine(b.foo(4));
        ...
        
        [Output]
        IBase     
        3
        B
        5
    </code> */
    /// </example>
    /// <remarks>
    /// <para>An Aspect could only introduce interface methods (including properties and events). The <see cref="IntroduceAttribute"/> requires to specify the
    /// interface where the method is defined. Partial interface introductions are not allowed. Each interface method
    /// must have exactly one corresponding aspect method (with a <see cref="IntroduceAttribute"/> that specifies that interface). 
    /// To find the corresponding aspect method, the aspect weaver uses the same rules like interweaving a <a href="html/a6898c0f-8a16-4ba2-acb9-cbd7f1efafce.htm">join-point</a>. 
    /// This means, that there is no need that aspect method and interface method have the same signature. You can use 
    /// join-point variables as well as wildcards in your aspect method.</para>
    /// <para>If you specify <see cref="ExistingInterfaces"/>.<see cref="ExistingInterfaces.Advice"/> and the target class already implements the interface, then
    /// your aspect method will be treated as a [<see cref="Loom.JoinPoints.CallAttribute">Call</see>(<see cref="Advice"/>.<see cref="Advice.Around"/>)] method. You could use
    /// the join-point parameter <see cref="Loom.JoinPoints.JPContextAttribute"/> to determine at runtime if the aspect method was interwoven as
    /// an advice, or as an introduction. In the latter case, the <see cref="Context"/>.<see cref="Context.CurrentMethod"/> property points to an 
    /// interface method and all calls of <see cref="Context"/>.<see cref="Context.Invoke"/>/<see cref="Context"/>.<see cref="Context.Call"/> will raise a <see cref="Loom.MissingTargetMethodException"/>.</para>
    /// <para>An introduction could be declared as <c>static</c> aspect method. This is the only way to introduce an interface to a target class, if the aspect will be used
    /// as annotation of a method, property, or event of that target class.</para>
    /// <para>To find the appropriate interface method, the weaver uses the following rules:</para>
    /// <include file="Doc/JoinPointAttributes.xml" path='doc/JoinPointRemarks/para[@use="introduction"]'/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Event | AttributeTargets.Property, AllowMultiple = true)]
    public class IntroduceAttribute : AspectMethodAttribute
    {
        internal Type iftype;
        internal ExistingInterfaces existinginterfaces;

        /// <summary>
        /// Defines an aspect method which introduces a method.
        /// </summary>
        /// <param name="iftype">the interface</param>
        public IntroduceAttribute(Type iftype) :
            base(Advice.Around)
        {
            if (!iftype.IsInterface) throw new AspectWeaverException(7, Loom.Resources.Errors.ERR_0007);
            this.iftype = iftype;
            this.existinginterfaces = ExistingInterfaces.Skip;
        }

        /// <summary>
        /// Defines an aspect method which introduces a method
        /// </summary>
        /// <param name="iftype">the interface</param>
        /// <param name="existinginterfaces">When an aspect tries to introduce an already existing interface then this enumeration configures the aspect weaver to perform the selected action</param>
        public IntroduceAttribute(Type iftype, ExistingInterfaces existinginterfaces)
            :
            base(Advice.Around)
        {
            if (!iftype.IsInterface) throw new AspectWeaverException(7, Loom.Resources.Errors.ERR_0007);
            this.iftype = iftype;
            this.existinginterfaces = existinginterfaces;
        }


        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmember)
        {
            switch (aspectmember.MemberType)
            {
                case MemberTypes.Method:
                    aspectclass.AddIntroduction(new IntroduceAspectMethod((MethodInfo)aspectmember, iftype, existinginterfaces));
                    break;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)aspectmember;
                    foreach (MethodInfo mi in pi.GetAccessors())
                    {
                        aspectclass.AddIntroduction(new IntroduceAspectProperty(mi, iftype, existinginterfaces));
                    }
                    break;
                case MemberTypes.Event:
                    EventInfo ei = (EventInfo)aspectmember;
                    foreach (MethodInfo mi in GetEventMethods(ei))
                    {
                        aspectclass.AddIntroduction(new IntroduceAspectEvent(mi, iftype, existinginterfaces));
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
        }
    }


}