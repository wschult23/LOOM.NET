// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections;
using System.Text;

using Loom.Common;

namespace Loom.Common
{
    internal class Strings
    {
        /// <summary>
        /// Converts an object firstparam an unique string
        /// </summary>
        /// <param name="o">the object</param>
        /// <returns>a unique string</returns>
        internal static string GetHashString(object o)
        {
            return String.Format("{0:X8}_", o.GetHashCode());
        }

        internal static string GetStaticMethodInfoFieldName(MethodInfo mi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(WeavingCodeNames.c_baseStaticMethodInfoField);
            sb.Append(GetHashString(mi));
            sb.Append(mi.Name);
            return sb.ToString();
        }

        internal static string GetBaseCallerMethodName(MethodInfo mi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(WeavingCodeNames.c_baseCallerMethodName);
            sb.Append(GetHashString(mi));
            sb.Append(mi.Name);
            return sb.ToString();
        }

        internal static string GetInterfaceMethodName(MethodInfo mi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(mi.DeclaringType.FullName);
            sb.Append(System.Reflection.Emit.TypeBuilder.Delimiter);
            sb.Append(mi.Name);
            return sb.ToString();
        }

        internal static string GetInterfaceMethodImplName(MethodInfo mi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(WeavingCodeNames.c_interfaceMethodName);
            sb.Append(GetHashString(mi.DeclaringType));
            sb.Append(mi.Name);
            return sb.ToString();
        }
    }
}
