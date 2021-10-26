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

using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder;

namespace Loom.AspectModel
{
    internal abstract class AspectMethod : AspectMember
    {
        protected IList<IJoinPointSelector> cpweaver;
        protected IResultTypeMatch cprestype;
        protected IList<IParameterMatch> cpparameter;
        protected Advice invokeorder;


        public AspectMethod(MethodInfo mi, Advice invokeorder)
            :
            base(mi)
        {
            this.invokeorder = invokeorder;
            // Vergleichsobjekte für Name
            this.cpweaver = CreateConnectionPointMatchList();
            // wenn keins definiert, dann diesen Methodennamen

            bool bSimple = this.cpweaver.Count == 0;
            if (bSimple)
            {
                this.cpweaver.Add(new IncludeSimpleMethodMatch(mi));
            }


            // Vergleichsobjekte für Parameter
            this.cpparameter = CreateParameterMatchList(bSimple);
            // Rückgabewert
            this.cprestype = CreateResultTypeMatch(cpparameter, invokeorder, bSimple);
        }

        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            bool bRes = false;
            MethodBaseJoinPoint jpt = jp as MethodBaseJoinPoint;
            if (jpt == null) return AspectMemberMatchResult.NoMatch;

            if (jp is MethodInfoJoinPoint)
            {
                bRes = IsParameterMatch(cpparameter, cprestype, jpt.Method);
            }
            else if (jp is ConstructorJoinPoint)
            {
                bRes = IsParameterMatch(cpparameter, null, jpt.Method);
            }
            if (!bRes) return AspectMemberMatchResult.NoMatch;

            return IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods);
        }

        public override void Interweave(MethodCodeBuilder mcb)
        {
            CreateAspectCallCodeBrick(mcb, cpparameter, invokeorder);
        }

    }

    /// <summary>
    /// Wird für Error und Warning verwendet
    /// </summary>
    internal abstract class CompilerSpecialAspectMethod : AspectMember
    {
        protected IList<IJoinPointSelector> cpweaver;
        protected IResultTypeMatch cprestype;
        protected IList<IParameterMatch> cpparameter;
        protected TargetTypes targettypes;


        public CompilerSpecialAspectMethod(MethodInfo mi, TargetTypes targettypes)
            :
            base(mi)
        {
            this.targettypes = targettypes;

            // Methode

            // Vergleichsobjekte für Name
            this.cpweaver = CreateConnectionPointMatchList();
            // wenn keins definiert, dann diesen Methodennamen

            bool bSimple = this.cpweaver.Count == 0;
            if (bSimple)
            {
                switch (targettypes)
                {
                    case TargetTypes.Call:
                        this.cpweaver.Add(new IncludeSimpleMethodMatch(mi)); break;
                    case TargetTypes.Create:
                        this.cpweaver.Add(IncludeSimpleMatchAll.GetObject()); break;
                }
            }


            // Vergleichsobjekte für Parameter
            this.cpparameter = CreateParameterMatchList(bSimple);
            // Rückgabewert
            this.cprestype = CreateResultTypeMatch(cpparameter, Advice.Before, bSimple && (targettypes == TargetTypes.Call));
        }

        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            switch (targettypes)
            {
                case TargetTypes.Call:
                    {
                        bool bRes = false;
                        MethodBaseJoinPoint jpt = jp as MethodBaseJoinPoint;
                        if (jpt == null) return AspectMemberMatchResult.NoMatch;

                        if (jp is MethodInfoJoinPoint)
                        {
                            bRes = IsParameterMatch(cpparameter, cprestype, jpt.Method);
                        }
                        else if (jp is ConstructorJoinPoint)
                        {
                            bRes = IsParameterMatch(cpparameter, null, jpt.Method);
                        }
                        if (!bRes) return AspectMemberMatchResult.NoMatch;

                        return IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods);
                    }
                case TargetTypes.Create:
                    {
                        // nur Konstruktoren testen
                        ConstructorJoinPoint jpt = jp as ConstructorJoinPoint;
                        if (jpt == null) return AspectMemberMatchResult.NoMatch;

                        // schauen, ob die Aspektmethode den richtigen Typ zurückgeben kann
                        return IsParameterMatch(cpparameter, cprestype, jpt.Method) ? IsJoinPointMatch(cpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
                    }
            }
            return AspectMemberMatchResult.NoMatch;
        }
    }
}
