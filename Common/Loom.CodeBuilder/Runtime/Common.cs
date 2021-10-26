// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Loom.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Common
    {
        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public static void AddAspect(Aspect aspect, ArrayList al, Type aspecttype)
        {
            if (aspecttype == null)
            {
                al.Add(aspect);
            }
            else
            {
                Type tp = aspect.GetType();
                if (tp == aspecttype || tp.IsSubclassOf(aspecttype))
                {
                    al.Add(aspect);
                }
            }
        }

        private static object c_syncobj = new Object();

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public static Delegate CreateDelegate(object target, Type type, MethodInfo mi, Type[] generics, ref Dictionary<Type, Delegate> ht)
        {
            Delegate res = null;
            if (ht == null)
            {
                lock (c_syncobj)
                {
                    if (ht == null)
                    {
                        ht = new Dictionary<Type, Delegate>();
                    }
                }
            }
            else
            {
                if (ht.TryGetValue(type, out res))
                {
                    return res;
                }
            }
            lock (c_syncobj)
            {
                if (ht.TryGetValue(type, out res))
                {
                    return res;
                }
                if (mi.IsGenericMethodDefinition)
                {
                    res = Delegate.CreateDelegate(type, target, mi.MakeGenericMethod(generics));
                }
                else
                {
                    res = Delegate.CreateDelegate(type, target, mi);
                }
                ht.Add(type, res);
                return res;
            }
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public static Delegate CreateDelegate(object target, Type type, MethodInfo mi, ref Delegate del)
        {
            // TODO: Das ist nicht ThreadSafe
            if (del == null)
            {
                lock (c_syncobj)
                {
                    if (del != null) return del;
                    del = Delegate.CreateDelegate(type, target, mi);
                }
            }
            return del;
        }

        /// <summary>
        /// This method is for internal purposes only! Do never call this method directly from your code!
        /// </summary>
        public static void ReplaceTypedNull(object[] args)
        {
            for (int iPos = 0; iPos < args.Length; iPos++)
            {
                TypedValue tv = args[iPos] as TypedValue;
                if (tv != null)
                {
                    args[iPos] = tv.Value;
                }
            }
        }
    }

    /// <summary>
    /// This delegate is for internal purposes only! Do never access this delegate directly from your code!
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate object RecallDelegate(object instance, object[] args);

}
