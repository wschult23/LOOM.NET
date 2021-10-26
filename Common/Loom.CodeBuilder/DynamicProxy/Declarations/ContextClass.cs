// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Loom.CodeBuilder.DynamicProxy;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;

namespace Loom.CodeBuilder.DynamicProxy.Declarations
{
    /// <summary>
    /// Deklaration einer Kontextklasse
    /// </summary>
    internal abstract class ContextClassDeclaration : Declaration
    {
        protected ConstructorInfo cicontext;
        protected TypeBuilder typebuilder;

        public override void CreateType()
        {
            typebuilder.CreateType();
        }

        public ConstructorInfo ConstructorInfo
        {
            get
            {
                return cicontext;
            }
        }
    }

    /// <summary>
    /// Deklaration ...
    /// </summary>
    internal abstract class CallContextClassDeclaration : ContextClassDeclaration
    {
        protected ProxyAdviceMethodBuilder meb;
        public CallContextClassDeclaration(ProxyAdviceMethodBuilder meb)
        {
            this.meb = meb;
        }
    }

    internal class CreateContextClassDeclaration : ContextClassDeclaration
    {
        ProxyAdviceConstructorBuilder meb;
        public CreateContextClassDeclaration(ProxyAdviceConstructorBuilder meb)
        {
            this.meb = meb;
        }

        public override void DefineMember(ProxyTypeBuilderBase tb)
        {
            cicontext = ContextClassImpl.DefineCreateContextClass(meb, out typebuilder);
        }
    }

    internal abstract class IntroduceContextClassDeclaration : ContextClassDeclaration
    {
        protected ProxyIntroducedMethodBuilder meb;
        public IntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb)
        {
            this.meb = meb;
        }

    }

}
