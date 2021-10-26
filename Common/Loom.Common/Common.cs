// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Text;
using Loom.Common.Properties;


namespace Loom.Common
{
    internal class Resolver
    {
        /// <summary>
        /// Liefert aus einem Namen ein Typ
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="assemblyscope"></param>
        /// <returns></returns>
        public static Type ResolveType(string typename, Assembly assemblyscope)
        {
            Type type = null;
            if (typename.Contains(","))
            {
                type = Type.GetType(typename);
            }
            else
            {
                type = assemblyscope.GetType(typename);
                if (type == null)
                {
                    type = Type.GetType(typename);
                }
            }
            if (type == null)
            {
                throw new ArgumentException(string.Format(Resources.ERRMSG_TypeNotFound, typename));
            }
            return type;
        }
    }

    internal class Convert
    {
        public static string ToString(MemberInfo mi)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Constructor:
                    return ToString((ConstructorInfo)mi);
                case MemberTypes.Method:
                    return ToString((MethodInfo)mi);
                case MemberTypes.NestedType:
                case MemberTypes.TypeInfo:
                    return ToString((Type)mi);
                case MemberTypes.Property:
                    return ToString((PropertyInfo)mi);
                case MemberTypes.Event:
                    return ToString((EventInfo)mi);
                default:
                    return mi.ToString();
            }
        }

        public static string ToString(MethodInfo mimethod)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ToString(mimethod.ReturnType));
            sb.Append(' ');
            sb.Append(ToString(mimethod.DeclaringType));
            sb.Append('.');
            sb.Append(mimethod.Name);
            if (mimethod.IsGenericMethod)
            {
                sb.Append('<');
                bool bfirst = true;
                foreach (Type t in mimethod.GetGenericArguments())
                {
                    if (!bfirst)
                    {
                        sb.Append(", ");
                    }
                    else
                    {
                        bfirst = false;
                    }
                    sb.Append(t.Name);
                }
                sb.Append('>');
            }
            sb.Append('(');
            sb.Append(ToString(mimethod.GetParameters()));
            sb.Append(')');

            return sb.ToString();
        }

        public static string ToString(ConstructorInfo ciconstructor)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ToString(ciconstructor.DeclaringType));
            sb.Append('.');
            sb.Append(ciconstructor.DeclaringType.Name.Replace('+', '.'));
            sb.Append('(');
            sb.Append(ToString(ciconstructor.GetParameters()));
            sb.Append(')');

            return sb.ToString();
        }

        public static string ToString(ParameterInfo[] piparameters)
        {
            StringBuilder sb = new StringBuilder();
            bool bFirst = true;
            foreach (ParameterInfo pi in piparameters)
            {
                if (!bFirst)
                {
                    sb.Append(", ");
                }
                else
                {
                    bFirst = false;
                }
                Type t = pi.ParameterType;
                if (t.IsByRef)
                {
                    sb.Append("ref ");
                    t = t.GetElementType();
                }
                if (t.IsGenericParameter)
                {
                    sb.Append(pi.ParameterType.Name);
                }
                else
                {
                    sb.Append(ToString(t));
                }
            }
            return sb.ToString();
        }

        public static string ToString(PropertyInfo pi)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ToString(pi.DeclaringType));
            sb.Append('.');
            sb.Append(pi.Name);
            return sb.ToString();
        }

        public static string ToString(EventInfo ei)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ToString(ei.DeclaringType));
            sb.Append('.');
            sb.Append(ei.Name);
            return sb.ToString();
        }

        public static string ToString(Type type)
        {
            return type.Name;
        }

        public static string ToString(Enum en)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(en.GetType().Name);
            sb.Append('.');
            sb.Append(en.ToString());
            return sb.ToString();
        }

        internal static string ToString(Type[] types)
        {
            StringBuilder sb = new StringBuilder();
            bool bFirst = true;
            foreach (var t in types)
            {
                if (!bFirst)
                {
                    sb.Append(", ");
                }
                else
                {
                    bFirst = false;
                }
                sb.Append(ToString(t));
            }
            return sb.ToString();
        }
    }

}
