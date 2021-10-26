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
    internal class CreateAspectMethod : AspectMember
    {
        /// <summary>
        /// the create Attribute
        /// </summary>
        private CreateAttribute create;
        /// <summary>
        /// a list of IJoinPointSelector objects
        /// </summary>
        private IList<IJoinPointSelector> cpweaver;
        private IList<IParameterMatch> cpparameter;
        private IResultTypeMatch cptype;


        public CreateAspectMethod(MethodInfo mi, CreateAttribute create) :
            base(mi)
        {
            this.create = create;

            // Vergleichsobjekte für CP
            cpweaver = CreateConnectionPointMatchList();
            if (cpweaver.Count == 0)
            {
                cpweaver.Add(IncludeSimpleMatchAll.GetObject());
                cpparameter = CreateParameterMatchList(true);
            }
            else
            {
                // Vergleichsobjekte für Parameter (ein impliziter Parameter - der Typ)
                cpparameter = CreateParameterMatchList(false);
            }

            // Der Rückgabetyp bei Create ist nie ein SimpleMatch!
            cptype = CreateResultTypeMatch(cpparameter, create.InvokeOrder, false);
        }

        /// <summary>
        /// Vergleicht, ob Methode in obj mit AspectMember übereinstimmt
        /// </summary>
        /// <param name="jp"></param>
        /// <param name="pass"></param>
        /// <param name="interwovenmethods"></param>
        /// <returns></returns>
        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            // nur Konstruktoren testen
            ConstructorJoinPoint jpt = jp as ConstructorJoinPoint;
            if (jpt == null) return AspectMemberMatchResult.NoMatch;

            // schauen, ob die Aspektmethode den richtigen Typ zurückgeben kann
            return IsParameterMatch(cpparameter, cptype, jpt.Method) ? IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
        }

        public override void Interweave(MethodCodeBuilder mcb)
        {
            CreateAspectCallCodeBrick(mcb, cpparameter, create.InvokeOrder);
        }
    }
}

