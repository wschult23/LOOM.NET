// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Loom.JoinPoints;
using Loom.AspectProperties;

namespace Loom
{
    /// <summary>
    /// The Aspect interface extends a .net class to an aspect class. 
    /// </summary>
    /// <remarks>
    /// An aspect class can be interwoven with target classes by using <see cref="O:Loom.Weaver.Create"/> or <see cref="O:Loom.Weaver.CreateInstance"/>. 
    /// An aspect class can contain aspect methods. See <see cref="Loom.AspectAttribute"/> if you want to use an aspect like an attribute for static weaving. 
    /// Each aspect method is defined through one of the following method attributes:
    /// <list type="table">
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.CreateAttribute"/></term>
    ///         <description>The aspect method advices a target class construction.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.CallAttribute"/></term>
    ///         <description>The aspect method advices a method call.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.AccessAttribute"/></term>
    ///         <description>The aspect method advices a property/event access.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.DestroyAttribute"/></term>
    ///         <description>The aspect method advices the dispose of target class object.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.FinalizeAttribute"/></term>
    ///         <description>The aspect method advices a target class object destruction.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.IntroduceAttribute"/></term>
    ///         <description>The aspect method will be introduced to a target class.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.WarningAttribute"/></term>
    ///         <description>The weaver reports a warning if the aspect method would become interwoven.</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Loom.JoinPoints.ErrorAttribute"/></term>
    ///         <description>The weaver throws an exception if the aspect method would become interwoven.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// This example shows a simple TRACING aspect:
    /**
    <code>
        public class TRACE : Aspect 
        { 
          [Call(Advice.Around)]
          [IncludeAll]
          public T f&lt;T&gt;([JPContext] Context context, params object[] args)
          {
            // This method will be called on every interwoven method
            object o = context.Invoke(args);  // invokes the target method
            return (T)o;
          }
        }

        public class A
        {
            ...
        }

        ...

        A a = Weaver.Create&lt;A&gt;(new TRACE());
         
    </code> */
    /// </example> 
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    public interface Aspect
    {
    }


    /// <summary>
    /// The <see cref="AspectAttribute"/> class is the base class of all aspects which can be interwoven statically. It also 
    /// implements the <see cref="Aspect"/> interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// According to the rules of <a href="html/aspectcoverage.htm">Aspect Coverage</a> the <see cref="AspectAttribute"/> can be applied to the following targets:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Target</term>
    ///         <description>Meaning</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Assembly (<see cref="System.AttributeTargets"/>.<c>Assembly</c>)</term>
    ///         <description>Every class defined in the assembly and every class derived* from that class will become interwoven with the aspect</description>
    ///     </item>
    ///     <item>
    ///         <term>Class (<see cref="System.AttributeTargets"/>.<c>Class</c>>)</term>
    ///         <description>The class itself and every class derived* from that class will become interwoven with the aspect</description>
    ///     </item>
    ///     <item>
    ///         <term>Interface (<see cref="System.AttributeTargets"/>.<c>Interface</c>)</term>
    ///         <description>Every interface implementation will become interwoven with the aspect</description>
    ///     </item>
    ///     <item>
    ///         <term>Member (<see cref="System.AttributeTargets"/>.<c>Method</c>, .<c>Constructor</c>, .<c>Property</c>, .<c>Event</c>)</term>
    ///         <description>Only the annotated member will become interwoven with the aspect**</description>
    ///     </item>
    /// </list>
    /// </para>
    /// <para>
    /// (*) Only if the <see cref="System.AttributeUsageAttribute.Inherited">AttributeUsage.Inherited</see> member is set to <c>true</c>.
    /// </para>
    /// <para>
    /// (**) Additionaly the aspect could introduce new methods, but the aspect method implementing the introduction has to be <c>static</c>.
    /// </para>
    /// <para>
    /// You can use the <see cref="Loom.AspectProperties.CreateAspectAttribute"/> to control the instance creation of your aspect.
    /// If the creation type is different to <see cref="Loom.AspectProperties.Per"/>.<see cref="Loom.AspectProperties.Per.AspectClass"/> or <see cref="Loom.AspectProperties.Per"/>.<see cref="Loom.AspectProperties.Per.Annotation"/>, 
    /// the aspect weaver uses <see cref="System.ICloneable.Clone"/> (if the aspect implements the <see cref="System.ICloneable"/> interface) or <see cref="System.Object.MemberwiseClone"/> (otherwise)
    /// to create new instances of the aspect.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows a simple TRACING aspect:
    /**
    <code>
        public class TRACE : AspectAttribute 
        { 
          [Call(Advice.Before)]
          [IncludeAll]
          public void before(object[] args)
          {
            // This method will be called before the execution of every target class method
            return default(T);
          }
        }

        [TRACE]  // static assignment of the TRACE aspect to the target class A
        public class A
        {
            ...
        }

        ...

        A a = Weaver.Create&lt;A&gt;();
         
    </code>*/
    /// </example>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class AspectAttribute : Attribute, Aspect
    {
        /// <summary>
        /// Initializes a new instance of the AspectAttribute class. 
        /// </summary>
        public AspectAttribute()
        {
        }

        /// <summary>
        /// This method is no longer supported and returns null.
        /// </summary>
        /// <param name="aspecttype">obsolete.</param>
        [Obsolete("This method is no longer supported")]
        public static AspectAttribute GetAspect(Type aspecttype)
        {
            return null;
        }

        /// <summary>
        /// This method is no longer supported and returns null.
        /// </summary>
        /// <typeparam name="ASPECTTYPE">obsolete.</typeparam>
        [Obsolete("This method is no longer supported")]
        public static AspectAttribute GetAspect<ASPECTTYPE>()
        {
            return null;
        }

        /// <summary>
        /// This Property is no longer supported an will always return false
        /// </summary>
        /// <value><c>false</c>.</value>
        [Obsolete("This Property is no longer supported an will always return false")]
        public static bool CompileMode
        {
            get
            {
                return false;
            }
        }
    }
}