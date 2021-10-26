// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;
using Loom.AspectModel;

namespace Loom.JoinPoints
{
    /// <summary>
    /// Defines the type of the join-point parameter.
    /// </summary>
    internal enum ParamType
    {
        /// <summary>
        /// The aspect parameter is an <see cref="Context"/>.
        /// </summary>
        Context,
        /// <summary>
        /// The aspect parameter is a type object representing the target class
        /// </summary>
        TargetClass,
        /// <summary>
        /// The aspect parameter is an object representing the target.
        /// </summary>
        Target,
        /// <summary>
        /// The aspect parameter is a type object representing the interwoven class.
        /// </summary>
        InterwovenClass,
        /// <summary>
        /// The aspect parameter is an object representing the return value of the target method.
        /// </summary>
        RetVal,
        /// <summary>
        /// The aspect parameter is an object representing the exception (if there was one) thrown by the target method.
        /// </summary>
        Exception,
        /// <summary>
        /// The aspect parameter is an object representing an aspect variable.
        /// </summary>
        Variable,
        /// <summary>
        /// Reserved for internal use.
        /// </summary>
        Unspecified
    }

    /// <summary>
    /// The base class for all attributes that declare a parameter in a aspect method signature as join-point parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public abstract class JoinPointParameterAttribute : Attribute, ICreateParameterMatch
    {
        ParamType apt;

        /// <summary>
        /// Declares a parameter in a aspect method signature as join-point parameter
        /// </summary>
        /// <param name="apt">the type of the join-point parameter</param>
        internal JoinPointParameterAttribute(ParamType apt)
        {
            this.apt = apt;
        }

        #region ICreateParameterMatch Members

        /// <summary>
        /// Baut das richtige Matchobjekt für den Parameterzugriff
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        IParameterMatch ICreateParameterMatch.Create(ParameterInfo pi)
        {
            // Check if parameter-Type is ok
            Type expected = null;
            switch (apt)
            {
                default: System.Diagnostics.Debug.Fail("undefined"); break;
                case ParamType.Unspecified:
                    throw new AspectWeaverException(2, Loom.Resources.Errors.ERR_0002, pi.Name, Common.Convert.ToString(pi.Member));
                case ParamType.Context: expected = typeof(Context); break;
                case ParamType.Target: expected = typeof(object); break;
                case ParamType.TargetClass: expected = typeof(Type); break;
                case ParamType.InterwovenClass: expected = typeof(Type); break;
                case ParamType.Exception: expected = typeof(Exception); break;
                case ParamType.RetVal:
                    {
                        MethodInfo mi = pi.Member as MethodInfo;
                        if (mi == null) throw new AspectWeaverException(4, Loom.Resources.Errors.ERR_0004, pi.Name, Common.Convert.ToString(pi.Member), ParamType.RetVal.ToString());
                        if (pi.ParameterType.IsGenericParameter)
                        {
                            return new GenericRetValAspectParameter(pi);
                        }
                        return new RetValAspectParameter(pi);
                    }
            }

            if (pi.ParameterType != expected)
            {
                throw new AspectWeaverException(5, Loom.Resources.Errors.ERR_0005, pi.Name, Common.Convert.ToString(pi.Member), expected.Name);
            }
            return new AspectParameter(pi, apt);
        }

        #endregion
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="Context"/> object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </para>
    /// <para>
    /// If your aspect method is interwoven with the <see cref="Advice">Advice.Around</see> parameter, you need a <see cref="Context"/> object to have control over the target class object. 
    /// The context object allows you to proceed the method call on the interwoven target class.
    /// </para>
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointParameterExample/*"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPContextAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="Context"/> object.
        /// </summary>
        public JPContextAttribute() : base(ParamType.Context) { }
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="System.Type"/> object representing the target class.
    /// </summary>
    /// <remarks>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointParameterExample/*"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPTargetClassAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="System.Type"/> object representing the target class.
        /// </summary>
        public JPTargetClassAttribute() : base(ParamType.TargetClass) { }
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains an <see cref="System.Object"/> representing the instance of the target class.
    /// </summary>
    /// <remarks>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointParameterExample/*"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPTargetAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains an <see cref="System.Object"/> representing the instance of the target class.
        /// </summary>
        public JPTargetAttribute() : base(ParamType.Target) { }
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="System.Type"/> object representing the interwoven class.
    /// </summary>
    /// <remarks>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </remarks>
    /// <include file="Doc/JoinPointAttributes.xml" path="doc/JoinPointParameterExample/*"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPInterwovenClassAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains a <see cref="System.Type"/> object representing the interwoven class.
        /// </summary>
        public JPInterwovenClassAttribute() : base(ParamType.InterwovenClass) { }
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains the return value of the executed target class method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </para>
    /// <para>
    /// The <see cref="JPRetValAttribute">JPRetVal</see> attribute can be used in every aspect method except <see cref="Advice.Around"/> and <see cref="Advice.AfterThrowing"/>.
    /// If this attribute is used in the parameter list, the return value of the target method has to match the type of the parameter annotated with the <see cref="JPRetValAttribute">JPRetVal</see> attribute.
    /// If it's a generic type, it has to fullfill the constraints of the generic type.
    /// </para>
    /// </remarks>
    /// <example>
    /// This example shows how to overwrite the return value of an interwoven target class using the RetVal join-point parameter:
    /**
    <code>
        public class TargetClass
        {
            public virtual string foo(string s)
            {
                return s;
            }
        }

        public class RetValAspect : Aspect
        {
            [Call(Advice.AfterReturning)]
            [IncludeAll]
            public void foo([JPRetVal] ref string returnValue, string name)
            {
                returnValue = "foo";
            }
        }
     
     
        ...
        TargetClass t = Weaver.Create&lt;TargetClass&gt;(new RetValAspect());

        Console.WriteLine(t.foo("bar"));
        ...
     
        
        [Output]
        foo  
    </code> */
    /// </example>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPRetValAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains the return value of the executed target class method.
        /// </summary>
        public JPRetValAttribute() : base(ParamType.RetVal) { }
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains the <see cref="System.Exception"/>object (if there is one) thrown by the executed target class method.
    /// </summary>
    /// <remarks>
    /// Use this attribute if you want to retrieve information about your interwoven target. All join-point parameters must
    /// be declared at the beginning of the aspect method's parameter list.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPExceptionAttribute : JoinPointParameterAttribute
    {
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains the <see cref="System.Exception"/>object (if there is one) thrown by the executed target class method.
        /// </summary>
        public JPExceptionAttribute() : base(ParamType.Exception) { }
    }


    /// <summary>
    /// Defines the scope of a declared <see cref="JPVariableAttribute"/>.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// The member is only visible to the interwoven aspect methods and their base aspect types, but not to the target class and other interwoven aspects.
        /// </summary>
        Private = 0x010,
        /// <summary>
        /// The member is visible to all interwoven aspect methods and to the target class, where the member is declared as protected. (Default)
        /// </summary>
        Protected = 0x000,
        /// <summary>
        /// The member is visible to all interwoven aspect methods and to the target class, where the member is declared as public. 
        /// Additionally, the member is visible outside of target class - as expected.
        /// </summary>
        Public = 0x020,
        /// <summary>
        /// The member is used, if existing and accessible. Otherwise a new member is created. (Default)
        /// </summary>
        Virtual = 0x000,
        /// <summary>
        /// The member is overridden by the join-point member. If the member member doesn't exist in the target class, an <see cref="Loom.AspectWeaverException"/> is thrown.
        /// </summary>
        Override = 0x100,
        /// <summary>
        /// The member is a static member of the interwoven class.
        /// </summary>
        Static = 0x001,
        /// <summary>
        /// The member is only visible within the context of an interwoven target method. The weaver creates a local copy of the member for every interwoven target method.
        /// </summary>
        Local = 0x012,
        /// <summary>
        /// The member is only visible within the context of an interwoven target method call.
        /// </summary>
        Call = 0x014,
        /// <summary>
        /// The member will be generated for each target (default).
        /// </summary>
        Target = 0
    }

    /// <summary>
    /// Declares a parameter in an aspect method as a join-point parameter which contains the value or a reference (if it is a ref-Parameter) to a value of a join-point variable.
    /// </summary>
    /// <remarks>
    /// <para>Join-point variables are used to pass values, bound to an interwoven class, between different aspect classes and aspect methods, or to access member variables of the target class.</para>
    /// <para>
    /// Use this attribute if you want to declare a new variable that is bound to your interwoven target. 
    /// A join-point variable has a scope (see <see cref="Scope"/> enumeration), a type, and a unique name. The scope is defined
    /// by the first parameter of the <see cref="JPVariableAttribute"/> attribute. The name and type of the join-point variable is the name and type of the 
    /// parameter, which is declared as join-point variable. Parameters in different aspect methods will access the same 
    /// join-point variable, if they have the same name, the same type, and the same scope.
    /// </para>
    /// <para>
    /// Join-point variables with <see cref="Scope.Public"/> and <see cref="Scope.Protected"/> accessibility can override member variables of the target class.
    /// A member variable of the target class will be overridden, if the name, type, and accessibility of both is the same.
    /// Overriding means that every access to the join-point variable is mapped to an access of the member variable.
    /// </para>
    /// <para>
    /// If a join-point variable has a <see cref="Scope.Static"/> scope, the instance of the join-point variable belongs to the target class. Otherwise, 
    /// each instance of the target class will have its own instance of the join-point variable (default).
    /// </para>
    /// <para>
    /// A <see cref="Scope.Local"/> join-point variable belongs to an interwoven target method. If an aspect method that accesses a local join-point variable, 
    /// it will access the instance of the join-point variable wich maps to the currently executing target class method.
    /// </para>
    /// If a join-point variable is marked as <see cref="Scope.Override"/>, there must be a member variable within the target clas with the same name, type, and accessibility.
    /// <para>Using the the bitwise OR operator '|', a combination of the scope attributes is possible. The default scope is (<see cref="Scope.Protected"/> | <see cref="Scope.Virtual"/>).</para>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Scope</term>
    ///         <term><see cref="Scope.Static"/></term>
    ///         <term><see cref="Scope.Local"/></term>
    ///         <term><see cref="Scope.Virtual"/></term>
    ///         <term><see cref="Scope.Override"/></term>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="Scope.Private"/></term>
    ///         <term>ok</term>
    ///         <term>ok</term>
    ///         <term>X</term>
    ///         <term>X</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Protected"/></term>
    ///         <term>ok</term>
    ///         <term>X</term>
    ///         <term>ok</term>
    ///         <term>ok</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Public"/></term>
    ///         <term>ok</term>
    ///         <term>X</term>
    ///         <term>ok</term>
    ///         <term>ok</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Static"/></term>
    ///         <term></term>
    ///         <term>ok</term>
    ///         <term>ok</term>
    ///         <term>ok</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Local"/></term>
    ///         <term>ok</term>
    ///         <term></term>
    ///         <term>X</term>
    ///         <term>X</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Virtual"/></term>
    ///         <term>ok</term>
    ///         <term>X</term>
    ///         <term></term>
    ///         <term>X</term>
    ///     </item>
    ///     <item>
    ///         <term><see cref="Scope.Override"/></term>
    ///         <term>ok</term>
    ///         <term>X</term>
    ///         <term>X</term>
    ///         <term></term>
    ///     </item>
    /// </list>
    /// <para>Legend:</para>
    /// <para>ok: indicates a legal combination</para>
    /// <para>X: indicates an illegal combination</para>
    /// <para>
    /// All join-point parameters must be declared at the beginning of the aspect method's parameter list.
    /// </para>
    /// </remarks>
    /// <seealso cref="Scope"/>
    /// <example>
    /// This example shows how to use the variable join-point parameter to pass a member variable between different aspect classes:
    /**
    <code>
        [Aspect1]
        [Aspect2]
        public class TargetClass
        {
            public string message; 
     
            public virtual string foo(string s)
            {
                return String.Format("{0} {1}", message, s);
            }
            
            public virtual string bar(string s)
            {
                return String.Format("{0} {1}", s, message);
            }
        }
        
        public class Aspect1 : AspectAttribute
        {
            [Call(Advice.Before)]
            public void foo([JPVariable(Scope.Public|Scope.Override)] ref string message, string s)
            {
                message = s;
            }
        }
     
        public class Aspect2 : AspectAttribute
        {
            [Call(Advice.After)]
            [IncludeAll]
            public void catchafter([JPVariable(Scope.Public|Scope.Override)] ref string message, [JPVariable(Scope.Local)] ref int calls, string s)
            {
                calls++;
                Console.Writeline("message=\"{0}\", calls={1}", message, calls.ToString());
            }
        }
     
        ...
        TargetClass t = Weaver.Create&lt;TargetClass&gt;();
        Console.WriteLine(t.bar("bar"));
        Console.WriteLine(t.foo("foo"));
        Console.WriteLine(t.foo("bar"));
        ...
        
        // [Output]
        // bar
        // message="", calls=1
        // foo foo
        // message="foo", calls=1
        // bar foo
        // message="foo", calls=2
    </code> */
    /// </example>    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class JPVariableAttribute : Attribute, ICreateParameterMatch
    {
        Scope scope;
        /// <summary>
        /// Declares a parameter in an aspect method as a join-point parameter which contains the value or a reference (if it is a ref-Parameter) to a value of a join-point variable.
        /// </summary>
        /// <param name="scope">the scope of the join-point variable</param>
        public JPVariableAttribute(Scope scope)
        {
            this.scope = scope;
        }

        #region ICreateParameterMatch Members

        IParameterMatch ICreateParameterMatch.Create(ParameterInfo pi)
        {
            bool isStatic = (scope & Scope.Static) != 0;
            bool isPrivate = (scope & Scope.Private) != 0;
            bool isPublic = (scope & Scope.Public) != 0;
            bool isOverride = (scope & Scope.Override) != 0;

            if ((isStatic && isOverride) ||
                (isPrivate && isPublic) ||
                (isPrivate && isOverride))
                throw new AspectWeaverException(19, Loom.Resources.Errors.ERR_0019, pi.Name, Convert.ToString(pi.Member));

            return new AspectParameter(pi, ParamType.Variable, scope);
        }

        #endregion
    }
}
