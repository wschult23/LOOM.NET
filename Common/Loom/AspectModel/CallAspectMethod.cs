// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder;

namespace Loom.AspectModel
{
    /// <summary>
    /// Summary description for AspectMethodCall.
    /// </summary>
    // container for call potential connection points
    internal class CallAspectMethod : AspectMethod
    {
        public CallAspectMethod(MethodInfo mi, CallAttribute call)
            :
            base(mi, call.InvokeOrder)
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
            MethodJoinPoint jpt = jp as MethodJoinPoint;
            if (jpt == null) return AspectMemberMatchResult.NoMatch;

            // Passts?
            return IsParameterMatch(cpparameter, cprestype, jpt.Method) ? IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
        }
    }

}
