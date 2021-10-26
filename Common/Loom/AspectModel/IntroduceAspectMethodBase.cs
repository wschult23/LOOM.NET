// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Collections.Generic;

using Loom.CodeBuilder;
using Loom.JoinPoints;
using Loom.JoinPoints.Implementation;

namespace Loom.AspectModel
{
    /// <summary>
    /// Summary description for IntroduceAspectMethod.
    /// </summary>
    internal abstract class IntroduceAspectMethodBase : AspectMember
    {
        protected IList<IJoinPointSelector> jpweaver;
        protected IResultTypeMatch jprestype;
        protected IList<IParameterMatch> jpparameter;
        protected Type iftype;

        /// <summary>
        /// Returns the introduced interface
        /// </summary>
        public Type Interface
        {
            get { return iftype; }
        }

        public IntroduceAspectMethodBase(MethodInfo aspectmethod, Type iftype, ExistingInterfaces existinginterfaces) :
            base(aspectmethod)
        {
            this.iftype = iftype;

        }
    }

    internal class IntroduceAspectProperty : IntroduceAspectMethod
    {
        public IntroduceAspectProperty(MethodInfo aspectmethod, Type iftype, ExistingInterfaces existinginterfaces)
            :
            base(aspectmethod, iftype, existinginterfaces)
        {
        }

        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            if (jp is PropertyJoinPoint)
            {
                return base.IsMatch(jp, pass, interwovenmethods);
            }
            return AspectMemberMatchResult.NoMatch;
        }
    }

    internal class IntroduceAspectEvent : IntroduceAspectMethod
    {
        public IntroduceAspectEvent(MethodInfo aspectmethod, Type iftype, ExistingInterfaces existinginterfaces)
            :
            base(aspectmethod, iftype, existinginterfaces)
        {
        }

        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            if (jp is EventJoinPoint)
            {
                return base.IsMatch(jp, pass, interwovenmethods);
            }
            return AspectMemberMatchResult.NoMatch;
        }
    }
}
