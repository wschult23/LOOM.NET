// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;

using Loom.AspectModel;
using Loom.CodeBuilder;
using Loom.JoinPoints.Implementation;

namespace Loom.JoinPoints
{

    /// <summary>
    /// Specifies that only methods declared at the level of the target class type's hierarchy should be considered to become interwoven. 
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IncludeDeclaredOnlyAttribute : Attribute, IJoinPointSelector
    {
        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodBaseJoinPoint jpt = jp as MethodBaseJoinPoint;
            if (jpt == null) return JoinPointSelectorResult.Undecided;
            return jpt.DeclaringType == jpt.Method.DeclaringType ? JoinPointSelectorResult.Match : JoinPointSelectorResult.NoMatch;
        }
    }


    /// <summary>
    /// This class is for internal purposes only!
    /// </summary>
    public abstract class IncludeBaseAttribute : Attribute, IJoinPointSelector
    {
        internal IJoinPointSelectorInternal include;
        internal IncludeBaseAttribute(IJoinPointSelectorInternal include)
        {
            this.include = include;
        }

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return include.IsMatch(jp, pass, interwovenmethods) ? JoinPointSelectorResult.Match : JoinPointSelectorResult.Undecided;
        }
    }

    /// <summary>
    /// This class is for internal purposes only!
    /// </summary>
    public abstract class ExcludeBaseAttribute : Attribute, IJoinPointSelector
    {
        internal IJoinPointSelectorInternal exclude;
        internal ExcludeBaseAttribute(IJoinPointSelectorInternal exclude)
        {
            this.exclude = exclude;
        }

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return exclude.IsMatch(jp, pass, interwovenmethods) ? JoinPointSelectorResult.NoMatch : JoinPointSelectorResult.Undecided;
        }
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It includes specified target methods to become interwoven with the aspect methods, only if 
    /// the signature of the target method matches.
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IncludeAttribute : IncludeBaseAttribute
    {
        /// <summary>
        /// Use this pointcut attribute on an aspect method to decscribe that target methods with a particular name should be considered 
        /// for interweaving with this aspect method.
        /// </summary>
        /// <param name="methodname">a pattern which describes all method names to include</param>
        /// <include file="Doc/JoinPointAttributes.xml" path="doc/WildcardRemarks/*"/>
        public IncludeAttribute(string methodname)
            :
            base(new MethodNameMatch(methodname))
        {
        }

        /// <summary>
        /// Use this pointcut attribute on an aspect method to decscribe that target methods of a particular type or interface should be considered 
        /// for interweaving with this aspect method.
        /// </summary>
        /// <param name="type">a class or interface type which should be included</param>
        public IncludeAttribute(Type type) :
            base(type.IsInterface ? (IJoinPointSelectorInternal)new InterfaceMatch(type) : (IJoinPointSelectorInternal)new TypeMatch(type))
        {
        }

        /// <summary>
        /// Use this pointcut attribute on an aspect method to decscribe that target methods of a particular type or interface and with a certain name
        /// should be considered for interweaving with this aspect method.
        /// </summary>
        /// <param name="type">a class or interface type which should be included</param>
        /// <param name="methodname">a pattern which describes all method names to include</param>
        /// <include file="Doc/JoinPointAttributes.xml" path="doc/WildcardRemarks/*"/>
        public IncludeAttribute(Type type, string methodname)
            :
            base(new CombinedMatch(type.IsInterface ? (IJoinPointSelectorInternal)new InterfaceMatch(type) : (IJoinPointSelectorInternal)new TypeMatch(type), new MethodNameMatch(methodname)))
        {
        }


    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It excludes specified target methods to become interwoven with the aspect methods.
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExcludeAttribute : ExcludeBaseAttribute
    {
        /// <summary>
        /// This pointcut attribute describes which target methods should not be interwoven with this aspect method.
        /// </summary>
        /// <param name="methodname">a methodnamepattern which describes all method names to exclude</param>
        /// <include file="Doc/JoinPointAttributes.xml" path="doc/WildcardRemarks/*"/>
        public ExcludeAttribute(string methodname)
            :
            base(new MethodNameMatch(methodname))
        {
        }

        /// <summary>
        /// This pointcut attribute describes which target methods should not be interwoven with this aspect method.
        /// </summary>
        /// <param name="type">a class or interface type which should not be interwoven</param>
        public ExcludeAttribute(Type type)
            :
            base(type.IsInterface ? (IJoinPointSelectorInternal)new InterfaceMatch(type) : (IJoinPointSelectorInternal)new TypeMatch(type))
        {
        }

        /// <summary>
        /// This pointcut attribute describes which target methods should not be interwoven with this aspect method.
        /// </summary>
        /// <param name="type">a class or interface type which should not be interwoven</param>
        /// <param name="methodname">a methodnamepattern which describes all method names to exclude</param>
        /// <include file="Doc/JoinPointAttributes.xml" path="doc/WildcardRemarks/*"/>
        public ExcludeAttribute(Type type, string methodname)
            :
            base(new CombinedMatch(type.IsInterface ? (IJoinPointSelectorInternal)new InterfaceMatch(type) : (IJoinPointSelectorInternal)new TypeMatch(type), new MethodNameMatch(methodname)))
        {
        }
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It includes target methods with a specified attribute to become interwoven with the aspect methods, 
    /// only if the signature of the target method matches.
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IncludeAnnotatedAttribute : IncludeBaseAttribute
    {
        /// <summary>
        /// This pointcut attribute includes all target methods which have the given attribute to become interwoven with this aspect method.
        /// </summary>
        /// <param name="type">the attribute type</param>
        public IncludeAnnotatedAttribute(Type type) :
            base(new AttributeMatch(type))
        {
        }
        /// <summary>
        /// This pointcut attribute includes all target methods which have the given attribute to become interwoven with this aspect method.
        /// </summary> 
        /// <param name="type">the attribute type</param>
        /// <param name="inherit">specifies whether to search in the method's inheritance chain to find the attribute</param>
        public IncludeAnnotatedAttribute(Type type, bool inherit) :
            base(new AttributeMatch(type, inherit))
        {
        }
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It excludes target methods with a specified attribute to become interwoven with the aspect methods. 
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExcludeAnnotatedAttribute : ExcludeBaseAttribute
    {
        /// <summary>
        /// This pointcut attribute excludes all target methods which have the given attribute to become interwoven with this aspect method.
        /// </summary>
        /// <param name="type">the attribute type</param>
        public ExcludeAnnotatedAttribute(Type type) :
            base(new AttributeMatch(type))
        {
        }
        /// <summary>
        /// This pointcut attribute excludes all target methods which have the given attribute to become interwoven with this aspect method.
        /// </summary>
        /// <param name="type">the attribute type</param>
        /// <param name="inherit">specifies whether to search in the method's inheritance chain to find the attribute</param>
        public ExcludeAnnotatedAttribute(Type type, bool inherit) :
            base(new AttributeMatch(type, inherit))
        {
        }
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It includes all target methods to become interwoven with the aspect methods, 
    /// only if the signature of the target method matches.
    /// </summary>
    /// <remarks>The finalizer is not included using this pointcut attribute. Use <see cref="FinalizeAttribute"/> to interweave the destructor with an aspect method.</remarks>
    /// <example>
    /// This example shows how to use the IncludeAll attribute to interweave methods of a target class:
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
            [Call(Advice.Before)]   // mark this method as an aspect method
            [IncludeAll]            // ignores the need of a matching method name
            public void foo([JPContext] Context ctx, params object[] args)  // but, still need to match the parameter list !!!
            {
                Console.WriteLine("This happens before {0}.", ctx.CurrentMethod);
            }
         } 
        
        ...
        // interweave target class methods with aspect methods using a simple match mechanism
        TargetClass target = Weaver.Create&lt;TargetClass&gt;(new SimpleAspect());
        target.method1();
        target.method2("foo");
        ...
     
     
        [Output]
        This happens before Void method1().
        Inside method1.
        This happens before Void method2(System.String).
        Inside method2.
        </code> */
    /// </example>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IncludeAllAttribute : Attribute, IJoinPointSelector
    {
        /// <summary>
        /// This pointcut attribute includes all target methods to become interwoven with this aspect method..
        /// </summary>
        public IncludeAllAttribute()
        {
        }

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return JoinPointSelectorResult.Match;
        }
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It only includes target methods which are not interwoven with other aspect methods to become interwoven with the aspect methods.
    /// </summary>
    /// <example>
    /// This example shows how to use the IncludeNonInterwoven attribute:
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
     
     
        [Output]
        Type does not contain Void foo(int) method.
        </code> */
    /// </example>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IncludeNonInterwovenAttribute : Attribute, IJoinPointSelector
    {
        #region IJoinPointSelector Members

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            if (interwovenmethods.Count > 0)
            {
                return JoinPointSelectorResult.NoMatch;
            }
            if (pass == 1)
            {
                return JoinPointSelectorResult.NotYet;
            }
            else
            {
                if (interwovenmethods.Count == 0) return JoinPointSelectorResult.Match;
            }
            return JoinPointSelectorResult.Undecided;
        }

        #endregion
    }

    /// <summary>
    /// This pointcut attribute is used on an aspect method. 
    /// It excludes target methods which are not interwoven with other aspect methods to become interwoven with the aspect methods.
    /// </summary>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointAttributeRemarks/*"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExcludeNonInterwovenAttribute : Attribute, IJoinPointSelector
    {
        #region IJoinPointSelector Members

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            if (pass == 1)
            {
                return JoinPointSelectorResult.NotYet;
            }
            else
            {
                if (interwovenmethods.Count == 0) return JoinPointSelectorResult.NoMatch;
            }
            return JoinPointSelectorResult.Undecided;
        }

        #endregion
    }
}
