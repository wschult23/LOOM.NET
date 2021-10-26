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
using Loom.AspectModel;
using System.Reflection;

namespace Loom.JoinPoints
{
    /// <summary>
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to interweave aspect methods with the target's class destructor. 
    /// </summary>
    /// <remarks>
    /// The aspect methods are invoked, 
    /// when the target destructor is executed.
    /// Use this attribute together with the <see cref="Advice"/> enumeration to specify how the aspect weaver should interweave the aspect class and target class.
    /// <list type="table">
    ///     <listheader>
    ///         <term>Advice</term>
    ///         <description>Usage</description>
    ///         <description>Return type</description>
    ///     </listheader>
    ///     <item>
    ///         <term>After</term>
    ///         <description>The aspect method will be invoked after the target class destructor has been called.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterReturning</term>
    ///         <description>The aspect method will be invoked after the target class destructor has been returned.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterThrowing</term>
    ///         <description>The aspect method will be invoked after the target class destructor has been thrown an exception.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Around</term>
    ///         <description>The aspect method will be invoked instead of the target class destructor.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>Before</term>
    ///         <description>The aspect method will be invoked before the target class destructor will be called.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the finalize attribute to interweave the destructor of a target class:
    /**
        <code>
        public class Target
        {
            public Target()  // constructor
            {
                Console.WriteLine("This is the target class constructor.");
            }

            ~Target()  // destructor
            {
                Console.WriteLine("This is the target class destructor.");
            }
        }
      
        public class DestructorAspect : Aspect
        {
            public DestructorAspect()
            {
                GC.SuppressFinalize(this);
            }

            [Finalize(Advice.Before)] // marks this method as an aspect method, which is called before the destrucor
            public void beforeFinalizing()
            {
                Console.WriteLine("This is before the destructor of the target class.");
            }

            [Finalize(Advice.After)] // marks this method as an aspect method, which is called after the destructor
            public void afterFinalizing() 
            {
                Console.WriteLine("This is after the destructor of the target class.");
            }
        }
     
        ...
        Target t = Weaver.Create&lt;Target&gt;(new DestrucorAspect());
        t = null;       // release all references
        GC.Collect();   // this starts the destruction of t
        ...
     
     
        // [Output]
        // This is the target class constructor.
        // This is before the destructor of the target class.
        // This is the target class destructor.
        // This is after the destructor of the target class.
        </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FinalizeAttribute : CallAttribute
    {
        /// <summary>
        /// retruns the approprirate AspectMember instance
        /// </summary>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmethod)
        {
            aspectclass.AddAdvice(new FinalizeAspectMethod((MethodInfo)aspectmethod, this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FinalizeAttribute"/> attribute and marks a method as aspect method. 
        /// It can be used to specify the type of the join-point and the position where the aspect method becomes interwoven.
        /// </summary>
        /// <param name="invokeOrder">describes at which point an interwoven method will be invoked</param>
        /// <seealso cref="Advice"/>
        public FinalizeAttribute(Advice invokeOrder)
            : base(invokeOrder)
        {
        }
    }
}
