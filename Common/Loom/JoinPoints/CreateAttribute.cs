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
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to interweave aspect methods with target class constructors. 
    /// </summary>
    /// <remarks>
    /// <para>The aspect method will be executed, when an object creation join-point of the target class is reached.</para>
    /// <para>Use this attribute together with the <see cref="Advice"/> enumeration to specify how the aspect weaver should interweave the aspect class and target class.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Advice</term>
    ///         <description>Usage</description>
    ///         <description>Return type</description>
    ///     </listheader>
    ///     <item>
    ///         <term>After</term>
    ///         <description>The aspect method will be invoked after the target class constructor has been called.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterReturning</term>
    ///         <description>The aspect method will be invoked after the target class constructor has been returned.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterThrowing</term>
    ///         <description>The aspect method will be invoked after the target class constructor has been thrown an exception.</description>
    ///         <description>Simple match: must equal the return type of the target method<br />Advanced match: generic type</description>
    ///     </item>
    ///     <item>
    ///         <term>Around</term>
    ///         <description>The aspect method will be invoked instead of the target class constructor.</description>
    ///         <description>Simple match: must equal the return type of the target method<br />Advanced match: generic type</description>
    ///     </item>
    ///     <item>
    ///         <term>Before</term>
    ///         <description>The aspect method will be invoked before the target class constructor will be called.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    /// </list>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointRemarks/*"/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the create attribute to interweave constructors of a target class:
    /**
        <code>
        public class TargetClass
        {
            public TargetClass()
            {
                Console.WriteLine("This is inside the default constructor.");
            }

            public TargetClass(int arg)
            {
                Console.WriteLine("This is inside the second constructor.");
            }
        }        
        
        public class CreateAspect : Aspect
        {
            [IncludeAll]                        // methodnamepattern all constructors
            [Create(Advice.Before)]             // mark this method as an aspect method which is called before the constructor
            public void constructor([JPContext] Context ctx, params object[] args) // using wildcard weaving with params keyword
            {
                Console.WriteLine("This is before the constructor " + ctx.CurrentMethod);
            }
        }
        
        ...
        CreateAspect aspect = new CreateAspect();
        
        // use the default constructor for object creation
        TargetClass t1 = Weaver.Create&lt;TargetClass&gt;(aspect);
        
        // use the second construcor for object creation
        TargetClass t2 = Weaver.Create&lt;TargetClass&gt;(aspect, 1);
        ...   
      
     
        // [Output]
        // This is before the constructor Void .ctor()
        // This is inside the default constructor.
        // This is before the constructor Void .ctor(Int32)
        // This is inside the second constructor.
        </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CreateAttribute : AspectMethodAttribute
    {
        /// <summary>
        /// Returns the approprirate AspectMember instance
        /// </summary>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmethod)
        {
            aspectclass.AddAdvice(new CreateAspectMethod((MethodInfo)aspectmethod, this));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAttribute"/> attribute and marks a method as aspect method. 
        /// It can be used to specify the type of the join-point and the position where the aspect method becomes interwoven.
        /// </summary>
        /// <param name="invokeOrder">describes at which point an interwoven method will be invoked</param>
        /// <seealso cref="Advice"/>
        public CreateAttribute(Advice invokeOrder)
            : base(invokeOrder)
        {
        }
    }

}
