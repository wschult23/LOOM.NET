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
    /// <summary>
    /// Summary description for Conversion.
    /// </summary>
    internal class Conversion
    {
        /// <summary>
        /// returns an array of parameter types
        /// </summary>
        /// <param name="piarr">array of parameter</param>
        /// <returns>array of parameter types</returns>
        public static Type[] ToTypeArray(ParameterInfo[] piarr)
        {
            Type[] res = new Type[piarr.Length];
            for (int i = 0; i < piarr.Length; i++)
            {
                res[i] = piarr[i].ParameterType;
            }
            return res;
        }

        [Obsolete("use Type.GetTypeArray")]
        public static Type[] ToTypeArray(object[] obarr)
        {
            Type[] res = new Type[obarr.Length];
            for (int i = 0; i < obarr.Length; i++)
            {
                res[i] = obarr[i].GetType();
            }
            return res;
        }

        /// <summary>
        /// Liefert den gebundenen Rückgabe-Typen fär eine generische Methode
        /// </summary>
        /// <param name="mb">die Methode</param>
        /// <param name="generics">die generischen Parameter</param>
        /// <returns></returns>
        public static Type ResolveReturnType(MethodInfo mb, Type[] generics)
        {
            Type rettype = mb.ReturnType;
            if (rettype.IsGenericParameter && generics != null) rettype = generics[rettype.GenericParameterPosition];
            return rettype;
        }

        public static object ToObject(Type primitivetype, string value)
        {
            System.Diagnostics.Debug.Fail("do not use");
            MethodInfo mi = primitivetype.GetMethod("Parse", new Type[] { typeof(string) });
            if (mi == null)
            {
                throw new ArgumentException(string.Format(Properties.Resources.ERRMSG_InvalidType, primitivetype.FullName));
            }
            try
            {
                return mi.Invoke(null, new object[] { value });
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}
