// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.CodeBuilder.DynamicProxy.DataSlots;

namespace Loom.CodeBuilder.DynamicProxy
{
    /// <summary>
    /// Das ist kein richtiger MethodBuilder sondern nur ein Container für die InitializerCodeBricks
    /// </summary>
    /// <remarks>
    /// Dieser Builder ist ein Hack um die Initializer zu implementieren. Der Kontext der Initializer kommt aus dem Builder, mit dem er verwoben ist,
    /// der Code wird jedoch jeweils in allen zum InterwovenType gehörenden Constructorbuilder implementiert
    /// 
    /// </remarks>
    internal class ProxyJPInitializerBuilder : ProxyMemberBuilder
    {
        private CodeBrick cb;
        private ProxyMemberBuilder pmbinterwoven;

        internal CodeBrick CodeBrick
        {
            get { return cb; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pmbinterwoven">Der Builder, mit dem der Initializer verwoben ist</param>
        /// <param name="aspectindex"></param>
        public ProxyJPInitializerBuilder(ProxyMemberBuilder pmbinterwoven, int aspectindex) :
            base(pmbinterwoven.DeclaringBuilder, pmbinterwoven.JoinPoint)
        {
            if (aspectindex >= 0)
            {
                this.DataSlots.Aspect = new AspectSlot(Declarations.Aspects[aspectindex]);
            }
            this.pmbinterwoven = pmbinterwoven;
        }

        internal void AddCodeBrick(CodeBrick codeBrick)
        {
            cb = codeBrick;
        }

        public override MethodBaseJoinPoint BindJoinPointToType()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Die Kontextklasse kommt aus dem zugehörigen Builder
        /// </summary>
        /// <returns></returns>
        public override Loom.CodeBuilder.DynamicProxy.Declarations.ContextClassDeclaration ContextClassDeclaration
        {
            get
            {
                return pmbinterwoven.ContextClassDeclaration;
            }
        }

        public override Dictionary<string, Loom.CodeBuilder.DynamicProxy.Declarations.JPVariableDeclaration> LocalSharedVariables
        {
            get
            {
                return pmbinterwoven.LocalSharedVariables;
            }
        }

        protected override Loom.CodeBuilder.DynamicProxy.Declarations.ContextClassDeclaration GetContextClassDeclaration()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Emit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override CodeBrickBuilder DefineAspectCall(System.Reflection.MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Hack um den Initializern den "richtigen" IL-Builder zu zeigen
        /// dies ist notwendig, da erst beim Emit die generics gebunden werden kännen
        /// </summary>
        /// <param name="proxyConstructorBuilder"></param>
        internal void SetEmitContext(ProxyConstructorBuilder proxyConstructorBuilder)
        {
            ilgen = proxyConstructorBuilder.ILGenerator;
        }
    }
}
