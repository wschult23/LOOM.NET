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
    /// This aspect method attribute can be used to mark an aspect method within a <see cref="Aspect"/> class to get a warning message from the weaver, when the aspect method becomes interwoven.
    /// </summary>
    /// <remarks>
    /// <para>Use the <see cref="Loom.AspectWeaver.WeaverMessages" /> to retrieve the warning.</para>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointRemarks/*"/>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/AspectMethodSeeAlso/*"/>
    /// <example>
    /// This example shows how to use the warning attribute and the <see cref="IncludeNonInterwovenAttribute">IncludeNonInterwovenAttribute attribute</see> to get a warning message of the weaver, when interweaving aspect methods with a target class:
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
            [Warning("Type does not contain Void foo(int) method.")]
            [IncludeNonInterwovenAttribute]
            public void foo(int i) { }
        } 
        
        ...
         /// define event handler and create a handler method
        Loom.WeaverMessages.WeaverMessageEventHandler eventHandler = new Loom.WeaverMessages.WeaverMessageEventHandler(Weaver_WeaverMessages);
        Weaver.WeaverMessages += eventHandler; // register event handler
     
        A a = Weaver.Create&lt;A&gt;(new SimpleAspect());
        B b = Weaver.Create&lt;B&gt;(new SimpleAspect());  // this fails
        ...
      
        /// the handler method for the event
        static void Weaver_WeaverMessages(object sender, Loom.WeaverMessages.WeaverMessageEventArgs args)
        {
            Console.WriteLine("{0}: {1}", args.Type, args.Message);
        }
        ...
     
 
        // [Output]
        // Type does not contain Void foo(int) method.
        </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class WarningAttribute : AspectMethodAttribute
    {
        private string message;
        private TargetTypes targettype;
        /// <summary>
        /// returns the approprirate AspectMember instance
        /// </summary>
        internal override void DefineWeavingPoints(AspectClass aspectclass, MemberInfo aspectmethod)
        {
            aspectclass.AddAdvice(new WarningAspectMethod((MethodInfo)aspectmethod, targettype, message));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarningAttribute"/> attribute and marks a method as aspect method. 
        /// If there is a match with a target method, the weaver generates a warning message.  
        /// </summary>
        /// <param name="message">the warning message that will be posted</param>
        /// <seealso cref="Advice"/>
        public WarningAttribute(string message)
            : base(Advice.Before)
        {
            this.targettype = TargetTypes.Call;
            this.message = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WarningAttribute"/> attribute and marks a method as aspect method. 
        ///  If there is a match with a target join-point, the weaver generates a warning message.  
        /// </summary>
        /// <param name="message">the warning message that will be posted</param>
        /// <param name="targettype">The type of the join-point.</param>
        /// <seealso cref="Advice"/>
        public WarningAttribute(TargetTypes targettype, string message)
            : base(Advice.Before)
        {
            this.targettype = targettype;
            this.message = message;
        }
    }
}
