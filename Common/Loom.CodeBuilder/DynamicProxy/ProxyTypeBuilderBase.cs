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


using Loom.WeaverMessages;
using Loom.Common;
using Loom.AspectModel;
using Loom.Runtime;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.Declarations;


namespace Loom.CodeBuilder.DynamicProxy
{
    internal abstract class ProxyTypeBuilderBase : InterwovenTypeBuilder
    {

        // Verwebung
        protected ModuleBuilder targetmodule;
        protected TypeBuilder typebuilder;
        protected GlobalAspectContainer globalaspectcontainer;
        private int contextclasses = 0;

        /// <summary>
        /// Container für alle zusaetzlichen Deklarationen im Proxy, auf die MethodBuilder bei der Verwebung Zugriff haben müssen
        /// </summary>
        protected DeclarationCollection declarations;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetmodule"></param>
        public ProxyTypeBuilderBase(ModuleBuilder targetmodule)
        {
            this.targetmodule = targetmodule;
            this.declarations = new DeclarationCollection();
        }

        public ModuleBuilder TargetModule
        {
            get { return targetmodule; }
        }

        public TypeBuilder TypeBuilder
        {
            get { return typebuilder; }
        }

        public abstract Type AspectType
        {
            get;
        }
        /// <summary>
        /// Basisklasse des neuen Typs
        /// </summary>
        public abstract Type BaseType
        {
            get;
        }
        /// <summary>
        /// Zielklasse
        /// </summary>
        public abstract Type TargetClass
        {
            get;
        }

        public DeclarationCollection Declarations
        {
            get { return declarations; }
        }

        public GlobalAspectContainer GlobalAspectContainer
        {
            get
            {
                if (globalaspectcontainer == null)
                {
                    globalaspectcontainer = new GlobalAspectContainer(this.targetmodule);
                }
                return globalaspectcontainer;
            }
        }

        public string GetContextClassName()
        {
            contextclasses++;
            return Common.WeavingCodeNames.c_baseCallerClassName + contextclasses.ToString();
        }

        /// <summary>
        /// Factories
        /// </summary>
        /// <param name="meb"></param>
        /// <returns></returns>
        public abstract ContextClassDeclaration CreateIntroduceContextClassDeclaration(ProxyIntroducedMethodBuilder meb);
        public abstract ContextClassDeclaration CreateCallContextClassDeclaration(ProxyAdviceMethodBuilder meb);

    }
}
