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
    internal class IntroduceAspectMethod : IntroduceAspectMethodBase
    {


        protected ExistingInterfaces existinginterfaces;

        public IntroduceAspectMethod(MethodInfo aspectmethod, Type iftype, ExistingInterfaces existinginterfaces)
            : base(aspectmethod, iftype, existinginterfaces)
        {
            if (!IsIfcPublic(iftype))
            {
                throw new AspectWeaverException(14, Resources.Errors.ERR_0014, Common.Convert.ToString(aspectmethod), Common.Convert.ToString(iftype));
            }

            this.existinginterfaces = existinginterfaces;

            // Vergleichsobjekte für Name
            this.jpweaver = CreateConnectionPointMatchList();

            // wenn keins definiert, dann diesen Methodennamen
            bool bSimple = this.jpweaver.Count == 0;
            if (bSimple)
            {
                this.jpweaver.Add(new IncludeSimpleMethodMatch(aspectmethod));
            }

            this.jpparameter = CreateParameterMatchList(bSimple);
            this.jprestype = CreateResultTypeMatch(jpparameter, Advice.Around, bSimple);
        }

        private bool IsIfcPublic(Type ifc)
        {
            if (ifc.IsPublic) return true;
            if (ifc.IsNested && ifc.IsNestedPublic) return IsIfcPublic(ifc.DeclaringType);
            return false;
        }


        /// <summary>
        /// Eine Introduction darf nicht mit einer Zielmethode verwoben werden
        /// </summary>
        /// <param name="jp"></param>
        /// <param name="pass"></param>
        /// <param name="interwovenmethods"></param>
        /// <returns></returns>
        public override AspectMemberMatchResult IsMatch(JoinPoint jp, int pass, AspectMemberCollection interwovenmethods)
        {
            MethodInfoJoinPoint jpt = (MethodInfoJoinPoint)jp;
            if (jpt.MethodInterface == null) return AspectMemberMatchResult.NoMatch;
            if (jpt.MethodInterface.DeclaringType != iftype) return AspectMemberMatchResult.NoMatch;
            if (jpt.MethodImplementation != null)
            {
                switch (existinginterfaces)
                {
                    case ExistingInterfaces.Skip: return AspectMemberMatchResult.NoMatch;
                    case ExistingInterfaces.Error: throw new AspectWeaverException(3, Loom.Resources.Errors.ERR_0003, Common.Convert.ToString(aspectmethod), jpt.DeclaringType.FullName, jpt.MethodInterface.DeclaringType.FullName, typeof(ExistingInterfaces).FullName + "." + ExistingInterfaces.Advice.ToString());
                }
            }

            return IsParameterMatch(jpparameter, jprestype, jpt.MethodInterface) ? IsJoinPointMatch(jpweaver, jp, pass, interwovenmethods) : AspectMemberMatchResult.NoMatch;
        }

        public override void Interweave(MethodCodeBuilder mcb)
        {
            CreateAspectCallCodeBrick(mcb, jpparameter, Advice.Around);
        }


    }
}
