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

using Loom.CodeBuilder;
using Loom.AspectModel;

namespace Loom.JoinPoints.Implementation
{

    /// <summary>
    /// überpräft, ob eine Aspektmethode mit einer Zielmethode verwoben werden kann.
    /// Wird bei von den <see cref="IJoinPointSelector"/> Attributen für die konkrete Implementation
    /// dieses Interfaces verwendet.
    /// </summary>
    internal interface IJoinPointSelectorInternal
    {
        bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods);
    }

    /// <summary>
    /// Wird von IncludeAttribute und ExcludeAttribute verwendet zum Stringvergleich von Methoden
    /// </summary>
    internal class MethodNameMatch : IJoinPointSelectorInternal
    {
        private static Regex wctoregexp = new Regex("([.^$+{}()\\[\\]])", RegexOptions.Compiled);
        private Regex regex;
        public MethodNameMatch(string match)
        {
            StringBuilder sb = new StringBuilder();
            string methodexp = wctoregexp.Replace(match, "\\$1").Replace("*", ".*").Replace("?", ".");
            sb.Append("^");
            sb.Append(methodexp);
            sb.Append("$");
            this.regex = new Regex(sb.ToString(), RegexOptions.Compiled);
        }
        public bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodJoinPoint jptm = jp as MethodJoinPoint;
            if (jptm != null)
            {
                if (regex.IsMatch(jptm.Method.Name)) return true;
                if (jptm.MethodInterface != null)
                {
                    if (regex.IsMatch(jptm.MethodInterface.Name)) return true;
                }
            }
            else
            {
                PropertyJoinPoint jptp = jp as PropertyJoinPoint;
                if (jptp != null)
                {
                    if (regex.IsMatch(jptp.PropertyInfo.Name)) return true;
                }
            }
            return false;
        }
    }

    internal class CombinedMatch : IJoinPointSelectorInternal
    {
        private IJoinPointSelectorInternal[] matches;
        public CombinedMatch(params IJoinPointSelectorInternal[] matches)
        {
            this.matches = matches;
        }

        public bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            foreach (IJoinPointSelectorInternal jps in matches)
            {
                if (!jps.IsMatch(jp, pass, interwovenmethods)) return false;
            }
            return true;
        }
    }

    internal class InterfaceMatch : IJoinPointSelectorInternal
    {
        private Type type;
        public InterfaceMatch(Type type)
        {
            this.type = type;
        }
        public bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodInfoJoinPoint jpt = jp as MethodInfoJoinPoint;
            if (jpt.MethodInterface == null) return false;
            return jpt.MethodInterface.DeclaringType == type;
        }
    }

    internal class TypeMatch : IJoinPointSelectorInternal
    {
        private Type type;
        public TypeMatch(Type type)
        {
            this.type = type;
        }
        public bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return jp.DeclaringType == type;
        }
    }

    internal class AttributeMatch : IJoinPointSelectorInternal
    {
        private Type type;
        private bool bInherit;
        public AttributeMatch(Type type)
        {
            this.type = type;
            this.bInherit = false;
        }
        public AttributeMatch(Type type, bool bInherit)
        {
            this.type = type;
            this.bInherit = bInherit;
        }
        public bool IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodBaseJoinPoint jptb = jp as MethodBaseJoinPoint;
            if (jptb != null && jptb.Method.GetCustomAttributes(type, bInherit).Length > 0) return true;
            MethodInfoJoinPoint jpti = jp as MethodInfoJoinPoint;
            if (jpti != null && jpti.MethodInterface != null && jpti.MethodInterface.GetCustomAttributes(type, bInherit).Length > 0) return true;
            return false;
        }
    }

    /// <summary>
    /// This class is used if there are no interweaving attributes defined
    /// </summary>
    internal class IncludeSimpleMethodMatch : IJoinPointSelector
    {
        private MethodInfo mi;
        public IncludeSimpleMethodMatch(MethodInfo mi)
        {
            this.mi = mi;
        }

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodInfoJoinPoint jpt = jp as MethodInfoJoinPoint;
            if (jpt == null) return JoinPointSelectorResult.Undecided;

            if (jpt.Method.Name == mi.Name)
            {
                return JoinPointSelectorResult.Match;
            }
            if (jpt.MethodInterface != null)
            {
                if (jpt.MethodInterface.Name == mi.Name) return JoinPointSelectorResult.Match;
            }
            return JoinPointSelectorResult.NoMatch;
        }
    }

    /// <summary>
    /// This class is used if there are no interweaving attributes defined
    /// </summary>
    internal class AlwaysTrueMatch : Loom.Common.Singleton<AlwaysTrueMatch>, IJoinPointSelector
    {
        public AlwaysTrueMatch()
        {
        }

        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return JoinPointSelectorResult.Match;
        }
    }


    /// <summary>
    /// This class is used if there are no interweaving attributes defined
    /// </summary>
    internal class IncludeSimpleMatchAll : Common.Singleton<IncludeSimpleMatchAll>, IJoinPointSelector
    {
        /// <summary>
        /// Bei Construktoren erfolgt der Match ausschlieälich über Parameter
        /// </summary>
        /// <returns></returns>
        JoinPointSelectorResult IJoinPointSelector.IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            return JoinPointSelectorResult.Match;
        }
    }

}