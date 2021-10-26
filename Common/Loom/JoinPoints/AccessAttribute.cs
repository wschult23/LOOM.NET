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
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to interweave aspect methods with target class properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <para>The aspect method will be executed, when property access join-point of the target class is reached.</para>
    /// </para>
    /// <para>If you want to use additional join-point-parameters for the aspect properties, you must split the aspect property inside the aspect into two seperate methods.
    /// Use the original property name with the prefixes set_ and get_ as the method names.</para>
    /// <para>Use this attribute together with the <see cref="Advice"/> enumeration to specify how the aspect weaver should interweave the aspect class and target class.</para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Advice</term>
    ///         <description>Usage</description>
    ///         <description>Return type</description>
    ///     </listheader>
    ///     <item>
    ///         <term>After</term>
    ///         <description>The aspect method will be invoked after the target property is accessed.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterReturning</term>
    ///         <description>The aspect method will be invoked after the target property was accessed and returns.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    ///     <item>
    ///         <term>AfterThrowing</term>
    ///         <description>The aspect method will be invoked after the target property has been thrown an exception.</description>
    ///         <description>Simple match: must equal the return type of the target method<br />Advanced match: generic type</description>
    ///     </item>
    ///     <item>
    ///         <term>Around</term>
    ///         <description>The aspect method will be invoked instead of the target property.</description>
    ///         <description>Simple match: must equal the return type of the target method<br />Advanced match: generic type</description>
    ///     </item>
    ///     <item>
    ///         <term>Before</term>
    ///         <description>The aspect method will be invoked before the target property will be accessed.</description>
    ///         <description><c>void</c></description>
    ///     </item>
    /// </list>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointRemarks/*"/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the access attribute to interweave a property of a target class:
    /**
    <code>
    public class TargetClass
    {
        private int _counter = 0;
         
        public virtual int Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }
    }
        
    public class PropertyAspect : Aspect
    {
        // use this option if you don't need additional join-point-parameters
        [Access(Advice.Before)] 
        public int Counter 
        { 
            get 
            { 
              ... 
            } 
                
            set 
            { 
              ... 
            } 
        } 
     
        // use this option if you want to use additional join-point-parameters
        [Access(Advice.Before)]
        public void set_Counter([JPContext] Context ctx, int value)
        {
            ...
        }
        [Access(Advice.Before)]
        public void get_Counter([JPContext] Context ctx)
        {
            ...
        }
     } 
    
    ...
    // interweave target class property Counter with aspect methods set_Counter, get_Counter and the Counter property
    TargetClass target = Weaver.Create&lt;TargetClass&gt;(new PropertyAspect());
    ...
    </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
    public class AccessAttribute : CallAttribute
    {
        /// <summary>
        /// retruns the approprirate AspectMember instance
        /// </summary>
        /// <returns></returns>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmember)
        {
            switch (aspectmember.MemberType)
            {
                case MemberTypes.Method:
                    aspectclass.AddAdvice(new AccessAspectMethod((MethodInfo)aspectmember, this));
                    break;
                case MemberTypes.Property:
                    PropertyInfo pi = (PropertyInfo)aspectmember;
                    foreach (MethodInfo mi in pi.GetAccessors())
                    {
                        aspectclass.AddAdvice(new AccessAspectMethod(mi, this));
                    }
                    break;
                case MemberTypes.Event:
                    EventInfo ei = (EventInfo)aspectmember;
                    foreach (MethodInfo mi in GetEventMethods(ei))
                    {
                        aspectclass.AddAdvice(new AccessAspectMethod(mi, this));
                    }
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessAttribute"/> attribute and marks a method as aspect method. 
        /// It can be used to specify the type of the join-point and the position where the aspect method becomes interwoven.
        /// </summary>
        /// <param name="invokeOrder">describes at which point an interwoven method will be invoked</param>
        /// <seealso cref="Advice"/>
        public AccessAttribute(Advice invokeOrder)
            : base(invokeOrder)
        {
        }
    }
}
