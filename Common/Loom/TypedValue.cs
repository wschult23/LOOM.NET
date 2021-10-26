// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Loom
{
    /// <summary>
    /// This class is for internal purposes only! Do never use this class directly from your code!
    /// </summary>
    public class TypedValue
    {
        

        /// <summary>
        /// Internal use only
        /// </summary>
        internal protected object val;

        internal TypedValue(object val)
        {
            this.val = val;
        }

        /// <summary>
        /// Determines whether the explicit typed value is a or derives from the class represented by the specified <see cref="System.Type"/>. 
        /// </summary>
        /// <param name="tp">The type to compare with the type of the current value</param>
        /// <returns><value>true</value> if the current explicit typed value is a or derives from class represented by the specified type <paramref name="tp"/>, <value>false</value> otherwise.</returns>
        public virtual bool IsType(Type tp)
        {
            return tp.IsInstanceOfType(val);
        }

        internal object Value
        {
            get
            {
                return val;
            }
        }

        /// <summary>
        /// Returns the Type
        /// </summary>
        internal virtual Type Type
        {
            get
            {
                return val.GetType();
            }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never use this method directly from your code!
        /// </summary>
        /// <param name="type">internal use.</param>
        /// <param name="value">internal use.</param>
        /// <returns>internal use.</returns>
        public static TypedValue Create(Type type, object value)
        {
            if (!type.IsClass || type.IsGenericType) throw new ArgumentException();
            return new RTTypedValue(type, value);
        }
    }

    /// <summary>
    /// This class is for internal purposes
    /// </summary>
    internal class RTTypedValue : TypedValue
    {
        private Type type;
        public RTTypedValue(Type type, object val) :
            base(val)
        {
            this.type = type;
        }

        public override bool IsType(Type tp)
        {
            return type == tp || tp.IsSubclassOf(type);
        }

        internal override Type Type
        {
            get
            {
                return this.type;
            }
        }
    }

    /// <summary>
    /// A class to express explicit typed values to prevent ambigous matches in <see cref="O:Loom.Weaver.Create"/>, <see cref="O:Loom.Weaver.CreateInstance"/> , <see cref="Loom.JoinPoints.Context.ReCall"/>, and <see cref="Loom.JoinPoints.Context.ReCallOn"/>.
    /// </summary>
    /// <example>
    /// This example shows how to use _T construct:
    /**
        <code>
        public class A 
        { 
            public A(A a) { } 

            public A(object o) { } 

            public A(string s) { } 
        } 

        ...
            object o = Weaver.Create&lt;A&gt;(_T&lt;string&gt;.Null);   // equiv. to new A((string)null); 
            A a = Weaver.Create&lt;A&gt;(_T&lt;A&gt;.Value(o));         // equiv. to new A((A)o); 
        ... 
            </code> */
    /// </example>
    /// <typeparam name="TYPE">the type</typeparam>
    public class _T<TYPE> : TypedValue where TYPE : class
    {
        _T(object val)
            : base(val)
        {
        }

        private static _T<TYPE> c_null = new _T<TYPE>(null);

        /// <summary>
        /// Returns a typed null value
        /// </summary>
        public static _T<TYPE> Null
        {
            get
            {
                return c_null;
            }
        }

        /// <summary>
        /// Returns an explicit typed object for <paramref name="value"/>. 
        /// </summary>
        /// <param name="value">The value to cast.</param>
        /// <returns>typed value for the given value</returns>
        public new static TypedValue Value(object value)
        {
            return new _T<TYPE>((TYPE)value);
        }

        internal override Type Type
        {
            get
            {
                return typeof(TYPE);
            }
        }

        /// <summary>
        /// Determines whether the explicit typed value is a or derives from the class represented by the specified <see cref="System.Type"/>. 
        /// </summary>
        /// <param name="tp">The type to compare with the type of the current value</param>
        /// <returns><value>true</value> if the current explicit typed value is a or derives from class represented by the specified type <paramref name="tp"/>, <value>false</value> otherwise.</returns>
        public override bool IsType(Type tp)
        {
            Type thisType = typeof(TYPE);
            return thisType == tp || thisType.IsSubclassOf(tp);
        }

        /// <summary>
        /// Returns the value represented by this object.
        /// </summary>
        /// <param name="t">an explicit typed value</param>
        /// <returns>the value represented by this object</returns>
        public static implicit operator TYPE(_T<TYPE> t) { return (TYPE)t.val; }
    }
}
