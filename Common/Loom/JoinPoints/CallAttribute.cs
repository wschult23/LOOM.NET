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
using System.Reflection;
using Loom.AspectModel;

namespace Loom.JoinPoints
{
    /// <summary>
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to interweave aspect methods with target class methods. 
    /// </summary>
    /// <remarks>
    /// <para>The aspect method will be executed, when a method call join-point of the target class is reached.</para>
    /// <para>
    ///   Use this attribute together with the <see cref="Advice"/> enumeration to specify how the aspect weaver should interweave the aspect class and target class.
    /// </para>
    /// <list type="table">
    ///   <listheader>
    ///     <term>Advice</term>
    ///     <description>Usage</description>
    ///     <description>Return type</description>
    ///   </listheader>
    ///   <item>
    ///     <term>After</term>
    ///     <description>The aspect method will be invoked after the target method has been called.</description>
    ///     <description>
    ///       <c>void</c>
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>AfterReturning</term>
    ///     <description>The aspect method will be invoked after the target method has been returned.</description>
    ///     <description>
    ///       <c>void</c>
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>AfterThrowing</term>
    ///     <description>The aspect method will be invoked after the target method has been thrown an exception.</description>
    ///     <description>
    ///       Simple match: must equal the return type of the target method<br />Advanced match: generic type
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>Around</term>
    ///     <description>The aspect method will be invoked instead of the target method.</description>
    ///     <description>
    ///       Simple match: must equal the return type of the target method<br />Advanced match: generic type
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term>Before</term>
    ///     <description>The aspect method will be invoked before the target method will be called.</description>
    ///     <description>
    ///       <c>void</c>
    ///     </description>
    ///   </item>
    /// </list>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointRemarks/*"/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the call attribute to interweave methods of a target class:
    /**
        <code>
        public class TargetClass
        {
            public virtual void method1()
            {
                Console.WriteLine("Inside method1."); 
            }

            public virtual void method2(string arg)
            {
                Console.WriteLine("Inside method2.");
            }
        }
            
        public class SimpleAspect : Aspect
        {
            [Call(Advice.Before)] // mark this method as an aspect method
            public void method1()
            {
                Console.WriteLine("This happens before the target method1().");
            }
            
            [Call(Advice.After)]  // mark this method as an aspect method
            public void method2(string arg)
            {
                Console.WriteLine("This happens after the target method2(string arg).");
            }
         } 
        
        ...
        // interweave target class methods with aspect methods using a simple match mechanism
        TargetClass target = Weaver.Create&lt;TargetClass&gt;(new SimpleAspect());
        target.method1();
        target.method2("foo");
        ...
     
     
        // [Output]
        // This happens before the target method1().
        // Inside method1.
        // Inside method2.
        // This happens after the target method2(string arg).
        </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CallAttribute : AspectMethodAttribute
    {
        /// <summary>
        /// Returns the approprirate AspectMember instance
        /// </summary>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmethod)
        {
            aspectclass.AddAdvice(new CallAspectMethod((MethodInfo)aspectmethod, this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallAttribute"/> attribute and marks a method as aspect method. 
        /// It can be used to specify the type of the join-point and the position where the aspect method becomes interwoven.
        /// </summary>
        /// <param name="invokeOrder">describes at which point in the execution of an join-point an interwoven method will be invoked</param>
        /// <seealso cref="Advice"/>
        public CallAttribute(Advice invokeOrder)
            : base(invokeOrder)
        {
        }
    }
}
