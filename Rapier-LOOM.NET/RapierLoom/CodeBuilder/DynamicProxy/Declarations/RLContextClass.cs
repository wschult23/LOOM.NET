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
using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.Declarations;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.JoinPoints.Implementation;
using Loom.Common;

namespace Loom.CodeBuilder.Gripper.DynamicProxy.Declarations
{
    internal class RLIntroduceContextClassDeclaration : IntroduceContextClassDeclaration
    {
        public RLIntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb) :
            base(meb)
        {
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            cicontext = ContextClassImpl.DefineIntroductionContextClass(meb, out typebuilder);
            ContextClassImpl.Impl_ReCallIndirect(typebuilder);
        }
    }

    /// <summary>
    /// Deklaration ...
    /// </summary>
    internal class RLCallContextClassDeclaration : CallContextClassDeclaration
    {
        public RLCallContextClassDeclaration(ProxyAdviceMethodBuilder meb)
            : base(meb)
        {
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            cicontext = ContextClassImpl.DefineCallContextClass(meb, out typebuilder);
            ContextClassImpl.Impl_ReCallIndirect(typebuilder);
        }
    }
}