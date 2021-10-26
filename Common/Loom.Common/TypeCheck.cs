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

namespace Loom.Common
{
    internal static class TypeCheck
    {
        public static bool IsEqualOrSubclassOrHasInterfaceOf(this Type tp, Type type)
        {
            if (tp==type || tp.IsSubclassOf(type))
            {
                return true;
            }

            return tp.HasInterfaceOf(type);
        }

        public static bool HasInterfaceOf(this Type tp, Type type)
        {
            foreach (var ifc in tp.GetInterfaces())
            {
                if (ifc == type || ifc.HasInterfaceOf(type))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Liefert den Typen der sowohl auf denZieltypen als auch auf einen Typen mit Constraints passt
        /// </summary>
        /// <param name="typewithconstraints">Der Typ in den targettyp passen soll</param>
        /// <param name="targettype">Der Typ, der geliefert wird</param>
        /// <returns></returns>
        public static Type GetCommonType(Type typewithconstraints, Type targettype)
        {
            if (typewithconstraints.IsGenericParameter)
            {
                if (targettype.IsClass)
                {
                    foreach (var con in typewithconstraints.GetGenericParameterConstraints())
                    {
                        if (con.IsSubclassOf(targettype))
                        {
                            targettype = con;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var con in typewithconstraints.GetGenericParameterConstraints())
                    {
                        if (con.HasInterfaceOf(targettype))
                        {
                            targettype = con;
                            break;
                        }
                    }
                }
            }
            return targettype;
        }
    }
}
