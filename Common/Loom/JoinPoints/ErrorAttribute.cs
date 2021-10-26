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
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to throw an exception, when the aspect method becomes interwoven.
    /// </summary>
    /// <remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointRemarks/*"/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the error attribute and <see cref="IncludeNonInterwovenAttribute">IncludeNonInterwovenAttribute attribute</see> to throw an exception, when interweaving aspect methods with a target class:
    /**
        <code>
        public class A 
        { 
            public virtual void foo(int i) { } 
        } 
        public class B 
        { 
            public virtual void foo() { } 
        } 

        public class SimpleAspect : Aspect 
        { 
            [Error("Type does not contain Void foo(int) method.")] 
            [MatchMode(Match.All)]  // every pointcut attribute must match, Include("foo") and IncludeNonInterwoven attribute
            [Include("foo")]        // methodnamepattern all target methods named "foo"
            [IncludeNonInterwoven]  // methodnamepattern only methods which are not interwoven with other aspect methods 
            public void fooError(params object[] args) { } 

            [Call(Advice.Before)] 
            public void foo(int i) { } 
        } 

        
        ...
        try
        {
            A a = Weaver.Create&lt;A&gt;(new SimpleAspect());
            B b = Weaver.Create&lt;B&gt;(new SimpleAspect());  // this fails
        }
        catch (WeaverException exc)
        {
            Console.WriteLine(exc); // this should end up here
        }
        ...
     
     
        // [Output]
        // Type does not contain Void foo(int) method.
        </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ErrorAttribute : AspectMethodAttribute
    {
        private string message;
        private TargetTypes targettype;
        /// <summary>
        /// returns the approprirate AspectMember instance
        /// </summary>
        /// <returns></returns>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmethod)
        {
            aspectclass.AddAdvice(new ErrorAspectMethod((MethodInfo)aspectmethod, targettype, message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorAttribute"/> attribute and marks a method as aspect method. 
        /// If there is a match with a target method, the weaver generates an error.  
        /// </summary>
        /// <param name="message">the error message of the thrown exception</param>
        /// <seealso cref="Advice"/>
        public ErrorAttribute(string message)
            : base(Advice.Before)
        {
            this.message = message;
            this.targettype = TargetTypes.Call;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorAttribute"/> attribute and marks a method as aspect method. 
        /// If there is a match with a target join-point, the weaver generates an error.  
        /// </summary>
        /// <param name="message">the error message of the thrown exception</param>
        /// <param name="targettype">The type of the Join-Point.</param>
        /// <seealso cref="Advice"/>
        public ErrorAttribute(TargetTypes targettype, string message)
            : base(Advice.Before)
        {
            this.message = message;
            this.targettype = targettype;
        }
    }
}
