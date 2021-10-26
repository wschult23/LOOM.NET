// ----------------------------------------------------------------
// * The LOOM.NET project http://rapier-loom.net *
// (C) Copyright by Wolfgang Schult. All rights reserved. 
// This code is licensed under the Apache License 2.0
// To get more information, go to http://loom.codeplex.com/license
// ----------------------------------------------------------------
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Text;

using Loom.Common;
using Loom.CodeBuilder;
using Loom.CodeBuilder.DynamicProxy.DataSlots;
using Loom.CodeBuilder.DynamicProxy.Parameter;
using Loom.CodeBuilder.DynamicProxy.CodeBricks;
using Loom.Runtime;
using Loom.JoinPoints.Implementation;
using Loom.CodeBuilder.DynamicProxy.Declarations;


namespace Loom.CodeBuilder.DynamicProxy
{

    /// <summary>
    /// Hier werden die Methoden der Zielklasse in einem durch einen
    /// <see cref="ProxyTypeBuilderBase"/> generierten Typ erzeugt
    /// </summary>
    internal class ProxyAdviceMethodBuilder : ProxyAdviceMemberBuilder
    {
        /// <summary>
        /// Methode, die aufgerufen werden muss, um die Basisimplementation zu erreichen, Null wenn mbmethodimplementation gerufen werden soll
        /// </summary>
        MethodBuilder mibasecallmethod;
        /// <summary>
        /// Die Implementation der Methode
        /// </summary>
        MethodBuilder mbmethodimplementation;
        /// <summary>
        /// Der Aspektindex
        /// </summary>
        int aspectindex;

        // general info
        public ProxyAdviceMethodBuilder(ProxyTypeBuilderBase iwtbuilder, MethodBaseJoinPoint joinpoint, int aspectindex) :
            base(iwtbuilder, joinpoint)
        {
            if (aspectindex >= 0)
            {
                this.DataSlots.Aspect = new AspectSlot(Declarations.Aspects[aspectindex]);
            }
            this.aspectindex = aspectindex;
        }

        public new MethodInfoJoinPoint JoinPoint
        {
            get
            {
                return (MethodInfoJoinPoint)joinpoint;
            }
        }

        public override MethodBaseJoinPoint BindJoinPointToType()
        {
            return ((IBindMethod)joinpoint).BindToType(mbmethodimplementation, mibasecallmethod);
        }



        public override CodeBrickBuilder DefineAspectCall(MethodInfo aspectmethod, Loom.JoinPoints.Advice advice)
        {
            if (advice == Loom.JoinPoints.Advice.Initialize)
            {
                return new ProxyInitializeMethodCodeBrickBuilder(this, aspectmethod, aspectindex);
            }
            else
            {
                return new ProxyAdviceMethodCodeBrickBuilder(this, aspectmethod, advice);
            }
        }


        public override void Emit()
        {
            // BuildMethod
            MethodInfo basemethod = JoinPoint.MethodImplementation;

            if (methodcode == null && !basemethod.IsAbstract)
            {
                methodcode = BaseMethodCall.GetObject();
            }

            Type[] parameters = Common.Conversion.ToTypeArray(basemethod.GetParameters());
            if (JoinPoint.MethodInterface == null)
            {
                mbmethodimplementation = DefineVirtualMethod(TypeBuilder, basemethod, basemethod.ReturnType, parameters);
                EmitMethod(mbmethodimplementation);
            }
            else
            {
                mibasecallmethod = DefineInterfaceImplementationMethod(TypeBuilder, JoinPoint.MethodInterface, basemethod.ReturnType, parameters);
                mbmethodimplementation = DefineAndEmitInterfaceMethod(TypeBuilder, JoinPoint.MethodInterface, basemethod.ReturnType, parameters, mibasecallmethod);

                EmitMethod(mibasecallmethod);
            }
        }

        protected override ContextClassDeclaration GetContextClassDeclaration()
        {
            return this.DeclaringBuilder.CreateCallContextClassDeclaration(this);
        }
    }
}
