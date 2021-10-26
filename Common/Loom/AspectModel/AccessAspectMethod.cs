// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;

namespace Loom.AspectModel
{
    internal class AccessAspectMethod : AspectMethod
    {
        public AccessAspectMethod(MethodInfo mi, AccessAttribute access)
            :
            base(mi, access.InvokeOrder)
        {
        }

        /// <summary>
        /// Vergleicht, ob Methode in obj mit AspectMember übereinstimmt
        /// </summary>
        /// <param name="jp">Der Joinpoint</param>
        /// <param name="interwovenmethods">bisher verwoben</param>
        /// <param name="pass">der wievielte Matchdurchlauf</param>
        /// <returns></returns>
        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            // nur Methoden testen
            PropertyJoinPoint jpt = jp as PropertyJoinPoint;
            if (jpt == null) return AspectMemberMatchResult.NoMatch;

            // Passts?
            return IsParameterMatch(cpparameter, cprestype, jpt.Method) ? IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
        }
    }
}
