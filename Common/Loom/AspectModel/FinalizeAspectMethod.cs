// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Loom.CodeBuilder;
using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;


namespace Loom.AspectModel
{
    internal class FinalizeAspectMethod : AspectMember
    {
        private IList<IParameterMatch> cpparameter;
        private IList<IJoinPointSelector> cpweaver;
        Advice invokeorder;

        internal FinalizeAspectMethod(MethodInfo mi, FinalizeAttribute destroy)
            :
            base(mi)
        {
            invokeorder = destroy.InvokeOrder;

            if (Method.ReturnType != typeof(void))
            {
                throw new AspectWeaverException(11, Loom.Resources.Errors.ERR_0011, Common.Convert.ToString(Method), typeof(FinalizeAttribute).Name.Replace("Attribute", ""));
            }

            cpweaver = CreateConnectionPointMatchList();
            if (cpweaver.Count == 0)
            {
                cpweaver.Add(IncludeSimpleMatchAll.GetObject());
                cpparameter = CreateParameterMatchList(true);
            }
            else
            {
                cpparameter = CreateParameterMatchList(false);
            }
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
            DestroyMethodJoinPoint jpt = jp as DestroyMethodJoinPoint;
            if (jpt == null) return AspectMemberMatchResult.NoMatch;

            // Passts?
            return IsParameterMatch(cpparameter, null, jpt.Method) ? IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
        }

        public override void Interweave(MethodCodeBuilder mcb)
        {
            CreateAspectCallCodeBrick(mcb, cpparameter, invokeorder);
        }

    }
}
